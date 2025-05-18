using AutoMapper;

namespace Peritos.Common.Api.Infrastructure
{
    public class CommonApiProfile : Profile
    {
        public CommonApiProfile()
        {
            CreateMap<Paging.ApiPagingParameters, Common.Abstractions.Paging.PagingParameters>()
                .ForMember(x => x.OrderBy, opts => opts.MapFrom<OrderByValueResolver>()) // Custom mapping for the orderby expression
                .IncludeAllDerived(); // Allows all inherited DTOs to also use this mapping by default. 
        }

        public class OrderByValueResolver : IValueResolver<Paging.ApiPagingParameters, Common.Abstractions.Paging.PagingParameters, string>
        {
            public string Resolve(Paging.ApiPagingParameters source, Abstractions.Paging.PagingParameters destination, string destMember, ResolutionContext context)
            {
                return !string.IsNullOrWhiteSpace(source.OrderBy) ? source.OrderBy : source.DefaultOrderByField;
            }
        }
    }
}
