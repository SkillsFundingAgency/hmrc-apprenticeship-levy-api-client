using Newtonsoft.Json;

namespace HMRC.ESFA.Levy.Api.Types
{
    public class Name
    {
        /// <summary>
        /// The first line of the employer's name
        /// </summary>
        [JsonProperty("nameLine1")]
        public string EmprefAssociatedName { get; set; }
    }
}