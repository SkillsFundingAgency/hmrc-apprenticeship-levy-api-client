using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using HMRC.ESFA.Levy.Api.Client.Services;
using HMRC.ESFA.Levy.Api.Types;
using HMRC.ESFA.Levy.Api.Types.Exceptions;

namespace HMRC.ESFA.Levy.Api.Client
{
    /// <summary>
    /// HMRC Apprenticeship Levy API Client
    /// </summary>
    public class ApprenticeshipLevyApiClient : IApprenticeshipLevyApiClient
    {
        private readonly HttpClient _client;
        private readonly IPaymentStatusProcessor _paymentStatusProcessor;
        private const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// The Authorization Scheme Name
        /// </summary>
        public const string Scheme = "Bearer";

        /// <summary>
        /// Default constuctor
        /// </summary>
        /// <param name="client">A configured HttpClient, alternatively use ApprenticeshipLevyApiClient.CreateHttpClient(token, url)</param>
        public ApprenticeshipLevyApiClient(HttpClient client) : this(client, new PaymentStatusProcessor(new CutoffDatesService()))
        {
        }

        public ApprenticeshipLevyApiClient(HttpClient client, IPaymentStatusProcessor paymentStatusProcessor)
        {
            _client = client;
            _paymentStatusProcessor = paymentStatusProcessor;

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json"));
        }

        /// <summary>
        /// Returns a list of valid links indexed by empref in HAL format
        /// </summary>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        public async Task<EmprefDiscovery> GetAllEmployers()
        {
            return await _client.Get<EmprefDiscovery>("apprenticeship-levy/");
        }

        /// <summary>
        /// Returns a list of valid links indexed by empref in HAL format
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        public async Task<EmprefDiscovery> GetAllEmployers(string authToken)
        {
            AddAuthTokenToClient(authToken, _client);

            return await GetAllEmployers();
        }

        /// <summary>
        /// Returns more details about an empref including details about the employer and a list of available endpoints that apply to the empref.
        /// </summary>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        public async Task<EmpRefLevyInformation> GetEmployerDetails(string empRef)
        {
            var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}";
            return await _client.Get<EmpRefLevyInformation>(url);
        }

        /// <summary>
        /// Returns more details about an empref including details about the employer and a list of available endpoints that apply to the empref.
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        public async Task<EmpRefLevyInformation> GetEmployerDetails(string authToken, string empRef)
        {
            AddAuthTokenToClient(authToken, _client);

            return await GetEmployerDetails(empRef);
        }

        /// <summary>
        /// Returns a list of levy declarations for a given employer reference.
        /// </summary>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        public async Task<LevyDeclarations> GetEmployerLevyDeclarations(string empRef, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return await GetEmployerLevyDeclarationsFromService(empRef, fromDate, toDate);
        }

        /// <summary>
        /// Returns a list of levy declarations for a given employer reference.
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        public async Task<LevyDeclarations> GetEmployerLevyDeclarations(string authToken, string empRef, DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            AddAuthTokenToClient(authToken, _client);

            return await GetEmployerLevyDeclarationsFromService(empRef, fromDate, toDate);
        }

        /// <summary>
        /// Returns a list of levy declarations for a given employer reference.
        /// </summary>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        private async Task<LevyDeclarations> GetEmployerLevyDeclarationsFromService(string empRef, DateTime? fromDate = null, DateTime? toDate = null)
        {

            var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/declarations";
            var parameters = HttpUtility.ParseQueryString(string.Empty);

            if (fromDate.HasValue)
            {
                parameters["fromDate"] = fromDate.Value.ToString(DateFormat);
            }

            if (toDate.HasValue)
            {
                parameters["toDate"] = toDate.Value.ToString(DateFormat);
            }

            if (parameters.AllKeys.Any())
            {
                url += "?" + parameters;
            }

            var levyDeclarations = await _client.Get<LevyDeclarations>(url);
            levyDeclarations.Declarations = _paymentStatusProcessor.ProcessDeclarationsByPayrollPeriod(levyDeclarations.Declarations, DateTime.Now);
            return levyDeclarations;
        }


        /// <summary>
        /// Returns a list of fraction calculations for a given employer reference.
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        public async Task<EnglishFractionDeclarations> GetEmployerFractionCalculations(string authToken, string empRef,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            AddAuthTokenToClient(authToken, _client);

            return await GetEmployerFractionCalculations(empRef, fromDate, toDate);
        }

        /// <summary>
        /// Returns a list of fraction calculations for a given employer reference.
        /// </summary>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        public async Task<EnglishFractionDeclarations> GetEmployerFractionCalculations(string empRef, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/fractions";
            var parameters = HttpUtility.ParseQueryString(string.Empty);

            if (fromDate.HasValue)
            {
                parameters["fromDate"] = fromDate.Value.ToString(DateFormat);
            }

            if (toDate.HasValue)
            {
                parameters["toDate"] = toDate.Value.ToString(DateFormat);
            }

            if (parameters.AllKeys.Any())
            {
                url += "?" + parameters;
            }

            return await _client.Get<EnglishFractionDeclarations>(url);
        }

        /// <summary>
        /// Checks the employment status of an individual in a payroll scheme.
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="nino">A valid National Insurance Number (nino) for the individual being checked.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        public async Task<EmploymentStatus> GetEmploymentStatus(string authToken, string empRef, string nino, DateTime? fromDate = null, DateTime? toDate = null)
        {
            AddAuthTokenToClient(authToken, _client);
            return await GetEmploymentStatus(empRef, nino, fromDate, toDate);
        }

        /// <summary>
        /// Checks the employment status of an individual in a payroll scheme.
        /// </summary>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="nino">A valid National Insurance Number (nino) for the individual being checked.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        public async Task<EmploymentStatus> GetEmploymentStatus(string empRef, string nino, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var url = $"apprenticeship-levy/epaye/{HttpUtility.UrlEncode(empRef)}/employed/{nino}";
            var parameters = HttpUtility.ParseQueryString(string.Empty);

            if (fromDate.HasValue)
            {
                parameters["fromDate"] = fromDate.Value.ToString(DateFormat);
            }

            if (toDate.HasValue)
            {
                parameters["toDate"] = toDate.Value.ToString(DateFormat);
            }

            if (parameters.AllKeys.Any())
            {
                url += "?" + parameters;
            }

            return await _client.Get<EmploymentStatus>(url);

        }

        /// <summary>
        /// Returns the date of the most recent fraction calculation batch run.
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <returns></returns>
        public async Task<DateTime> GetLastEnglishFractionUpdate(string authToken)
        {
            AddAuthTokenToClient(authToken, _client);

            return await GetLastEnglishFractionUpdate();
        }

        /// <summary>
        /// Returns the date of the most recent fraction calculation batch run.
        /// </summary>
        /// <returns></returns>
        public async Task<DateTime> GetLastEnglishFractionUpdate()
        {
            const string url = "apprenticeship-levy/fraction-calculation-date";
            return await _client.Get<DateTime>(url);
        }

        /// <summary>
        /// Returns an HttpClient configured to access the API 
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <param name="baseUrl">The base url of the HMRC API</param>
        /// <returns></returns>
        public static HttpClient CreateHttpClient(string authToken, string baseUrl)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            AddAuthTokenToClient(authToken, client);
            return client;
        }

        private static void AddAuthTokenToClient(string authToken, HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Scheme, authToken);
        }
    }
}