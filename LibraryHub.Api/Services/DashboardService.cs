using LibraryHub.Api.Data;
using LibraryHub.Common.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryHub.Api.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly LibraryDbContext _db;

        public DashboardService(LibraryDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardDto> GetAsync()
        {
            var now = DateTime.UtcNow;

            return new DashboardDto
            {
                TotalBooks = await _db.Books.CountAsync(),
                TotalMembers = await _db.Members.CountAsync(),
                ActiveLoans = await _db.Loans.CountAsync(l => l.ReturnedOn == null),
                OverdueLoans = await _db.Loans.CountAsync(l => l.ReturnedOn == null && l.DueOn < now)
            };
        }
    }
}