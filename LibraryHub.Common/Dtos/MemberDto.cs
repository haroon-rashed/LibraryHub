using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryHub.Common.Dtos
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime JoinedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
