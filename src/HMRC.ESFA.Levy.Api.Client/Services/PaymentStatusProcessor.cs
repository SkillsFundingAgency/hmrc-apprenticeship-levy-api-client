using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.Services
{
    internal class PaymentStatusProcessor : IPaymentStatusProcessor
    {
        private readonly ICutoffDatesService _cutoffDatesService;

        public PaymentStatusProcessor(ICutoffDatesService cutoffDatesService)
        {
            _cutoffDatesService = cutoffDatesService;
        }

        public List<Declaration> ProcessDeclarationsByPayrollPeriod(List<Declaration> declarations, DateTime dateTimeProcessingInvoked)
        {
            foreach (var payrollPeriod in FindPayrollPeriods(declarations))
            {
                SetDeclarationPaymentStatusesByYearMonth(declarations, payrollPeriod, dateTimeProcessingInvoked);
            }

            return declarations;
        }

        private static IEnumerable<PayrollPeriod> FindPayrollPeriods(IEnumerable<Declaration> declarations)
        {
            return declarations
                .Where(x => x.PayrollPeriod != null)
                .Select(x => new PayrollPeriod {Year = x.PayrollPeriod.Year, Month = x.PayrollPeriod.Month})
                .GroupBy(x => x.Year + x.Month)
                .Select(payroll => payroll.First());
        }

        private void SetDeclarationPaymentStatusesByYearMonth(IEnumerable<Declaration> declarations, PayrollPeriod payrollPeriod, DateTime dateTimeProcessingInvoked)
        {
            var significantProcessingDates = GetDates(dateTimeProcessingInvoked, payrollPeriod);

            var periodDeclarations = declarations
                .Where(x => x.PayrollPeriod != null && x.PayrollPeriod.Year == payrollPeriod.Year && x.PayrollPeriod.Month == payrollPeriod.Month);

            SetPaymentStatuses(periodDeclarations, significantProcessingDates);
        }

        private SignificantProcessingDates GetDates(DateTime dateTimeProcessingInvoked, PayrollPeriod payrollPeriod)
        {
            return new SignificantProcessingDates
            {
                DateProcessorInvoked = dateTimeProcessingInvoked,
                DateOfCutoffForProcessing = _cutoffDatesService.GetDateTimeForProcessingCutoff(payrollPeriod),
                DateOfCutoffForSubmission = _cutoffDatesService.GetDateTimeForSubmissionCutoff(payrollPeriod)
            };
        }

        private static void SetPaymentStatuses(IEnumerable<Declaration> periodDeclarations, SignificantProcessingDates significantProcessingDates)
        {
            if (significantProcessingDates.DateProcessorInvoked.ToUniversalTime() >= significantProcessingDates.DateOfCutoffForProcessing
                )
                {
                    periodDeclarations
                        .SetLateDeclarations(significantProcessingDates.DateOfCutoffForSubmission)
                        .SetLatestDeclaration(significantProcessingDates.DateOfCutoffForSubmission);
                }
        }

    }
}
