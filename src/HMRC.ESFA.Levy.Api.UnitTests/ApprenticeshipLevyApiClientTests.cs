using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using HMRC.ESFA.Levy.Api.Client;
using HMRC.ESFA.Levy.Api.Client.Services;
using HMRC.ESFA.Levy.Api.Types;
using HMRC.ESFA.Levy.Api.Types.Exceptions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace HMRC.ESFA.Levy.Api.UnitTests
{
    [TestFixture]
    public class ApprenticeshipLevyApiClientTests
    {
       
        [Test]
        public async Task ShouldGetLevyDeclarations()
        {
            // Arrange
            var expectedDeclarations = new List<Declaration>
            {
                new Declaration()
            };
            var expected = new LevyDeclarations
            {
                EmpRef = "000/AA00000",
                Declarations = expectedDeclarations
            };
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localhost/apprenticeship-levy/epaye/{HttpUtility.UrlEncode(expected.EmpRef)}/declarations")
                .Respond("application/json", JsonConvert.SerializeObject(expected));

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/");
            var client = new ApprenticeshipLevyApiClient(httpClient);

            // Act
            var declarations = await client.GetEmployerLevyDeclarations(expected.EmpRef);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            Assert.AreEqual(expected.EmpRef, declarations.EmpRef);
            Assert.AreEqual(expected.Declarations.Count, declarations.Declarations.Count);
        }

        [Test]
        public async Task ShouldThrowExceptionIfEmployerNotFound()
        {
            // Arrange
            var expected = new LevyDeclarations
            {
                EmpRef = "000/AA00000"
            };
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localhost/apprenticeship-levy/epaye/{HttpUtility.UrlEncode(expected.EmpRef)}/declarations?fromDate=2017-04-01")
                .Respond(HttpStatusCode.NotFound, x => new StringContent(""));

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/");

            var client = new ApprenticeshipLevyApiClient(httpClient);

            // Act
            var ex = Assert.ThrowsAsync<ApiHttpException>(() => client.GetEmployerLevyDeclarations(expected.EmpRef));

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            Assert.AreEqual(404, ex.HttpCode);
        }
        

        [Test]
        public async Task ShouldGetLevyDeclarationsWithDeclarationTypes()
        {
            // Arrange
            var expectedDeclarations = new List<Declaration>
            {
                new Declaration()
            };
            var expected = new LevyDeclarations
            {
                EmpRef = "000/AA00000",
                Declarations = expectedDeclarations
            };
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localhost/apprenticeship-levy/epaye/{HttpUtility.UrlEncode(expected.EmpRef)}/declarations")
                .Respond("application/json", JsonConvert.SerializeObject(expected));

            var mockDeclarationTypeProcessor = new Mock<IPaymentStatusProcessor>();
            mockDeclarationTypeProcessor.Setup(x => x.ProcessDeclarationPaymentStatuses(It.IsAny<List<Declaration>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(expectedDeclarations);

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/");
            var client = new ApprenticeshipLevyApiClient(httpClient, mockDeclarationTypeProcessor.Object);

            // Act
            var declarations = await client.GetEmployerLevyDeclarationsWithPaymentStatuses(expected.EmpRef, new DateTime());

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            mockHttp.VerifyNoOutstandingRequest();
            Assert.AreEqual(expected.EmpRef, declarations.EmpRef);
            Assert.AreEqual(expectedDeclarations.Count, declarations.Declarations.Count);
        }
    }
}
