using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Peritos.Common.Testing.Web
{
    public class ControllerTestingContext<T> : TestingContext<T> where T : class
    {
        [TestInitialize]
        public void SetupControllerTestingContext()
        {
            Fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
        }
    }
}
