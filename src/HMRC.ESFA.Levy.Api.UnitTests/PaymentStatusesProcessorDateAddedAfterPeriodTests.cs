using System;
using System.Collections.Generic;
using System.Linq;
using HMRC.ESFA.Levy.Api.Client.Services;
using HMRC.ESFA.Levy.Api.Types;
using NUnit.Framework;

namespace HMRC.ESFA.Levy.Api.UnitTests
{
    [TestFixture]
    public class PaymentStatusesProcessorDateAddedAfterPeriodTests
    {
        private PaymentStatusProcessor _processor;
        private List<Declaration> _declarations;
        private List<Declaration> _declarationsAfterAccountCreatedDatePostProcessed;
        private const string PayrollYear = "17-18";
        private int _countOfDeclarationsPreProcessing;
        private int _countOfUnprocessedAfterProcessing;
        
        [SetUp]
        public void Init()
        {
            _processor = new PaymentStatusProcessor();

     
            _declarations = GetDeclarationList();
            _countOfDeclarationsPreProcessing = _declarations.Count;


            var dateAccountCreatedAfterCutoff = new DateTime(2017, 09, 23, 00, 00, 00, DateTimeKind.Utc);

            _declarationsAfterAccountCreatedDatePostProcessed = _processor
                .ProcessDeclarationPaymentStatuses(_declarations, dateAccountCreatedAfterCutoff);

            _countOfUnprocessedAfterProcessing =
                _declarationsAfterAccountCreatedDatePostProcessed.Count(
                    x => x.LevyDeclarationPaymentStatus == LevyDeclarationPaymentStatus.UnprocessedPayment);
        }
        
        [Test]
        public void ShouldCheckAllEntriesRemainUnprocessed()
        {
            Assert.AreEqual(_countOfDeclarationsPreProcessing, _countOfUnprocessedAfterProcessing);
        }
        
        private static readonly Declaration NoPayrollPeriod = new Declaration
        {
            Id = "noPayrollPeriod",
            SubmissionTime = new DateTime(2017, 8, 19, 00, 00, 00, DateTimeKind.Utc)
        };

        private static readonly Declaration LateEntrySecond = new Declaration
        {
            Id = "LateEntrySecond",
            SubmissionTime = new DateTime(2017, 12, 20, 00, 00, 00, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 5, Year = PayrollYear }
        };

        private static readonly Declaration UnprocessedAndVeryEarly = new Declaration
        {
            Id = "unprocessedAndVeryEarly",
            SubmissionTime = new DateTime(2017, 1, 20, 00, 00, 00, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 5, Year = PayrollYear }
        };

        private static readonly Declaration LastBeforeCutoff = new Declaration
        {
            Id = "LastBeforeCutoff",
            SubmissionTime = new DateTime(2017, 09, 19, 23, 59, 59, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 5, Year = PayrollYear }
        };

        private static readonly Declaration SecondLastBeforeCutoff = new Declaration
        {
            Id = "secondLastBeforeCutoff",
            SubmissionTime = new DateTime(2017, 09, 19, 10, 15, 00, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 5, Year = PayrollYear },
        };
        private static readonly Declaration LateEntryFirst = new Declaration
        {
            Id = "LateEntryFirst",
            SubmissionTime = new DateTime(2017, 09, 20, 0, 0, 0, DateTimeKind.Utc),
            PayrollPeriod = new PayrollPeriod { Month = 5, Year = PayrollYear },
        };

        private static readonly Declaration Early = new Declaration
        {
            Id = "early",
            SubmissionTime = new DateTime(2017, 8, 19, 00, 00, 00, DateTimeKind.Utc),
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

