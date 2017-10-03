using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Client.Services;
using HMRC.ESFA.Levy.Api.Types;
using NUnit.Framework;

namespace HMRC.ESFA.Levy.Api.UnitTests
{
    [TestFixture]
    public class PaymentStatusTest
    {
        private PaymentStatusProcessor _processor;

        [SetUp]
        public void Init()
        {
            _processor = new PaymentStatusProcessor(new CutoffDatesService());
        }

        [TestCase(20, 9, 2017)]
        [TestCase(24, 9, 2017)]
        [TestCase(20, 10, 2017)]
        [TestCase(24, 10, 2017)]
        public void ShouldSetCorrectPaymentStatus(int day, int month, int year)
        {
            var today = new DateTime(year, month, day);
            var declarations = new List<Declaration>();
            declarations.Add(new Declaration
            {
                Id = "lateentry",
                //SubmissionTime = new DateTime(2017, 9, 19, 00, 00, 00, DateTimeKind.Utc),
                SubmissionTime = new DateTime(today.Year, today.Month - 1, 19, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = GetPayrollMonth(today.Month - 3), Year = GetPayollYear(today) }
            });
            declarations.Add(new Declaration
            {
                Id = "latestentry",
                //SubmissionTime = new DateTime(2017, 9, 18, 00, 00, 00, DateTimeKind.Utc),
                SubmissionTime = new DateTime(today.Year, today.Month - 1, 18, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = GetPayrollMonth(today.Month - 2), Year = GetPayollYear(today) }
            });
            declarations.Add(new Declaration
            {
                Id = "anotherlatestentry",
                SubmissionTime = new DateTime(today.Year, today.Month - 1, 19, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = GetPayrollMonth(today.Month - 2), Year = GetPayollYear(today) }
            });
            declarations.Add(new Declaration
            {
                Id = "futurepayrollentry",
                SubmissionTime = new DateTime(today.Year, today.Month, 19, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = GetPayrollMonth(today.Month), Year = GetPayollYear(today) }
            });
            declarations.Add(new Declaration
            {
                Id = "anotherfuturepayrollentry",
                SubmissionTime = new DateTime(today.Year, today.Month - 1, 19, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = GetPayrollMonth(today.Month + 1), Year = GetPayollYear(today) }
            });

            var declarationsPostProcessed = _processor.ProcessDeclarationsByPayrollPeriod(declarations, today);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(LevyDeclarationPaymentStatus.LatePayment, declarationsPostProcessed.Single(x => x.Id == "lateentry").LevyDeclarationPaymentStatus, $"Check declaration with id lateentry");
                Assert.AreEqual(LevyDeclarationPaymentStatus.UnprocessedPayment, declarationsPostProcessed.Single(x => x.Id == "latestentry").LevyDeclarationPaymentStatus, $"Check declaration with id latestentry");
                Assert.AreEqual(LevyDeclarationPaymentStatus.LatestPayment, declarationsPostProcessed.Single(x => x.Id == "anotherlatestentry").LevyDeclarationPaymentStatus, $"Check declaration with id anotherlatestentry");
                Assert.AreEqual(LevyDeclarationPaymentStatus.UnprocessedPayment, declarationsPostProcessed.Single(x => x.Id == "futurepayrollentry").LevyDeclarationPaymentStatus, $"Check declaration with id futurepayrollentry");
                Assert.AreEqual(LevyDeclarationPaymentStatus.UnprocessedPayment, declarationsPostProcessed.Single(x => x.Id == "anotherfuturepayrollentry").LevyDeclarationPaymentStatus, $"Check declaration with id anotherfuturepayrollentry");
            });
        }

        
        [TestCase(24, 9, 2017)]
        [TestCase(24, 10, 2017)]
        public void ShouldHaveZeroOrOneLatestPaymentPerMonth(int day, int month, int year)
        {
            var today = new DateTime(year, month, day);
            var declarations = new List<Declaration>();
            declarations.Add(new Declaration
            {
                Id = "month7latestentry",
                SubmissionTime = new DateTime(today.Year, today.Month - 1, 18, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = GetPayrollMonth(today.Month - 2), Year = GetPayollYear(today) }
            });
            declarations.Add(new Declaration
            {
                Id = "month6latestentry",
                SubmissionTime = new DateTime(today.Year, today.Month - 2, 18, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = GetPayrollMonth(today.Month - 3), Year = GetPayollYear(today) }
            });
            declarations.Add(new Declaration
            {
                Id = "month8latestentry",
                SubmissionTime = new DateTime(today.Year, today.Month - 4, 24, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = GetPayrollMonth(today.Month - 1), Year = GetPayollYear(today) }
            });
            declarations.Add(new Declaration
            {
                Id = "month4latestentry",
                SubmissionTime = new DateTime(today.Year, today.Month - 4, 18, 00, 00, 00, DateTimeKind.Utc),
                PayrollPeriod = new PayrollPeriod { Month = GetPayrollMonth(today.Month - 5), Year = GetPayollYear(today) }
            });
            var declarationsPostProcessed = _processor.ProcessDeclarationsByPayrollPeriod(declarations, today);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(LevyDeclarationPaymentStatus.LatestPayment, declarationsPostProcessed.Single(x => x.Id == "month7latestentry").LevyDeclarationPaymentStatus, $"Check declaration with id month7latestentry");
                Assert.AreEqual(LevyDeclarationPaymentStatus.LatestPayment, declarationsPostProcessed.Single(x => x.Id == "month6latestentry").LevyDeclarationPaymentStatus, $"Check declaration with id month6latestentry");
                Assert.AreEqual(LevyDeclarationPaymentStatus.LatestPayment, declarationsPostProcessed.Single(x => x.Id == "month8latestentry").LevyDeclarationPaymentStatus, $"Check declaration with id month8latestentry");
                Assert.AreEqual(LevyDeclarationPaymentStatus.LatestPayment, declarationsPostProcessed.Single(x => x.Id == "month4latestentry").LevyDeclarationPaymentStatus, $"Check declaration with id month4latestentry");
            });
        }


        public short GetPayrollMonth(int month)
        {
            var taxmonth = month - 3;
            return (short)(taxmonth <= 0 ? month + 9 : taxmonth);
        }
        public string GetPayollYear(DateTime date)
        {
            return date.Month >= 4 ? $"{date.ToString("yy")}-{(date.AddYears(1)).ToString("yy")}" : $"{(date.AddYears(-1)).ToString("yy")}-{date.ToString("yy")}";
        }

        [TestCase(4, 1)]
        [TestCase(5, 2)]
        [TestCase(6, 3)]
        [TestCase(7, 4)]
        [TestCase(8, 5)]
        [TestCase(9, 6)]
        [TestCase(10, 7)]
        [TestCase(11, 8)]
        [TestCase(12, 9)]
        [TestCase(1, 10)]
        [TestCase(2, 11)]
        [TestCase(3, 12)]
        public void ShouldReturnCorrectPayrollMonth(int month, short taxmonth)
        {
            var result = GetPayrollMonth(month);
            Assert.AreEqual(taxmonth, result, $"failed for month {month}");
        }

        [TestCase(12, 2016, "16-17")]
        [TestCase(1, 2017,"16-17")]
        [TestCase(4, 2017, "17-18")]
        [TestCase(10, 2017, "17-18")]
        [TestCase(2, 2018, "17-18")]
        [TestCase(4, 2018, "18-19")]
        public void ShouldReturnCorrectPayrollYear(int month, int year, string payrollyear)
        {
            var date = new DateTime(year, month, 1);
            var result = GetPayollYear(date);
            Assert.AreEqual(payrollyear, result, $"failed for {date.ToString("dd/MM/yyyy")}");
        }
    }
}
