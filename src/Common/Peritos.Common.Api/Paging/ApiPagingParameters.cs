using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peritos.Common.Api.Paging
{
    public abstract class ApiPagingParameters 
    {
        [Range(1, 100)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 25;


        public string OrderBy { get; set; }

        [JsonIgnore]
        protected virtual PagingParametersOrderByDirectionEnum DefaultOrderByDirection => PagingParametersOrderByDirectionEnum.Ascending;

        private PagingParametersOrderByDirectionEnum? _orderByDirection;
        public PagingParametersOrderByDirectionEnum OrderByDirection
        {
            get => _orderByDirection ?? DefaultOrderByDirection;
            set => _orderByDirection = value;
        }

        [JsonIgnore]
        [OrderByFieldValidator]
        public abstract List<string> OrderByFieldWhitelist { get; }

        [JsonIgnore]
        public abstract string DefaultOrderByField { get; }
    }

    public enum PagingParametersOrderByDirectionEnum
    {
        Ascending,
        Descending
    }
}
