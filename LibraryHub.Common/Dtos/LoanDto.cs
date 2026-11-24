using System;
using System.Collections.Generic;
using System.Text;
using LibraryHub.Common.Enums;

namespace LibraryHub.Common.Dtos
{
    public class LoanDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public DateTime BorrowedOn { get; set; }
        public DateTime DueOn { get; set; }
        public DateTime? ReturnedOn { get; set; }
        public decimal LateFee { get; set; }
        public LoanStatus Status { get; set; }
    }
}