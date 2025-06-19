using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peritos.Common.Api.Paging
{
    /// <summary>
    /// Base class for pagination and sorting parameters used in API requests.
    /// Provides support for page size, page number, sorting field, and sorting direction.
    /// </summary>
    public abstract class ApiPagingParameters 
    {
        /// <summary>
        /// Gets or sets the current page number. Must be between 1 and 100.
        /// </summary>
        [Range(1, 100)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of items per page. Must be between 1 and 100.
        /// </summary>
        [Range(1, 100)]
        public int PageSize { get; set; } = 25;

        /// <summary>
        /// Gets or sets the name of the field to order by.
        /// This value is validated against <see cref="OrderByFieldWhitelist"/>.
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Gets the default ordering direction.
        /// Override this property in derived classes to change the default.
        /// </summary>
        [JsonIgnore]
        protected virtual PagingParametersOrderByDirectionEnum DefaultOrderByDirection => PagingParametersOrderByDirectionEnum.Ascending;

        private PagingParametersOrderByDirectionEnum? _orderByDirection;

        /// <summary>
        /// Gets or sets the sorting direction (ascending or descending).
        /// If not explicitly set, defaults to <see cref="DefaultOrderByDirection"/>.
        /// </summary>
        public PagingParametersOrderByDirectionEnum OrderByDirection
        {
            get => _orderByDirection ?? DefaultOrderByDirection;
            set => _orderByDirection = value;
        }

        /// <summary>
        /// Gets the list of allowed field names that can be used for sorting via the <see cref="OrderBy"/> property.
        /// Must be implemented by derived classes.
        /// </summary>
        [JsonIgnore]
        [OrderByFieldValidator]
        public abstract List<string> OrderByFieldWhitelist { get; }

        /// <summary>
        /// Gets the default field name to use for sorting when <see cref="OrderBy"/> is not provided.
        /// Must be implemented by derived classes.
        /// </summary>
        [JsonIgnore]
        public abstract string DefaultOrderByField { get; }
    }

    /// <summary>
    /// Specifies the direction of sorting for paginated API results.
    /// </summary>
    public enum PagingParametersOrderByDirectionEnum
    {
        Ascending,
        Descending
    }
}
