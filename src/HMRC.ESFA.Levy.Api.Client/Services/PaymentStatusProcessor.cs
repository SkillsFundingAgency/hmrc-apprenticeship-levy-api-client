using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.Services
{
    public class PaymentStatusProcessor : IPaymentStatusProcessor
    {
        private const int DayInMonthForSubmissionCutoff = 20;
        private const int DayInMonthForSubmissionProcessing = 23;

        public List<Declaration> ProcessDeclarationPaymentStatuses(List<Declaration> declarations, DateTime dateAdded)
        {
            foreach (var payrollPeriod in FindPayrollPeriods(declarations))
            {
                SetDeclarationPaymentStatusesByYearMonth(declarations, dateAdded,payrollPeriod);
            }

            return declarations;
        }

        private static IEnumerable<PayrollPeriod> FindPayrollPeriods(IEnumerable<Declaration> declarations)
        {
            return declarations
                .Where(x => x.PayrollPeriod != null)
                .Select(x => new PayrollPeriod {Year = x.PayrollPeriod.Year, Month = x.PayrollPeriod.Month})
                .Distinct();
        }

        private void SetDeclarationPaymentStatusesByYearMonth(IEnumerable<Declaration> declarations, DateTime dateAdded, PayrollPeriod payrollPeriod)
        {
            var significantProcessingDates = GetDates(dateAdded, payrollPeriod);

            var periodDeclarations = declarations
                .Where(x => x.PayrollPeriod != null && x.PayrollPeriod.Year == payrollPeriod.Year && x.PayrollPeriod.Month == payrollPeriod.Month);

            SetPaymentStatuses(periodDeclarations, significantProcessingDates);
        }

        private SignificantProcessingDates GetDates(DateTime dateAdded, PayrollPeriod payrollPeriod)
        {
            return new SignificantProcessingDates
            {
                DateAdded = dateAdded,
                DateOfCutoffForProcessing = GetDateOfCutoffInUtc(payrollPeriod, DayInMonthForSubmissionProcessing),
                dateOfCutoffForSubmission = GetDateOfCutoffInUtc(payrollPeriod, DayInMonthForSubmissionCutoff)
            };
        }

        private static void SetPaymentStatuses(IEnumerable<Declaration> periodDeclarations, SignificantProcessingDates significantProcessingDates)
        {
            if (significantProcessingDates.DateAdded.ToUniversalTime() < significantProcessingDates.DateOfCutoffForProcessing)
            {
                periodDeclarations
                    .SetLateDeclarations(significantProcessingDates.dateOfCutoffForSubmission)
                    .SetLatestDeclaration(significantProcessingDates.dateOfCutoffForSubmission);
            }
        }

        private static DateTime GetDateOfCutoffInUtc(PayrollPeriod payrollPeriod, int dateOfCutoff)
        {
            const int monthModifierToAlignWithCalendarMonths = 4;
            var monthOfProcessing = payrollPeriod.Month + monthModifierToAlignWithCalendarMonths;
            var yearOfProcessing = 2000 + int.Parse(payrollPeriod.Year.Substring(0, 2));
            if (monthOfProcessing > 12)
            {
                monthOfProcessing = monthOfProcessing - 12;
                yearOfProcessing++;
            }

            return new DateTime(yearOfProcessing, monthOfProcessing, dateOfCutoff, 00, 00, 00, DateTimeKind.Utc);
        }
    }

    public static class IEnumerableDeclarationExtensions
    {
        public static IEnumerable<Declaration> SetLateDeclarations(this IEnumerable<Declaration> list, DateTime dateOfCutoffForSubmission)
        {
            foreach (var declaration in list)
            {
                if (declaration.SubmissionTime.ToUniversalTime() >= dateOfCutoffForSubmission)
                {
                    declaration.LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.LatePayment;
                }
            }

            return list;
        }

        public static IEnumerable<Declaration> SetLatestDeclaration(this IEnumerable<Declaration> periodDeclarations, DateTime dateOfCutoffForSubmission)
        {
            var latestDeclaration =
                periodDeclarations
                    .OrderByDescending(x => x.SubmissionTime)
                    .First(
                    declaration => declaration.SubmissionTime.ToUniversalTime() < dateOfCutoffForSubmission);

            if (latestDeclaration != null)
            {
                latestDeclaration.LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.LatestPayment;
            }

            return periodDeclarations;
        }
    }

    public class SignificantProcessingDates
    {
        public DateTime DateAdded { get; set; }
        public DateTime DateOfCutoffForProcessing { get; set; }
        public DateTime dateOfCutoffForSubmission { get; set; }
    }
}
