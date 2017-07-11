using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HMRC.ESFA.Levy.Api.Types
{
    public class FractionCalculation
    {
        /// <summary>
        /// The date that the fractions were calculated.
        /// </summary>
        [JsonProperty("calculatedAt")]
        public DateTime CalculatedAt { get; set; }

        [JsonProperty("fractions")]
        public List<Fraction> Fractions { get; set; }
    }


}