using System;

namespace HMRC.ESFA.Levy.Api.Types
{
    public class EmploymentStatus
    {
        /// <summary>
        /// The PAYE Reference for the employer. This will be the same as provided in the URL.
        /// </summary>
        public string Empref { get; set; }
        /// <summary>
        /// The NINO of the individual being checked. This will be the same as provided in the URL.
        /// </summary>
        public string Nino { get; set; }
        /// <summary>
        /// The start date of the range the check should be made for.
        /// </summary>
        public DateTime FromDate { get; set; }
        /// <summary>
        /// The end date of the range the check should be made for.
        /// </summary>
        public DateTime ToDate { get; set; }
        /// <summary>
        /// Whether or not the individual was employed in the scheme at any time with the date range.
        /// </summary>
        public bool Employed { get; set; }
    }
}