using System;

namespace HMRC.ESFA.Levy.Api.Client
{
    internal class SignificantProcessingDates
    { 
        public DateTime DateOfCutoffForProcessing { get; set; }
        public DateTime DateOfCutoffForSubmission { get; set; }
    }
}
