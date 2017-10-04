using System;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.Services
{
    internal class CutoffDatesService : ICutoffDatesService
    {
        private const int DayInMonthForSubmissionCutoff = 20;
        private const int DayInMonthForSubmissionProcessing = 23;
        private const int MonthModifierToAlignWithCalendarMonths = 4;

        public DateTime GetDateTimeForSubmissionCutoff(PayrollPeriod payrollPeriod)
        {
            return GetDateOfCutoffInUtc(payrollPeriod, DayInMonthForSubmissionCutoff);
        }

        public DateTime GetDateTimeForProcessingCutoff(PayrollPeriod payrollPeriod)
        {
            return GetDateOfCutoffInUtc(payrollPeriod, DayInMonthForSubmissionProcessing);
        }

        private static DateTime GetDateOfCutoffInUtc(PayrollPeriod payrollPeriod, int dateOfCutoff)
        {
           var monthOfProcessing = payrollPeriod.Month + MonthModifierToAlignWithCalendarMonths;
            var yearOfProcessing = 2000 + int.Parse(payrollPeriod.Year.Substring(0, 2));

            if (monthOfProcessing > 12)
            {
                monthOfProcessing = monthOfProcessing - 12;
                yearOfProcessing++;
            }

            return new DateTime(yearOfProcessing, monthOfProcessing, dateOfCutoff, 00, 00, 00, DateTimeKind.Utc);
        }

    }
}