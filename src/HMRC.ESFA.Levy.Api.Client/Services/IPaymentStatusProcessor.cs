using System;
using System.Collections.Generic;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.Services
{
    internal interface IPaymentStatusProcessor
    {
        List<Declaration> ProcessDeclarationsByPayrollPeriod(List<Declaration> declarations, DateTime dateTimeProcessingInvoked);
    }
}