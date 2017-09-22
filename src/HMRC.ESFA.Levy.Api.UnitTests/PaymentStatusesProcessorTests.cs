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
        private List<Declaration> _emptyDeclarationPostProcessed;
        private const string PayrollYear = "16-17";

        [SetUp]
        public void Init()
        {
            _processor = new PaymentStatusProcessor();

            var declarations = GetDeclarationList();
      
            var dateAccountCreated = new DateTime(2016, 9, 22, 23, 59, 59, DateTimeKind.Utc);

            _declarationsPostProcessed = _processor.ProcessDeclarationPaymentStatuses(declarations, dateAccountCreated, DateTime.Now);

            _emptyDeclarationPostProcessed = _processor.ProcessDeclarationPaymentStatuses(new List<Declaration>(), DateTime.Now.AddDays(-60), DateTime.Now);
       }

        [Test]
        public void ShouldSetCorrectPaymentStatus()
        {
            var declarations = new List<Declaration>();
            declarations.Add(new Declaration
            {
                Id = "lateentry",
                SubmissionTime = new DateTime(2016, 9, 20, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = 4, Year = PayrollYear }
            });
            declarations.Add(new Declaration
            {
                Id = "latestentry",
                SubmissionTime = new DateTime(2016, 9, 18, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = 5, Year = PayrollYear }
            });
            declarations.Add(new Declaration
            {
                Id = "anotherlatestentry",
                SubmissionTime = new DateTime(2016, 9, 19, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = 5, Year = PayrollYear }
            });
            declarations.Add(new Declaration
            {
                Id = "futurepayrollentry",
                SubmissionTime = new DateTime(2016, 9, 19, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = 6, Year = PayrollYear }
            });

            var dateAccountCreated = new DateTime(2016, 9, 1, 23, 59, 59, DateTimeKind.Utc);
            var dateProcessWasInvoked = new DateTime(2016,10,01,00,00,00,DateTimeKind.Utc);
            _declarationsPostProcessed = _processor.ProcessDeclarationPaymentStatuses(declarations, dateAccountCreated, dateProcessWasInvoked);

            Assert.AreEqual(LevyDeclarationPaymentStatus.UnprocessedPayment,
                _declarationsPostProcessed.Single(x => x.Id == "lateentry").LevyDeclarationPaymentStatus,
                "Unprocessed expected as entry is late for period, but period fell before account date added");
            Assert.AreEqual(LevyDeclarationPaymentStatus.UnprocessedPayment,
                _declarationsPostProcessed.Single(x => x.Id == "latestentry").LevyDeclarationPaymentStatus,
                "Unprocessed expected as entry is second latest for period");
            Assert.AreEqual(LevyDeclarationPaymentStatus.LatestPayment,
                _declarationsPostProcessed.Single(x => x.Id == "anotherlatestentry").LevyDeclarationPaymentStatus,
                "LatestPayment expected as entry is latest for period");
            Assert.AreEqual(LevyDeclarationPaymentStatus.UnprocessedPayment,
                _declarationsPostProcessed.Single(x => x.Id == "futurepayrollentry").LevyDeclarationPaymentStatus,
                "Future payroll entry (ie in a period not yet run) should be unprocessed");
        }

        [Test]
        public void ShouldReturnEmptyListIfPassedEmptyList()
        {
            Assert.AreEqual(_emptyDeclarationPostProcessed.Count, 0);
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
        public void ShouldSetDeclarationBeforeDateAddedToUnprocessed()
        {
            var expectedEntry = _declarationsPostProcessed.First(x => x.Id == "early");
            Assert.AreEqual(expectedEntry.LevyDeclarationPaymentStatus,
                LevyDeclarationPaymentStatus.UnprocessedPayment);
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
            var dateAccountCreated = new DateTime(2016, 01, 01, 00, 00, 00, DateTimeKind.Utc);

            var processor = new PaymentStatusProcessor();
            Assert.Throws<FormatException>(() => processor.ProcessDeclarationPaymentStatuses(brokenDeclarationList, dateAccountCreated, DateTime.Now));
    }

        private static readonly Declaration NoPayrollPeriod = new Declaration
        {
            Id = "noPayrollPeriod",
            SubmissionTime = new DateTime(2016, 8, 19, 00, 00, 00, DateTimeKind.Utc)
        };

        private static readonly Declaration LateEntrySecond = new Declaration
        {
            Id = "LateEntrySecond",
            SubmissionTime = new DateTime(2016, 12, 20, 00, 00, 00, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod {Month = 5, Year = PayrollYear }
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

        private static readonly Declaration Early = new Declaration
        {
            Id = "early",
            SubmissionTime = new DateTime(2016, 8, 19, 00, 00, 00, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 4, Year = PayrollYear },
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
                Early,
                NoPayrollPeriod
            };
        }
    }
}

