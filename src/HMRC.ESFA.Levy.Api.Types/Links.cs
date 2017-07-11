using Newtonsoft.Json;

namespace HMRC.ESFA.Levy.Api.Types
{
    public class Links
    {
        public Link Self { get; set; }
        public Link Declarations { get; set; }
        public Link Fractions { get; set; }
    }
}