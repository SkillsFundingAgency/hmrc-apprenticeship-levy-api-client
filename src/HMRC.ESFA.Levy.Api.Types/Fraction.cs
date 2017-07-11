using Newtonsoft.Json;

namespace HMRC.ESFA.Levy.Api.Types
{
    public class Fraction
    {
        /// <summary>
        /// The region the specific fraction applies to. Will always be England for the forseeable future.
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        /// The fraction calculated for the region. Will be a decimal in the range 0 to 1.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}