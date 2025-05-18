using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace Peritos.Common.Testing.Web
{
    public class FilterTestingContext<T> : TestingContext<T> where T : class
    {
        protected ActionExecutingContext _actionExecutingContext;
        protected Mock<ActionExecutionDelegate> _mockActionExecutingDelegate;

        [TestInitialize]
        public void SetupFilterContext()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockRouteData = new Mock<RouteData>();
            var mockActionDescriptor = new Mock<ActionDescriptor>();
            _actionExecutingContext = new ActionExecutingContext(new ActionContext(mockHttpContext.Object, mockRouteData.Object, mockActionDescriptor.Object),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                null);


            _mockActionExecutingDelegate = new Mock<ActionExecutionDelegate>();
        }
    }
}
