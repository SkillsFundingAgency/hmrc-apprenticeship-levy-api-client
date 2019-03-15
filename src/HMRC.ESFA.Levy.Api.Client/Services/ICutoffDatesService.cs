using System;
using HMRC.ESFA.Levy.Api.Types;

public interface ICutoffDatesService
{
    DateTime GetDateTimeForSubmissionCutoff(PayrollPeriod payrollPeriod);
    DateTime GetDateTimeForProcessingCutoff(PayrollPeriod payrollPeriod);
}