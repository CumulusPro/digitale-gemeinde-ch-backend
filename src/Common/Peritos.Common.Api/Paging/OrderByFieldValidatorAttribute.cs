using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Peritos.Common.Api.Paging
{
    public class OrderByFieldValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var pagingParameters = (ApiPagingParameters)validationContext.ObjectInstance;

            if (!string.IsNullOrWhiteSpace(pagingParameters.OrderBy)
                && !pagingParameters.OrderByFieldWhitelist.Any(x => x.Equals(pagingParameters.OrderBy, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return new ValidationResult($"{pagingParameters.OrderBy} is not a valid sort field");
            }

            return ValidationResult.Success;
        }
    }
}
