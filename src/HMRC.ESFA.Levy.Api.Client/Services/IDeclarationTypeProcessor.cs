using System.Collections.Generic;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client.Services
{
    public interface IDeclarationTypeProcessor
    {
        List<Declaration> ProcessDeclarationEntryTypes(List<Declaration> declarations);
    }
}