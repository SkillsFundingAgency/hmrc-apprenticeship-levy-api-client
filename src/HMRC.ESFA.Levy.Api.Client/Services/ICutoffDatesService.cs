using System;
using HMRC.ESFA.Levy.Api.Types;

internal interface ICutoffDatesService
{
    DateTime GetDateTimeForSubmissionCutoff(PayrollPeriod payrollPeriod);
    DateTime GetDateTimeForProcessingCutoff(PayrollPeriod payrollPeriod);
}