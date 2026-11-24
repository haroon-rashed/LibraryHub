using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryHub.Common.Constants
{
    public static class LoanRules
    {
        public const int MaxActiveLoansPerMember = 3;
        public const int LoanPeriodDays = 14;
        public const decimal LateFeePerDay = 10;
    }
}