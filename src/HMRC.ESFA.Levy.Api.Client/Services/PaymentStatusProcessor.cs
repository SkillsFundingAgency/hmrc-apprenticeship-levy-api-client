using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.Services
{
    public class PaymentStatusProcessor : IPaymentStatusProcessor
    {

        public List<Declaration> ProcessDeclarationPaymentStatuses(List<Declaration> declarations, DateTime dateAdded)
        {
            var distinctPeriodsInPayroll = declarations
                .Select(x => x.PayrollPeriod.Year + x.PayrollPeriod.Month.ToString())
                .Distinct();

            foreach (var period in distinctPeriodsInPayroll)
            {
                foreach (var declaration in declarations
                    .Where(x => x.PayrollPeriod.Year + x.PayrollPeriod.Month.ToString() == period)
                    .OrderByDescending(x => x.SubmissionTime))
                {

                    var dateOfCutoffForProcessing = DateOfCutoffUtc(declaration.PayrollPeriod, 23);
            
                    if (dateAdded.ToUniversalTime() >= dateOfCutoffForProcessing) break;

                    var dateOfCutoffForSubmission = DateOfCutoffUtc(declaration.PayrollPeriod, 20);

                    if (declaration.SubmissionTime.ToUniversalTime() >= dateOfCutoffForSubmission)
                        declaration.LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.LatePayment;
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
            var monthOfProcessing = payrollPeriod.Month + 4;
            var yearOfProcessing = 2000 + int.Parse(payrollPeriod.Year.Substring(0, 2));
            if (monthOfProcessing > 12)
            {
                monthOfProcessing = monthOfProcessing - 12;
                yearOfProcessing = yearOfProcessing + 1;
            }

            return new DateTime(yearOfProcessing, monthOfProcessing, dateOfCutoff, 00, 00, 00, DateTimeKind.Utc);
        }
    }
}
