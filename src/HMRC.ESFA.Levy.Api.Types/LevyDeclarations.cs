using System.Collections.Generic;

namespace HMRC.ESFA.Levy.Api.Types
{
    public class LevyDeclarations
    {
        /// <summary>
        /// The PAYE Reference for the employer. This will be the same as provided in the URL.
        /// </summary>
        public string EmpRef { get; set; }

        public List<Declaration> Declarations { get; set; }
    }
}