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
            LevyDeclarations expected = new LevyDeclarations
            {
                EmpRef = "000/AA00000",
                Declarations = expectedDeclarations
            };
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localhost/apprenticeship-levy/epaye/{HttpUtility.UrlEncode(expected.EmpRef)}/declarations")
                .Respond("application/json", JsonConvert.SerializeObject(expected));

            var mockDeclarationTypeProcessor = new Mock<IDeclarationTypeProcessor>();
            mockDeclarationTypeProcessor.Setup(x => x.ProcessDeclarationEntryTypes(It.IsAny<List<Declaration>>()))
                .Returns(expectedDeclarations);

            HttpClient httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/");
            ApprenticeshipLevyApiClient client = new ApprenticeshipLevyApiClient(httpClient, mockDeclarationTypeProcessor.Object);

            // Act
            LevyDeclarations declarations = await client.GetEmployerLevyDeclarations(expected.EmpRef);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            Assert.AreEqual(expected.EmpRef, declarations.EmpRef);
            Assert.AreEqual(expected.Declarations.Count, declarations.Declarations.Count);
        }

        [Test]
        public async Task ShouldThrowExceptionIfEmployerNotFound()
        {
            // Arrange
            LevyDeclarations expected = new LevyDeclarations
            {
                EmpRef = "000/AA00000"
            };
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localhost/apprenticeship-levy/epaye/{HttpUtility.UrlEncode(expected.EmpRef)}/declarations?fromDate=2017-04-01")
                .Respond(HttpStatusCode.NotFound, x => new StringContent(""));

            HttpClient httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/");

            ApprenticeshipLevyApiClient client = new ApprenticeshipLevyApiClient(httpClient, null);

            // Act
            //var declarations = await client.GetLevyDeclarations(expected.EmpRef);
            ApiHttpException ex = Assert.ThrowsAsync<ApiHttpException>(() => client.GetEmployerLevyDeclarations(expected.EmpRef));

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            Assert.AreEqual(404, ex.HttpCode);
        }
    }
}
