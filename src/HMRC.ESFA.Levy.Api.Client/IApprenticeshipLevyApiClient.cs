﻿using System;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Types;
using HMRC.ESFA.Levy.Api.Types.Exceptions;

namespace HMRC.ESFA.Levy.Api.Client
{
    public interface IApprenticeshipLevyApiClient
    {
        /// <summary>
        /// Returns a list of valid links indexed by empref in HAL format
        /// </summary>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        Task<EmprefDiscovery> GetAllEmployers();

        /// <summary>
        /// Returns a list of valid links indexed by empref in HAL format
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        Task<EmprefDiscovery> GetAllEmployers(string authToken);

        /// <summary>
        /// Returns more details about an empref including details about the employer and a list of available endpoints that apply to the empref.
        /// </summary>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        Task<EmpRefLevyInformation> GetEmployerDetails(string empRef);

        /// <summary>
        /// Returns more details about an empref including details about the employer and a list of available endpoints that apply to the empref.
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        Task<EmpRefLevyInformation> GetEmployerDetails(string authToken, string empRef);

        /// <summary>
        /// Returns a list of levy declarations for a given employer reference.
        /// </summary>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        Task<LevyDeclarations> GetEmployerLevyDeclarations(string empRef, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Returns a list of levy declarations for a given employer reference.
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        Task<LevyDeclarations> GetEmployerLevyDeclarations(string authToken, string empRef,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        /// <summary>
        /// Returns a list of fraction calculations for a given employer reference.
        /// </summary>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        Task<EnglishFractionDeclarations> GetEmployerFractionCalculations(string empRef, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Returns a list of fraction calculations for a given employer reference.
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        Task<EnglishFractionDeclarations> GetEmployerFractionCalculations(string authToken, string empRef,
            DateTime? fromDate = null, DateTime? toDate = null);

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
        Task<EmploymentStatus> GetEmploymentStatus(string authToken, string empRef, string nino, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Checks the employment status of an individual in a payroll scheme.
        /// </summary>
        /// <param name="empRef">A valid employer reference for the PAYE scheme.</param>
        /// <param name="nino">A valid National Insurance Number (nino) for the individual being checked.</param>
        /// <param name="fromDate">The date of the earliest calculation to return. Defaults to 72 months prior to current date.</param>
        /// <param name="toDate">The date of the latest calculation to return. Defaults to current date.</param>
        /// <exception cref="ApiHttpException"></exception>
        /// <returns></returns>
        Task<EmploymentStatus> GetEmploymentStatus(string empRef, string nino, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Returns the date of the most recent fraction calculation batch run.
        /// </summary>
        /// <param name="authToken">The access token from SFA.DAS.TokenService.Api.Client</param>
        /// <returns></returns>
        Task<DateTime> GetLastEnglishFractionUpdate(string authToken);
        
        /// <summary>
        /// Returns the date of the most recent fraction calculation batch run.
        /// </summary>
        /// <returns></returns>
        Task<DateTime> GetLastEnglishFractionUpdate();
    }
}