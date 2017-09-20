using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Client.Services;
using HMRC.ESFA.Levy.Api.Types;
using NUnit.Framework;

namespace HMRC.ESFA.Levy.Api.UnitTests
{
    [TestFixture]
    public class PaymentStatusesProcessorTests
    {
        private PaymentStatusProcessor _processor;
        private List<Declaration> _declarationsPostProcessed;
        private List<Declaration> _emptyDeclarationsPreprocessed;
        private List<Declaration> _emptyDeclarationPostProcessed;
        private List<Declaration> _declarationsAfterAccountCreatedDatePostProcessed;
        private List<Declaration> _declarationsAccountCreatedAtStartOfYearPostProcessed;
        private const string PayrollYear = "17-18";

        [SetUp]
        public void Init()
        {
            _processor = new PaymentStatusProcessor();

            _emptyDeclarationsPreprocessed = new List<Declaration>();

            var declarations = GetDeclarationList();

            var declarationsWithPassedAccountCreatedDate = GetDeclarationList();

            var dateAccountCreated = new DateTime(2017, 9, 22, 23, 59, 59, DateTimeKind.Utc);

            _declarationsPostProcessed = _processor.ProcessDeclarationPaymentStatuses(declarations, dateAccountCreated);

            _emptyDeclarationPostProcessed = _processor.ProcessDeclarationPaymentStatuses(_emptyDeclarationsPreprocessed, new DateTime());

            var dateAccountCreatedAfterCutoff = new DateTime(2017, 09, 23, 00, 00, 00, DateTimeKind.Utc);

            _declarationsAfterAccountCreatedDatePostProcessed = _processor
                .ProcessDeclarationPaymentStatuses(declarationsWithPassedAccountCreatedDate, dateAccountCreatedAfterCutoff);

            var dateAccountCreatedStartOfYear = new DateTime(2017, 01, 01, 00, 00, 00, DateTimeKind.Utc);

            var declarationsAccountCreatedAtStartOfYear = GetDeclarationList();

            _declarationsAccountCreatedAtStartOfYearPostProcessed = _processor
                .ProcessDeclarationPaymentStatuses(declarationsAccountCreatedAtStartOfYear,
                    dateAccountCreatedStartOfYear);

        }

        private static List<Declaration> GetDeclarationList()
        {
            return new List<Declaration>
            {
                new Declaration
                {
                    Id = "Late2",
                    SubmissionTime = new DateTime(2017, 12, 20, 00, 00, 00, DateTimeKind.Utc),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear},
                },
                new Declaration
                {
                    Id = "unprocessedAndVeryEarly",
                    SubmissionTime = new DateTime(2017, 1, 20, 00, 00, 00, DateTimeKind.Utc),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear},
                },
                new Declaration
                {
                    Id = "LastBeforeCutoff",
                    SubmissionTime = new DateTime(2017, 09, 19, 23, 59, 59, DateTimeKind.Utc),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear}
                },
                new Declaration
                {
                    Id = "secondLastBeforeCutoff",
                    SubmissionTime = new DateTime(2017, 09, 19, 10, 15, 00, DateTimeKind.Utc),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear},
                },
                new Declaration
                {
                    Id = "Late1",
                    SubmissionTime = new DateTime(2017, 09, 20, 0, 0, 0, DateTimeKind.Utc),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear},
                },
                new Declaration
                {
                    Id = "early",
                    SubmissionTime = new DateTime(2017, 8, 19, 00, 00, 00, DateTimeKind.Utc),
                    PayrollPeriod = new PayrollPeriod {Month = 4, Year = PayrollYear},
                },
                new Declaration
                {
                    Id = "noPayrollPeriod",
                    SubmissionTime = new DateTime(2017, 8, 19, 00, 00, 00, DateTimeKind.Utc)
                },
            };
        }

        [Test]
        public void ShouldReturnEmptyListIfPassedEmptyList()
        {
            Assert.AreEqual(_emptyDeclarationPostProcessed.Count, 0);
        }

        [Test]
        public void ShouldSetLate1PaymentStatusToLate()
        {
            var expectedLateEntry = _declarationsPostProcessed.First(x => x.Id == "Late1");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatePayment);
        }

        [Test]
        public void ShouldSetLate2PaymentStatusToLate()
        {
            var expectedLateEntry = _declarationsPostProcessed.First(x => x.Id == "Late2");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatePayment);
        }

        [Test]
        public void ShouldSetUnprocessedAndEarlyToUnprocessed()
        {
            var expectedLateEntry = _declarationsPostProcessed.First(x => x.Id == "unprocessedAndVeryEarly");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetLastCutoffToLastBeforeCutoffToLatestPayment()
        {
            var expectedLateEntry = _declarationsPostProcessed.First(x => x.Id == "LastBeforeCutoff");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatestPayment);
        }

        [Test]
        public void ShouldSetSecondLastCutoffToUnprocessed()
        {
            var expectedLateEntry = _declarationsPostProcessed.First(x => x.Id == "secondLastBeforeCutoff");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }


        [Test]
        public void ShouldSetDeclarationBeforeDateAddedToUnprocessed()
        {
            var expectedLateEntry = _declarationsPostProcessed.First(x => x.Id == "early");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }


        [Test]
        public void ShouldSetLateEntry1WithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry = _declarationsAfterAccountCreatedDatePostProcessed.First(x => x.Id == "Late1");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }


        [Test]
        public void ShouldSetLateEntry2WithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry = _declarationsAfterAccountCreatedDatePostProcessed.First(x => x.Id == "Late2");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetUnprocessedAndEarlywithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry =
                _declarationsAfterAccountCreatedDatePostProcessed.First(x => x.Id == "unprocessedAndVeryEarly");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetLastCutoffwithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry =
                _declarationsAfterAccountCreatedDatePostProcessed.First(x => x.Id == "LastBeforeCutoff");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetSecondLastCutoffwithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry =
                _declarationsAfterAccountCreatedDatePostProcessed.First(x => x.Id == "secondLastBeforeCutoff");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }


        [Test]
        public void ShouldSetDeclarationBeforeDateAddedwithPassedAccountDateAddedToUnprocessed()
        {
            var expectedLateEntry = _declarationsAfterAccountCreatedDatePostProcessed.First(x => x.Id == "early");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
        }

        [Test]
        public void ShouldSetSingleDeclarationInPeriodBeforeDateAddedToLatestOnAccountOpenAtYearStart()
        {
            var expectedLateEntry = _declarationsAccountCreatedAtStartOfYearPostProcessed.First(x => x.Id == "early");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatestPayment);
        }

        [Test]
        public void ShouldSetADeclarationWithoutAPaymentPeriodToUnprocessed()
        {
            var expectedLateEntry = _declarationsAccountCreatedAtStartOfYearPostProcessed.First(x => x.Id == "noPayrollPeriod");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.UnprocessedPayment);
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
            var dateAccountCreated = new DateTime(2017, 01, 01, 00, 00, 00, DateTimeKind.Utc);

            var processor = new PaymentStatusProcessor();
            Assert.Throws<FormatException>(() => processor.ProcessDeclarationPaymentStatuses(brokenDeclarationList, dateAccountCreated));
    }
}
}

