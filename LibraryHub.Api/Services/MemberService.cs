using AutoMapper;
using LibraryHub.Api.Data;
using LibraryHub.Api.Entities;
using LibraryHub.Common.Dtos;
using LibraryHub.Common.Responses;
using Microsoft.EntityFrameworkCore;

namespace LibraryHub.Api.Services
{
    public class MemberService : IMemberService
    {
        private readonly LibraryDbContext _db;
        private readonly IMapper _mapper;

        public MemberService(LibraryDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagedResult<MemberDto>> GetAllAsync(string? search, bool activeOnly, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.Members.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(m => m.FullName.Contains(term) || m.Email.Contains(term));
            }

            if (activeOnly)
            {
                query = query.Where(m => m.IsActive);
            }

            var totalCount = await query.CountAsync();

            var members = await query
                .OrderBy(m => m.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<MemberDto>
            {
                Items = _mapper.Map<List<MemberDto>>(members),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ServiceResult<MemberDto>> GetByIdAsync(int id)
        {
            var member = await _db.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return ServiceResult<MemberDto>.NotFound($"Member with id {id} was not found.");
            }

            return ServiceResult<MemberDto>.Success(_mapper.Map<MemberDto>(member));
        }

        public async Task<ServiceResult<MemberDto>> CreateAsync(SaveMemberDto dto)
        {
            var email = dto.Email.Trim();

            if (await _db.Members.AnyAsync(m => m.Email == email))
            {
                return ServiceResult<MemberDto>.Conflict($"A member with email {email} already exists.");
            }

            var member = _mapper.Map<Member>(dto);
            member.Email = email;
            member.JoinedOn = DateTime.UtcNow;

            _db.Members.Add(member);
            await _db.SaveChangesAsync();

            return ServiceResult<MemberDto>.Success(_mapper.Map<MemberDto>(member));
        }

        public async Task<ServiceResult<MemberDto>> UpdateAsync(int id, SaveMemberDto dto)
        {
            var member = await _db.Members.FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return ServiceResult<MemberDto>.NotFound($"Member with id {id} was not found.");
            }

            var email = dto.Email.Trim();

            if (await _db.Members.AnyAsync(m => m.Email == email && m.Id != id))
            {
                return ServiceResult<MemberDto>.Conflict($"A member with email {email} already exists.");
            }

            _mapper.Map(dto, member);
            member.Email = email;

            await _db.SaveChangesAsync();

            return ServiceResult<MemberDto>.Success(_mapper.Map<MemberDto>(member));
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var member = await _db.Members.FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return ServiceResult<bool>.NotFound($"Member with id {id} was not found.");
            }

            if (await _db.Loans.AnyAsync(l => l.MemberId == id && l.ReturnedOn == null))
            {
                return ServiceResult<bool>.Conflict("This member has active loans and cannot be deleted.");
            }

            var returnedLoans = _db.Loans.Where(l => l.MemberId == id);
            _db.Loans.RemoveRange(returnedLoans);
            _db.Members.Remove(member);
            await _db.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
    }
}