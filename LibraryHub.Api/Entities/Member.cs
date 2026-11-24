namespace LibraryHub.Api.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime JoinedOn { get; set; }
        public bool IsActive { get; set; }

        public List<Loan> Loans { get; set; } = [];
    }
}