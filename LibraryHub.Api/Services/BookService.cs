using AutoMapper;
using LibraryHub.Api.Data;
using LibraryHub.Api.Entities;
using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;
using Microsoft.EntityFrameworkCore;

namespace LibraryHub.Api.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _db;
        private readonly IMapper _mapper;

        public BookService(LibraryDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagedResult<BookDto>> GetAllAsync(string? search, bool availableOnly, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.Books.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(b => b.Title.Contains(term) || b.Author.Contains(term));
            }

            if (availableOnly)
            {
                query = query.Where(b => b.AvailableCopies > 0);
            }

            var totalCount = await query.CountAsync();

            var books = await query
                .OrderBy(b => b.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<BookDto>
            {
                Items = _mapper.Map<List<BookDto>>(books),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ServiceResult<BookDto>> GetByIdAsync(int id)
        {
            var book = await _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return ServiceResult<BookDto>.NotFound($"Book with id {id} was not found.");
            }

            return ServiceResult<BookDto>.Success(_mapper.Map<BookDto>(book));
        }

        public async Task<ServiceResult<BookDto>> CreateAsync(SaveBookDto dto)
        {
            var isbn = dto.Isbn.Trim();

            if (await _db.Books.AnyAsync(b => b.Isbn == isbn))
            {
                return ServiceResult<BookDto>.Conflict($"A book with ISBN {isbn} already exists.");
            }

            var book = _mapper.Map<Book>(dto);
            book.Isbn = isbn;
            book.AvailableCopies = book.TotalCopies;

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            return ServiceResult<BookDto>.Success(_mapper.Map<BookDto>(book));
        }

        public async Task<ServiceResult<BookDto>> UpdateAsync(int id, SaveBookDto dto)
        {
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return ServiceResult<BookDto>.NotFound($"Book with id {id} was not found.");
            }

            var isbn = dto.Isbn.Trim();

            if (await _db.Books.AnyAsync(b => b.Isbn == isbn && b.Id != id))
            {
                return ServiceResult<BookDto>.Conflict($"A book with ISBN {isbn} already exists.");
            }

            var copiesOnLoan = book.TotalCopies - book.AvailableCopies;

            if (dto.TotalCopies < copiesOnLoan)
            {
                return ServiceResult<BookDto>.BadRequest(
                    $"Total copies cannot be less than the {copiesOnLoan} copies currently on loan.");
            }

            _mapper.Map(dto, book);
            book.Isbn = isbn;
            book.AvailableCopies = book.TotalCopies - copiesOnLoan;

            await _db.SaveChangesAsync();

            return ServiceResult<BookDto>.Success(_mapper.Map<BookDto>(book));
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return ServiceResult<bool>.NotFound($"Book with id {id} was not found.");
            }

            if (await _db.Loans.AnyAsync(l => l.BookId == id && l.ReturnedOn == null))
            {
                return ServiceResult<bool>.Conflict("This book has active loans and cannot be deleted.");
            }

            var returnedLoans = _db.Loans.Where(l => l.BookId == id);
            _db.Loans.RemoveRange(returnedLoans);
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
    }
}