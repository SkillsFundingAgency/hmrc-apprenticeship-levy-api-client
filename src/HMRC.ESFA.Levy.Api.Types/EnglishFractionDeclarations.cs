using System.Collections.Generic;
using Newtonsoft.Json;

namespace HMRC.ESFA.Levy.Api.Types
{
    public class EnglishFractionDeclarations
    {
        [JsonProperty("empref")]
        public string Empref { get; set; }

        [JsonProperty("fractionCalculations")]
        public List<FractionCalculation> FractionCalculations { get; set; }
    }
}