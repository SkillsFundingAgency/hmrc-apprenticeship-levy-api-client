using Newtonsoft.Json;

namespace HMRC.ESFA.Levy.Api.Types
{
    public class EmpRefLevyInformation
    {
        [JsonProperty("_links")]
        public Links Links { get; set; }
        public Employer Employer { get; set; }
    }
}