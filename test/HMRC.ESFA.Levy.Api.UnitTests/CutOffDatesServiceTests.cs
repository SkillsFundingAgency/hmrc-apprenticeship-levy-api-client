using HMRC.ESFA.Levy.Api.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Types;
using NUnit.Framework;

namespace HMRC.ESFA.Levy.Api.UnitTests
{
    
    [TestFixture]
    public class CutOffDatesServiceTests
    {
        private CutoffDatesService _cutoffdateservice;

        [SetUp]
        public void Init()
        {
            _cutoffdateservice = new CutoffDatesService();
        }

        [TestCase(1, 5, 2017)]
        [TestCase(2, 6, 2017)]
        [TestCase(3, 7, 2017)]
        [TestCase(4, 8, 2017)]
        [TestCase(5, 9, 2017)]
        [TestCase(6, 10, 2017)]
        [TestCase(7, 11, 2017)]
        [TestCase(8, 12, 2017)]
        [TestCase(9, 1, 2018)]
        [TestCase(10, 2, 2018)]
        [TestCase(11, 3, 2018)]
        [TestCase(12, 4, 2018)]
        public void ShouldReturnCorrectSubmissionCutOffDate(short taxmonth, int expectedmonth, int expectedyear)
        {
            var payrollPeriod = new PayrollPeriod { Year = "17-18", Month = taxmonth };
            var dateforsubmission = _cutoffdateservice.GetDateTimeForSubmissionCutoff(payrollPeriod);
            Assert.AreEqual(new DateTime(expectedyear, expectedmonth, 1).ToString("MM/YYYY"), dateforsubmission.ToString("MM/YYYY"));
        }

        [TestCase(1, 5, 2017)]
        [TestCase(2, 6, 2017)]
        [TestCase(3, 7, 2017)]
        [TestCase(4, 8, 2017)]
        [TestCase(5, 9, 2017)]
        [TestCase(6, 10, 2017)]
        [TestCase(7, 11, 2017)]
        [TestCase(8, 12, 2017)]
        [TestCase(9, 1, 2018)]
        [TestCase(10, 2, 2018)]
        [TestCase(11, 3, 2018)]
        [TestCase(12, 4, 2018)]
        public void ShouldReturnCorrectProcessingCutOffDate(short taxmonth, int expectedmonth, int expectedyear)
        {
            var payrollPeriod = new PayrollPeriod { Year = "17-18", Month = taxmonth };
            var dateforprocessing = _cutoffdateservice.GetDateTimeForProcessingCutoff(payrollPeriod);
            Assert.AreEqual(new DateTime(expectedyear, expectedmonth, 1).ToString("MM/YYYY"), dateforprocessing.ToString("MM/YYYY"));
        }
    }
}
