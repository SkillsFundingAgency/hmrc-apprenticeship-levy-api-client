using System;

namespace HMRC.ESFA.Levy.Api.Client
{
    internal class SignificantProcessingDates
    {
        public DateTime DateAdded { get; set; }
        public DateTime DateProcessorInvoked { get; set; }
        public DateTime DateOfCutoffForProcessing { get; set; }
        public DateTime DateOfCutoffForSubmission { get; set; }
    }
}
