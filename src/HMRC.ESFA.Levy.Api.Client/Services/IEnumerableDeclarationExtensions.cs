using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.Services
{
    public static class IEnumerableDeclarationExtensions
    {
        public static IEnumerable<Declaration> SetLateDeclarations(this IEnumerable<Declaration> list, DateTime dateOfCutoffForSubmission)
        {
            foreach (var declaration in list.Where(declaration => declaration.SubmissionTime.ToUniversalTime() >= dateOfCutoffForSubmission))
            {             
                    declaration.LevyDeclarationSubmissionStatus = LevyDeclarationSubmissionStatus.LateSubmission;
            }

            return list;
        }

        public static IEnumerable<Declaration> SetLatestDeclaration(this IEnumerable<Declaration> periodDeclarations, DateTime dateOfCutoffForSubmission)
        {
            var latestDeclaration =
                periodDeclarations
                .Where(x => x.LevyDeclarationSubmissionStatus != LevyDeclarationSubmissionStatus.LateSubmission)
                    .OrderByDescending(x => x.SubmissionTime)
                    .FirstOrDefault(
                        declaration => declaration.SubmissionTime.ToUniversalTime() < dateOfCutoffForSubmission);

            if (latestDeclaration != null)
            {
                latestDeclaration.LevyDeclarationSubmissionStatus = LevyDeclarationSubmissionStatus.LatestSubmission;
            }

            return periodDeclarations;
        }
    }
}