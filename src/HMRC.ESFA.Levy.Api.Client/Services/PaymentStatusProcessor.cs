using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.Services
{
    public class PaymentStatusProcessor : IPaymentStatusProcessor
    {
        private const int DayInMonthForSubmissionProcessing = 23;
        private const int DayInMonthForSubmissionCutoff = 20;

        public List<Declaration> ProcessDeclarationPaymentStatuses(List<Declaration> declarations, DateTime dateAdded)
        {
            var declarationsWithPaymentPeriod = declarations.Where(x => x.PayrollPeriod != null).ToList();

            var distinctPeriodsInPayroll = declarationsWithPaymentPeriod
                .Select(x => $@"{x.PayrollPeriod.Year}{x.PayrollPeriod.Month}")
                .Distinct();

            foreach (var period in distinctPeriodsInPayroll)
            {
                foreach (var declaration in declarationsWithPaymentPeriod
                    .Where(x => $@"{x.PayrollPeriod.Year}{x.PayrollPeriod.Month}" == period)
                    .OrderByDescending(x => x.SubmissionTime))
                {
                    var dateOfCutoffForProcessing = DateOfCutoffUtc(declaration.PayrollPeriod, DayInMonthForSubmissionProcessing);
            
                    if (dateAdded.ToUniversalTime() >= dateOfCutoffForProcessing) break;

                    var dateOfCutoffForSubmission = DateOfCutoffUtc(declaration.PayrollPeriod, DayInMonthForSubmissionCutoff);

                    if (declaration.SubmissionTime.ToUniversalTime() >= dateOfCutoffForSubmission)
                    {
                        declaration.LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.LatePayment;
                    }
                    else
                    {
                        declaration.LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.LatestPayment;
                        break;
                    }
                }
            }

            return declarations;
        }

        private static DateTime DateOfCutoffUtc(PayrollPeriod payrollPeriod, int dateOfCutoff)
        {
            const int monthModifierToAlignWithCalendarMonths = 4;
            var yearRange = payrollPeriod.Year;
            var monthOfProcessing = payrollPeriod.Month + monthModifierToAlignWithCalendarMonths;
            var yearOfProcessing = 2000 + int.Parse(yearRange.Substring(0, 2));
            if (monthOfProcessing > 12)
            {
                monthOfProcessing = monthOfProcessing - 12;
                yearOfProcessing++;
            }

            return new DateTime(yearOfProcessing, monthOfProcessing, dateOfCutoff, 00, 00, 00, DateTimeKind.Utc);
        }
    }
}
