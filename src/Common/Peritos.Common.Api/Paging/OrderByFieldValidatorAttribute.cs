using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Peritos.Common.Api.Paging
{
    /// <summary>
    /// Validation attribute to ensure that the value of the 'OrderBy' field is within an allowed whitelist of field names.
    /// </summary>
    public class OrderByFieldValidatorAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates that the 'OrderBy' property in the associated <see cref="ApiPagingParameters"/> object
        /// is included in the whitelist of allowed fields.
        /// </summary>
        /// <param name="value">The value to validate (not used directly).</param>
        /// <param name="validationContext">The context in which the validation is performed, providing access to the object instance.</param>
        /// <returns>
        /// A <see cref="ValidationResult"/> indicating success if the 'OrderBy' value is valid or null/empty;
        /// otherwise, an error message indicating the field is not allowed.
        /// </returns>
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
