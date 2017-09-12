using System;
using Newtonsoft.Json;

namespace HMRC.ESFA.Levy.Api.Types
{
    public class Declaration
    {
        /// <summary>
        /// A unique identifier for the declaration. This will remain consistent from one call to the API to the next so that the client can identify declarations they’ve already retrieved. It is the identifier assigned by the RTI system to the EPS return, so it is possible to cross-reference with HMRC if needed. Dividing this identifier by 10 (ignoring the remainder) gives the identifier assigned by the RTI system to the EPS return, so it is possible to cross-reference with HMRC if needed. Taking this identifier modulo 10 gives the type of entry: 0, no entry; 1, inactive; 2, levy declaration; 3, ceased.
        /// </summary>
        public string Id { get; set; }

        public long SubmissionId { get; set; }

        /// <summary>
        /// If present, indicates the date that the payroll scheme was ceased.
        /// </summary>
        public DateTime? DateCeased { get; set; }

        /// <summary>
        /// Indicates the the payroll scheme will be inactive starting from this date. Should always be the 6th of the month of the first inactive payroll period.
        /// </summary>
        public DateTime? InactiveFrom { get; set; }

        /// <summary>
        /// The date after which the payroll scheme will be active again. Should always be the 5th of the month of the last inactive payroll period.
        /// </summary>
        public DateTime? InactiveTo { get; set; }

        /// <summary>
        /// If present, will always have the value true and indicates that no declaration was necessary for this period. This can be interpreted to mean that the YTD levy balance is unchanged from the previous submitted value.
        /// </summary>
        public bool NoPaymentForPeriod { get; set; }

        /// <summary>
        /// The time at which the EPS submission that this declaration relates to was received by HMRC. If the backend systems return a bad date that can not be handled this will be set to 1970-01-01T01:00:00.000.
        /// </summary>
        public DateTime SubmissionTime { get; set; }

        public PayrollPeriod PayrollPeriod { get; set; }

        /// <summary>
        /// The amount of apprenticeship levy that was declared in the payroll month.
        /// </summary>
        [JsonProperty("levyDueYTD")]
        public decimal LevyDueYearToDate { get; set; }

        /// <summary>
        /// The annual amount of apprenticeship levy allowance that has been allocated to this payroll scheme. If absent then the value can be taken as 0. The maximum value in the 2017/18 will be 15,000.
        /// </summary>
        public decimal LevyAllowanceForFullYear { get; set; }

        /// <summary>
        /// Each LevyDeclaration is either standard, LastPaymentBeforeCutoff, or Late (after Cutoff)
        /// </summary>
        public LevyDeclarationType LevyDeclarationType { get; set; }
    }
}