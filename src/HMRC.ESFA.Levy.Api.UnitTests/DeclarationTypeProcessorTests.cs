using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Client.Services;
using HMRC.ESFA.Levy.Api.Types;
using NUnit.Framework;

namespace HMRC.ESFA.Levy.Api.UnitTests
{
    [TestFixture]
    public class DeclarationTypeProcessorTests
    {
        private DeclarationTypeProcessor _processor;
        private List<Declaration> _declarations;
        private List<Declaration> _newDeclarations;
        private List<Declaration> _emptyDeclarations;
        private List<Declaration> _emptyDeclarationResult;

        [SetUp]
        public void Init()
        {
            _processor = new DeclarationTypeProcessor();

            _emptyDeclarations = new List<Declaration>();

            _declarations = new List<Declaration>
            {
                new Declaration
                {
                    Id = "Late2",
                    SubmissionTime = new DateTime(2017, 12,20, 00,00,00),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = "2017"},
                  },
                new Declaration
                {
                    Id = "unprocessedAndVeryEarly",
                    SubmissionTime = new DateTime(2017, 1,20, 00,00,00),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = "2017"},
                 },
                new Declaration
                {
                    Id = "LastBeforeCutoff",
                    SubmissionTime = new DateTime(2017, 08,19, 23,59,59),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = "2017"}
                },
                new Declaration
                {
                    Id = "secondLastBeforeCutoff",
                    SubmissionTime = new DateTime(2017, 08, 19, 10,15,00),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = "2017"},
                  },
                new Declaration
                {
                    Id = "Late1",
                    SubmissionTime = new DateTime(2017, 08, 20, 0,0,0),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = "2017"},
                }
            };

            _newDeclarations = _processor.ProcessDeclarationEntryTypes(_declarations);

            _emptyDeclarationResult = _processor.ProcessDeclarationEntryTypes(_emptyDeclarations);

        }

        [Test]
        public void ShouldReturnEmtpyListIfPassedEmptyList()
        {
              Assert.AreEqual(_emptyDeclarationResult.Count, 0);
        }


        [Test]
        public void ShouldSetLateEntry1ToLate()
        {
            var expectedLateEntry = _newDeclarations.FirstOrDefault(x => x.Id == "Late1");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatePayment);
        }

        [Test]
        public void ShouldSetLateEntry2ToLate()
        {
            var expectedLateEntry = _newDeclarations.FirstOrDefault(x => x.Id == "Late2");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatePayment);
        }

        [Test]
        public void ShouldSetUnprocessedAndEarly()
        {
            var expectedLateEntry = _newDeclarations.FirstOrDefault(x => x.Id == "unprocessedAndVeryEarly");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetLastCutoffToLastBeforeCutoff()
        {
            var expectedLateEntry = _newDeclarations.FirstOrDefault(x => x.Id == "LastBeforeCutoff");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatestPayment);
        }

        [Test]
        public void ShouldSetSecondLastCutoffToStandard()
        {
            var expectedLateEntry = _newDeclarations.FirstOrDefault(x => x.Id == "secondLastBeforeCutoff");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.UnprocessedPayment);
        }
    }
}

