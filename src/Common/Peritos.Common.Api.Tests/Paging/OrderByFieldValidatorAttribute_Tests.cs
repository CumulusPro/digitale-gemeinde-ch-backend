using Microsoft.VisualStudio.TestTools.UnitTesting;
using Peritos.Common.Api.Paging;
using Peritos.Common.Testing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peritos.Common.Api.Tests.Paging
{
    [TestClass]
    public class OrderByFieldValidatorAttribute_Tests : TestingContext<OrderByFieldValidatorAttribute>
    {
        [TestMethod]
        public void WhenIsValidCalled_AndOrderByFieldIsNotNull_AndFieldNotInWhiteList_ErrorMessageReturned()
        {
            var pagingParameters = new TestPagingParameters { OrderBy = "NotValid" };
            var result = ClassUnderTest.GetValidationResult(pagingParameters, new ValidationContext(pagingParameters));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
        }

        [TestMethod]
        public void WhenIsValidCalled_AndOrderByFieldIsNotNull_AndFieldIsInWhiteList_ValidationResultIsSuccess()
        {
            var pagingParameters = new TestPagingParameters { OrderBy = "Valid" };
            var result = ClassUnderTest.GetValidationResult(pagingParameters, new ValidationContext(pagingParameters));
            Assert.AreEqual(result, ValidationResult.Success);
        }

        private class TestPagingParameters : ApiPagingParameters
        {
            public override List<string> OrderByFieldWhitelist => new List<string> { "Valid" };

            public override string DefaultOrderByField => string.Empty;
        }
    }
}
