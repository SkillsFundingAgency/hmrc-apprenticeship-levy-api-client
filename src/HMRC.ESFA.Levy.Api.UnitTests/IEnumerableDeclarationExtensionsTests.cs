using HMRC.ESFA.Levy.Api.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Client.Services;

namespace HMRC.ESFA.Levy.Api.UnitTests
{
    [TestFixture]
    public class IEnumerableDeclarationExtensionsTests
    {
        [Test]
        public void ShouldSetLatePaymentIfSubmittedOnCutOffDate()
        {
            var declarations = new List<Declaration>();
            declarations.Add(new Declaration
            {
                SubmissionTime = new DateTime(2017, 5, 20, 00, 00, 00, DateTimeKind.Utc),
                LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.UnprocessedPayment
            });
            var result = declarations.SetLateDeclarations(new DateTime(2017, 5, 20));

            Assert.AreEqual(LevyDeclarationPaymentStatus.LatePayment, result.Single().LevyDeclarationPaymentStatus);
        }

        [Test]
        public void ShouldSetLatePaymentIfSubmittedAfterCutOffDate()
        {
            var declarations = new List<Declaration>();
            declarations.Add(new Declaration
            {
                SubmissionTime = new DateTime(2017, 5, 21, 00, 00, 00, DateTimeKind.Utc),
                LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.UnprocessedPayment
            });
            var result = declarations.SetLateDeclarations(new DateTime(2017, 5, 20));

            Assert.AreEqual(LevyDeclarationPaymentStatus.LatePayment, result.Single().LevyDeclarationPaymentStatus);
        }
        [Test]
        public void ShouldNotSetLatePaymentIfSubmittedbeforeCutOffDate()
        {
            var declarations = new List<Declaration>();
            declarations.Add(new Declaration
            {
                SubmissionTime = new DateTime(2017, 5, 19, 00, 00, 00, DateTimeKind.Utc),
                LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.UnprocessedPayment
            });
            var result = declarations.SetLateDeclarations(new DateTime(2017, 5, 20));

            Assert.AreEqual(LevyDeclarationPaymentStatus.UnprocessedPayment, result.Single().LevyDeclarationPaymentStatus);
        }

        [Test]
        public void ShouldSetLatestPaymentIfSubmittedbeforeCutOffDate()
        {
            var declarations = new List<Declaration>();
            declarations.Add(new Declaration
            {
                SubmissionTime = new DateTime(2017, 5, 19, 00, 00, 00, DateTimeKind.Utc),
                LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.UnprocessedPayment
            });
            var result = declarations.SetLatestDeclaration(new DateTime(2017, 5, 20));

            Assert.AreEqual(LevyDeclarationPaymentStatus.LatestPayment, result.Single().LevyDeclarationPaymentStatus);
        }
        [Test]
        public void ShouldNotSetLatestPaymentIfSubmittedAfterCutOffDate()
        {
            var declarations = new List<Declaration>();
            declarations.Add(new Declaration
            {
                SubmissionTime = new DateTime(2017, 5, 21, 00, 00, 00, DateTimeKind.Utc),
                LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.UnprocessedPayment
            });
            var result = declarations.SetLatestDeclaration(new DateTime(2017, 5, 20));

            Assert.AreEqual(LevyDeclarationPaymentStatus.UnprocessedPayment, result.Single().LevyDeclarationPaymentStatus);
        }

        [Test]
        public void ShouldNotSetLatePaymentIfSubmittedOnCutOffDate()
        {
            var declarations = new List<Declaration>();
            declarations.Add(new Declaration
            {
                SubmissionTime = new DateTime(2017, 5, 20, 00, 00, 00, DateTimeKind.Utc),
                LevyDeclarationPaymentStatus = LevyDeclarationPaymentStatus.UnprocessedPayment
            });
            var result = declarations.SetLatestDeclaration(new DateTime(2017, 5, 20));

            Assert.AreEqual(LevyDeclarationPaymentStatus.UnprocessedPayment, result.Single().LevyDeclarationPaymentStatus);
        }
    }
}
