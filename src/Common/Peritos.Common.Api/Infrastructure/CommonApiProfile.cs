using AutoMapper;

namespace Peritos.Common.Api.Infrastructure
{
    /// <summary>
    /// AutoMapper profile for mapping common API paging parameters to internal paging parameters.
    /// </summary>
    public class CommonApiProfile : Profile
    {
        public CommonApiProfile()
        {
            CreateMap<Paging.ApiPagingParameters, Common.Abstractions.Paging.PagingParameters>()
                .ForMember(x => x.OrderBy, opts => opts.MapFrom<OrderByValueResolver>()) // Custom mapping for the orderby expression
                .IncludeAllDerived(); // Allows all inherited DTOs to also use this mapping by default. 
        }

        /// <summary>
        /// Resolves the value for the OrderBy property during mapping from API paging parameters
        /// to internal paging parameters. Falls back to <c>DefaultOrderByField</c> if no value is provided.
        /// </summary>
        public class OrderByValueResolver : IValueResolver<Paging.ApiPagingParameters, Common.Abstractions.Paging.PagingParameters, string>
        {
            /// <summary>
            /// Resolves the value for the destination OrderBy property.
            /// </summary>
            /// <param name="source">The source <see cref="Paging.ApiPagingParameters"/> object.</param>
            /// <param name="destination">The destination <see cref="Common.Abstractions.Paging.PagingParameters"/> object.</param>
            /// <param name="destMember">The existing destination member value (not used).</param>
            /// <param name="context">The mapping context.</param>
            /// <returns>The specified <c>OrderBy</c> value or the default if null or whitespace.</returns>
            public string Resolve(Paging.ApiPagingParameters source, Abstractions.Paging.PagingParameters destination, string destMember, ResolutionContext context)
            {
                return !string.IsNullOrWhiteSpace(source.OrderBy) ? source.OrderBy : source.DefaultOrderByField;
            }
        }
    }
}
