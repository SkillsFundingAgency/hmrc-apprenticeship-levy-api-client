using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.Services
{
    public class DeclarationTypeProcessor : IDeclarationTypeProcessor
    {
        private IDeclarationTypeProcessor _declarationTypeProcessorImplementation;

        public List<Declaration> ProcessDeclarationEntryTypes(List<Declaration> declarations, DateTime dateAdded)
        {
            var distinctPeriodsInPayroll = declarations
                .Select(x => int.Parse(x.PayrollPeriod.Year + x.PayrollPeriod.Month.ToString()))
                .Distinct();

            foreach (var period in distinctPeriodsInPayroll)
            {
                foreach (var declaration in declarations
                    .Where(x => int.Parse(x.PayrollPeriod.Year + x.PayrollPeriod.Month.ToString()) == period)
                    .OrderByDescending(x => x.SubmissionTime))
                {
                    if (declaration.SubmissionTime >= dateAdded)
                    {
                        var targetYear = int.Parse(declaration.PayrollPeriod.Year);
                        var targetMonth = ExtractTargetMonth(declaration.PayrollPeriod.Month);

                        if (declaration.SubmissionTime >= new DateTime(targetYear, targetMonth, 20))
                            declaration.LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.LatePayment;
                        else
                        {
                            declaration.LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.LatestPayment;
                            break;
                        }
                    }
                }
            }

            return declarations;
        }

        private static int ExtractTargetMonth(short month)
        {
            var targetMonth = month + 3;
            if (targetMonth > 12)
                targetMonth = targetMonth - 12;
            return targetMonth;
        }
    }
}
