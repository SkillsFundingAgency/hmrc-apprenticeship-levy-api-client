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
        private List<Declaration> _newDeclarationsStandardTests;
        private List<Declaration> _emptyDeclarations;
        private List<Declaration> _emptyDeclarationResult;
        private List<Declaration> _newDeclarationsPassedAccountCreatedDateTests;
        private List<Declaration> _newDeclarationsAccountCreatedAtStartOfYearTests;
        private const string PayrollYear = "17-18";

        [SetUp]
        public void Init()
        {
            _processor = new DeclarationTypeProcessor();

            _emptyDeclarations = new List<Declaration>();

            _declarations = GetDeclarationList();

            var declarationsWithPassedAccountCreatedDate = GetDeclarationList();

            var dateAccountCreated = new DateTime(2017, 9, 23, 00, 59, 59, DateTimeKind.Local);

            _newDeclarationsStandardTests = _processor.ProcessDeclarationEntryTypes(_declarations, dateAccountCreated);

            _emptyDeclarationResult = _processor.ProcessDeclarationEntryTypes(_emptyDeclarations, new DateTime());

            var dateAccountCreatedAfterCutoff = new DateTime(2017, 09, 23, 01, 00, 00, DateTimeKind.Local);

            _newDeclarationsPassedAccountCreatedDateTests = _processor
                .ProcessDeclarationEntryTypes(declarationsWithPassedAccountCreatedDate, dateAccountCreatedAfterCutoff);

            var dateAccountCreatedStartOfYear = new DateTime(2017, 01, 01, 00, 00, 00, DateTimeKind.Local);

            _newDeclarationsAccountCreatedAtStartOfYearTests = GetDeclarationList();

            _newDeclarationsAccountCreatedAtStartOfYearTests = _processor
                .ProcessDeclarationEntryTypes(_newDeclarationsAccountCreatedAtStartOfYearTests,
                    dateAccountCreatedStartOfYear);

        }

        private static List<Declaration> GetDeclarationList()
        {
            return new List<Declaration>
            {
                new Declaration
                {
                    Id = "Late2",
                    SubmissionTime = new DateTime(2017, 12, 20, 00, 00, 00),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear},
                },
                new Declaration
                {
                    Id = "unprocessedAndVeryEarly",
                    SubmissionTime = new DateTime(2017, 1, 20, 00, 00, 00),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear},
                },
                new Declaration
                {
                    Id = "LastBeforeCutoff",
                    SubmissionTime = new DateTime(2017, 09, 20, 0, 59, 59),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear}
                },
                new Declaration
                {
                    Id = "secondLastBeforeCutoff",
                    SubmissionTime = new DateTime(2017, 09, 19, 10, 15, 00),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear},
                },
                new Declaration
                {
                    Id = "Late1",
                    SubmissionTime = new DateTime(2017, 09, 20, 1, 0, 0),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear},
                },
                new Declaration
                {
                    Id = "early",
                    SubmissionTime = new DateTime(2017, 8, 19, 00, 00, 00),
                    PayrollPeriod = new PayrollPeriod {Month = 4, Year = PayrollYear},
                },
            };
        }

        [Test]
        public void ShouldReturnEmtpyListIfPassedEmptyList()
        {
            Assert.AreEqual(_emptyDeclarationResult.Count, 0);
        }

        [Test]
        public void ShouldSetLateEntry1ToLate()
        {
            var expectedLateEntry = _newDeclarationsStandardTests.First(x => x.Id == "Late1");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatePayment);
        }

        [Test]
        public void ShouldSetLateEntry2ToLate()
        {
            var expectedLateEntry = _newDeclarationsStandardTests.First(x => x.Id == "Late2");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatePayment);
        }

        [Test]
        public void ShouldSetUnprocessedAndEarly()
        {
            var expectedLateEntry = _newDeclarationsStandardTests.First(x => x.Id == "unprocessedAndVeryEarly");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetLastCutoffToLastBeforeCutoff()
        {
            var expectedLateEntry = _newDeclarationsStandardTests.First(x => x.Id == "LastBeforeCutoff");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatestPayment);
        }

        [Test]
        public void ShouldSetSecondLastCutoffToStandard()
        {
            var expectedLateEntry = _newDeclarationsStandardTests.First(x => x.Id == "secondLastBeforeCutoff");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }


        [Test]
        public void ShouldSetDeclarationBeforeDateAddedToUnprocessed()
        {
            var expectedLateEntry = _newDeclarationsStandardTests.First(x => x.Id == "early");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }


        [Test]
        public void ShouldSetLateEntry1WithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry = _newDeclarationsPassedAccountCreatedDateTests.First(x => x.Id == "Late1");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }


        [Test]
        public void ShouldSetLateEntry2WithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry = _newDeclarationsPassedAccountCreatedDateTests.First(x => x.Id == "Late2");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetUnprocessedAndEarlywithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry =
                _newDeclarationsPassedAccountCreatedDateTests.First(x => x.Id == "unprocessedAndVeryEarly");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetLastCutoffwithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry =
                _newDeclarationsPassedAccountCreatedDateTests.First(x => x.Id == "LastBeforeCutoff");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetSecondLastCutoffwithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry =
                _newDeclarationsPassedAccountCreatedDateTests.First(x => x.Id == "secondLastBeforeCutoff");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }


        [Test]
        public void ShouldSetDeclarationBeforeDateAddedwithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry = _newDeclarationsPassedAccountCreatedDateTests.First(x => x.Id == "early");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetDeclarationBeforeDateAddedToUnprocessedOnAccountOpenAtYearStart()
        {
            var expectedLateEntry = _newDeclarationsAccountCreatedAtStartOfYearTests.First(x => x.Id == "early");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatestPayment);
        }


        [Test]
        public void ShouldThrowAFormatExceptionIfYearFormatIncorrect()
        {
            var brokenDeclarationList = new List<Declaration>
            {

            new Declaration
            {
                Id = "broken",
                SubmissionTime = new DateTime(2017, 8, 01, 00, 00, 00),
                PayrollPeriod = new PayrollPeriod {Month = 5, Year = "brokenFormat"}
            }
        };
            var dateAccountCreated = new DateTime(2017, 01, 01, 00, 00, 00, DateTimeKind.Local);

            var processor = new DeclarationTypeProcessor();
            Assert.Throws<FormatException>(() => processor.ProcessDeclarationEntryTypes(brokenDeclarationList, dateAccountCreated));
    }
}
}

