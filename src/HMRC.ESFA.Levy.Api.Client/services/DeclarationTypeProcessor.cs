using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.services
{
    public interface IDeclarationTypeProcessor
    {
        List<Declaration> ProcessDeclarationEntryTypes(List<Declaration> declarations);
    }

    public class DeclarationTypeProcessor: IDeclarationTypeProcessor
    {
        public List<Declaration> ProcessDeclarationEntryTypes(List<Declaration> declarations)
        {
            foreach(var declaration in declarations) declaration.LevyDeclarationType = LevyDeclarationType.Unprocessed;

            var distinctPeriodsInPayroll = declarations
                .Select(x => int.Parse(x.PayrollPeriod.Year + x.PayrollPeriod.Month.ToString()))
                .Distinct();

            foreach (var period in distinctPeriodsInPayroll)
            {
                foreach (var declaration in declarations
                    .Where(x => int.Parse(x.PayrollPeriod.Year + x.PayrollPeriod.Month.ToString()) == period)
                    .OrderByDescending(x => x.SubmissionTime))
                    {
                    var targetYear = int.Parse(declaration.PayrollPeriod.Year);
                    var targetMonth = ExtractTargetMonth(declaration.PayrollPeriod.Month);

                        if (declaration.SubmissionTime >= new DateTime(targetYear, targetMonth, 20))
                        declaration.LevyDeclarationType = LevyDeclarationType.Late;
                    else
                    { 
                        declaration.LevyDeclarationType = LevyDeclarationType.Latest;
                        break;
                    }
                }
            }

            return declarations;
        }

        private static int ExtractTargetMonth(short month)
        {
            var targetMonth =month + 3;
            if (targetMonth > 12)
                targetMonth = targetMonth - 12;
            return targetMonth;
        }
    }
}
