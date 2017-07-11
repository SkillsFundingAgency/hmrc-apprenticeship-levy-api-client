namespace HMRC.ESFA.Levy.Api.Types
{
    public class PayrollPeriod
    {
        /// <summary>
        /// The tax year of the payroll period against which the declaration was made.
        /// </summary>
        /// <example>15-16</example>
        public string Year { get; set; }

        /// <summary>
        /// The tax month of the payroll period against which the declaration was made. Month 1 is April.
        /// </summary>
        public short Month { get; set; }
    }
}