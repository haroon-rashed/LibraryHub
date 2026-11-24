using System;
using System.Collections.Generic;
using System.Text;
namespace LibraryHub.Common.Dtos
{
    public class DashboardDto
    {
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
    }
}