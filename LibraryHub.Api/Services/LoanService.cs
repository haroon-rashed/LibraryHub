using LibraryHub.Api.Data;
using LibraryHub.Api.Entities;
using LibraryHub.Common.Constants;
using LibraryHub.Common.Dtos;
using LibraryHub.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace LibraryHub.Api.Services
{
    public class LoanService : ILoanService
    {
        private readonly LibraryDbContext _db;
        private readonly ILogger<LoanService> _logger;

        public LoanService(LibraryDbContext db, ILogger<LoanService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ServiceResult<List<LoanDto>>> GetAllAsync(string status)
        {
            var now = DateTime.UtcNow;
            var query = _db.Loans.AsNoTracking();

            switch (status.Trim().ToLowerInvariant())
            {
                case "all":
                    break;
                case "active":
                    query = query.Where(l => l.ReturnedOn == null);
                    break;
                case "overdue":
                    query = query.Where(l => l.ReturnedOn == null && l.DueOn < now);
                    break;
                default:
                    return ServiceResult<List<LoanDto>>.BadRequest("Status must be all, active or overdue.");
            }

            var loans = await query
                .OrderByDescending(l => l.BorrowedOn)
                .Select(l => new LoanDto
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    BookTitle = l.Book.Title,
                    MemberId = l.MemberId,
                    MemberName = l.Member.FullName,
                    BorrowedOn = l.BorrowedOn,
                    DueOn = l.DueOn,
                    ReturnedOn = l.ReturnedOn,
                    LateFee = l.LateFee
                })
                .ToListAsync();

            foreach (var loan in loans)
            {
                loan.Status = GetStatus(loan.ReturnedOn, loan.DueOn, now);
            }

            return ServiceResult<List<LoanDto>>.Success(loans);
        }

        public async Task<ServiceResult<LoanDto>> BorrowAsync(CreateLoanDto dto)
        {
            var member = await _db.Members.FirstOrDefaultAsync(m => m.Id == dto.MemberId);
            if (member == null)
            {
                return ServiceResult<LoanDto>.NotFound($"Member with id {dto.MemberId} was not found.");
            }

            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == dto.BookId);
            if (book == null)
            {
                return ServiceResult<LoanDto>.NotFound($"Book with id {dto.BookId} was not found.");
            }

            // Rule 3: an inactive member cannot borrow
            if (!member.IsActive)
            {
                return Reject("An inactive member cannot borrow books.", dto);
            }

            // Rule 2: a book can be borrowed only when copies are available
            if (book.AvailableCopies <= 0)
            {
                return Reject("No copies of this book are currently available.", dto);
            }

            // Rule 1: at most 3 active loans per member
            var activeLoanCount = await _db.Loans
                .CountAsync(l => l.MemberId == dto.MemberId && l.ReturnedOn == null);

            if (activeLoanCount >= LoanRules.MaxActiveLoansPerMember)
            {
                return Reject(
                    $"A member can have at most {LoanRules.MaxActiveLoansPerMember} active loans.", dto);
            }

            // Rule 6: no two active loans of the same book for one member
            var alreadyHasBook = await _db.Loans
                .AnyAsync(l => l.MemberId == dto.MemberId && l.BookId == dto.BookId && l.ReturnedOn == null);

            if (alreadyHasBook)
            {
                return Reject("This member already has an active loan of this book.", dto);
            }

            // Rule 4: due date is the borrow date plus the loan period
            var borrowedOn = DateTime.UtcNow;

            var loan = new Loan
            {
                Book = book,
                Member = member,
                BorrowedOn = borrowedOn,
                DueOn = borrowedOn.AddDays(LoanRules.LoanPeriodDays),
                LateFee = 0
            };

            // Rule 2: borrowing lowers the available copies by 1
            book.AvailableCopies--;

            _db.Loans.Add(loan);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Loan {LoanId} created: book {BookId} borrowed by member {MemberId}, due {DueOn}",
                loan.Id, book.Id, member.Id, loan.DueOn);

            return ServiceResult<LoanDto>.Success(ToDto(loan));
        }

        public async Task<ServiceResult<LoanDto>> ReturnAsync(int id)
        {
            var loan = await _db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                return ServiceResult<LoanDto>.NotFound($"Loan with id {id} was not found.");
            }

            // Rule 9: a returned loan cannot be returned again
            if (loan.ReturnedOn != null)
            {
                _logger.LogWarning("Return rejected: loan {LoanId} is already returned", id);
                return ServiceResult<LoanDto>.Conflict("This loan has already been returned.");
            }

            var returnedOn = DateTime.UtcNow;

            // Rule 5: late fee per overdue day, 0 if returned on time
            var daysOverdue = Math.Max(0, (returnedOn.Date - loan.DueOn.Date).Days);

            loan.ReturnedOn = returnedOn;
            loan.LateFee = daysOverdue * LoanRules.LateFeePerDay;

            // Rule 2: returning raises the available copies by 1
            loan.Book.AvailableCopies++;

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Loan {LoanId} returned: book {BookId}, member {MemberId}, late fee {LateFee}",
                loan.Id, loan.BookId, loan.MemberId, loan.LateFee);

            return ServiceResult<LoanDto>.Success(ToDto(loan));
        }

        private ServiceResult<LoanDto> Reject(string message, CreateLoanDto dto)
        {
            _logger.LogWarning(
                "Borrow rejected for member {MemberId}, book {BookId}: {Message}",
                dto.MemberId, dto.BookId, message);

            return ServiceResult<LoanDto>.Conflict(message);
        }

        private static LoanStatus GetStatus(DateTime? returnedOn, DateTime dueOn, DateTime now)
        {
            if (returnedOn != null)
            {
                return LoanStatus.Returned;
            }

            return dueOn < now ? LoanStatus.Overdue : LoanStatus.Active;
        }

        private static LoanDto ToDto(Loan loan)
        {
            return new LoanDto
            {
                Id = loan.Id,
                BookId = loan.BookId,
                BookTitle = loan.Book.Title,
                MemberId = loan.MemberId,
                MemberName = loan.Member.FullName,
                BorrowedOn = loan.BorrowedOn,
                DueOn = loan.DueOn,
                ReturnedOn = loan.ReturnedOn,
                LateFee = loan.LateFee,
                Status = GetStatus(loan.ReturnedOn, loan.DueOn, DateTime.UtcNow)
            };
        }
    }
}