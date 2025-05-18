using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Peritos.Common.Api.ApplicationInsights;
using Peritos.Common.Testing;
using System;

namespace Peritos.Common.Api.Tests.ApplicationInsights
{
    [TestClass]
    public class CorrelationService_Tests : TestingContext<CorrelationService>
    {
         private HttpContext _httpContext;

        [TestInitialize]
        public void Setup()
        {
            _httpContext = new DefaultHttpContext();
            GetMockFor<IHttpContextAccessor>()
                .SetupGet(x => x.HttpContext)
                .Returns(_httpContext);
        }

        [TestMethod]
        public void WhenGetCorrelationIdCalled_AndHttpContextIsNull_EmptyStringReturned()
        {
            GetMockFor<IHttpContextAccessor>()
                .SetupGet(x => x.HttpContext)
                .Returns(() => null);

            var result = ClassUnderTest.CorrelationId;

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void WhenGetCorrelationIdCalled_AndHeaderContainsCorrelationId_HeaderCorrelationIdReturned()
        {
            string headerCorrelationId = Guid.NewGuid().ToString();
            _httpContext.Request.Headers["x-correlation-id"] = headerCorrelationId;
            var result = ClassUnderTest.CorrelationId;
            Assert.AreEqual(headerCorrelationId, result);
        }
    }
}
