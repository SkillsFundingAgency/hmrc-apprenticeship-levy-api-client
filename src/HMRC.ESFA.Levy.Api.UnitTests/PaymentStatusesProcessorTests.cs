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
        private const string PayrollYear = "16-17";

        [SetUp]
        public void Init()
        {
            _processor = new PaymentStatusProcessor();

            var declarations = GetDeclarationList();  
            var dateProcessWasInvoked = new DateTime(2016, 10, 01, 00, 00, 00, DateTimeKind.Utc);
            _declarationsPostProcessed = _processor.ProcessDeclarationPaymentStatuses(declarations, dateProcessWasInvoked);
         }

        [Test]
        public void ShouldReturnEmptyListIfPassedEmptyList()
        {
            var emptyDeclarationPostProcessed = _processor.ProcessDeclarationPaymentStatuses(new List<Declaration>(), DateTime.Now);
            Assert.AreEqual(emptyDeclarationPostProcessed.Count, 0);
        }

        [Test]
        public void ShouldThrowAFormatExceptionIfYearFormatIncorrect()
        {
            var brokenDeclarationList = new List<Declaration>
            {
                new Declaration
                {
                    Id = "broken",
                    SubmissionTime = new DateTime(2016, 8, 01, 00, 00, 00),
                    PayrollPeriod = new PayrollPeriod {Month = 5, Year = "brokenFormat"}
                }
            };
    
            var processor = new PaymentStatusProcessor();
            Assert.Throws<FormatException>(() => processor.ProcessDeclarationPaymentStatuses(brokenDeclarationList, DateTime.Now));
        }

        [Test]
        public void ShouldSetLateEntryFirstPaymentStatusToLate()
        {
            var expectedLateEntry = _declarationsPostProcessed.First(x => x.Id == "LateEntryFirst");
            Assert.AreEqual(expectedLateEntry.LevyDeclarationPaymentStatus, LevyDeclarationPaymentStatus.LatePayment);
        }

        [Test]
        public void ShouldSetLateEntrySecondPaymentStatusToLate()
        {
            var expectedLateEntry = _declarationsPostProcessed.First(x => x.Id == "LateEntrySecond");
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
        public void ShouldSetLastBeforeCutoffToLatestPayment()
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
        public void ShouldSetFutureDeclarationLatestInPeriodAfterDateInvokedToUnprocessed()
        {
            var expectedEntry = _declarationsPostProcessed.First(x => x.Id == "futureEntryLatest");
            Assert.AreEqual(expectedEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment,
                "Future payroll entry that is latest (ie in a period not yet run) should be unprocessed");
        }

        private static readonly Declaration NoPayrollPeriod = new Declaration
        {
            Id = "noPayrollPeriod",
            SubmissionTime = new DateTime(2016, 8, 19, 00, 00, 00, DateTimeKind.Utc)
        };

        private static readonly Declaration LateEntrySecond = new Declaration
        {
            Id = "LateEntrySecond",
            SubmissionTime = new DateTime(2016, 9, 21, 00, 00, 00, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 5, Year = PayrollYear }
        };

        private static readonly Declaration UnprocessedAndVeryEarly = new Declaration
        {
            Id = "unprocessedAndVeryEarly",
            SubmissionTime = new DateTime(2016, 1, 20, 00, 00, 00, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod {Month = 5,Year = PayrollYear}
        };

        private static readonly Declaration LastBeforeCutoff = new Declaration
        {
            Id = "LastBeforeCutoff",
            SubmissionTime = new DateTime(2016, 09, 19, 23, 59, 59, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 5, Year = PayrollYear }
        };

        private static readonly Declaration SecondLastBeforeCutoff = new Declaration
        {
            Id = "secondLastBeforeCutoff",
            SubmissionTime = new DateTime(2016, 09, 19, 10, 15, 00, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 5,Year = PayrollYear },
        };
        private static readonly Declaration LateEntryFirst = new Declaration
        {
            Id = "LateEntryFirst",
            SubmissionTime = new DateTime(2016, 09, 20, 0, 0, 0, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 5, Year = PayrollYear },
        };

        private static readonly Declaration FutureEntryLatest = new Declaration
        {
            Id = "futureEntryLatest",
            SubmissionTime = new DateTime(2016, 9, 19, 00, 00, 00, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 6, Year = PayrollYear }
        };

        private static readonly Declaration FutureEntryLate = new Declaration
        {
            Id = "futureEntryLate",
            SubmissionTime = new DateTime(2016, 9, 21, 00, 00, 00, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 6, Year = PayrollYear }
        };

        private static List<Declaration> GetDeclarationList()
        {

            return new List<Declaration>
            {
                LateEntrySecond,
                UnprocessedAndVeryEarly,
                LastBeforeCutoff,
                SecondLastBeforeCutoff,
                LateEntryFirst,
                NoPayrollPeriod,
                FutureEntryLatest,
                FutureEntryLate

            };
        }
    }
}

