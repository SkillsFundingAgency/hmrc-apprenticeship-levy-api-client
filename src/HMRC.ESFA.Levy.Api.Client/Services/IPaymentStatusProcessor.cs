using System;
using System.Collections.Generic;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.Services
{
    public interface IPaymentStatusProcessor
    {
        List<Declaration> ProcessDeclarationPaymentStatuses(List<Declaration> declarations, DateTime dateAdded, DateTime currentDateTime);
    }
}