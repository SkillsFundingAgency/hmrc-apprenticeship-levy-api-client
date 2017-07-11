using System;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Types;

namespace HMRC.ESFA.Levy.Api.Client
{
    public interface IApprenticeshipLevyApiClient
    {
        Task<EmprefDiscovery> GetAllEmployers();
        Task<EmpRefLevyInformation> GetEmployerDetails(string empRef);
        Task<EnglishFractionDeclarations> GetEmployerFractionCalculations(string empRef, DateTime? fromDate =  null, DateTime? toDate =  null);
        Task<LevyDeclarations> GetEmployerLevyDeclarations(string empRef, DateTime? fromDate = null, DateTime? toDate = null);
        Task<EmploymentStatus> GetEmploymentStatus(string empRef, string nino, DateTime? fromDate =  null, DateTime? toDate =  null);
        Task<DateTime> GetLastEnglishFractionUpdate();
    }
}