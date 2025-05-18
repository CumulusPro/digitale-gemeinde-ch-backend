using AutoMapper;
using Cpro.Forms.Data.Models.Tenant;
using Cpro.Forms.Data.Models.User;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Models.Tenant;
using Cpro.Forms.Service.Models.User;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Infrastructure
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<SearchRequest, Data.Models.SearchRequest>()
                .ReverseMap();
            CreateMap<FormSearchRequest, Data.Models.FormSearchRequest>()
                .ReverseMap();
            CreateMap<FormTemplate, Data.Models.FormTemplate>();
            CreateMap<Data.Models.FormTemplate, FormTemplate>()
                .ForMember(x => x.id, opt => opt.MapFrom(y => y.Id));

            CreateMap<FormData, Data.Models.FormData>();
            CreateMap<Data.Models.FormData, FormData>()
                .ForMember(x => x.id, opt => opt.MapFrom(y => y.Id));

            CreateMap<CreateFormTemplateRequest, Data.Models.FormTemplate>();
            
                
            CreateMap<PagingResponse<FormTemplate>, PagingResponse<Data.Models.FormTemplate>>()
                .ReverseMap();

            CreateMap<FormDesign, Data.Models.FormDesign>()
                .ForMember(x => x.Tags, opt => opt.MapFrom(y => Helper.Join(y.tags, '|')));

            CreateMap<Data.Models.FormDesign, FormDesign>()
                .ForMember(x => x.tags, opt => opt.MapFrom(y => Helper.Split(y.Tags, '|')))
                .ForMember(x => x.id, opt => opt.MapFrom(y => y.Id));

            CreateMap<PagingResponse<FormDesign>, PagingResponse<Data.Models.FormDesign>>()
                .ReverseMap();
            CreateMap<PagingResponse<FormData>, PagingResponse<Data.Models.FormData>>()
                .ReverseMap();

            CreateMap<Data.Models.Designer, Designer>();
            CreateMap<Data.Models.Processor, Processor>();
            CreateMap<Data.Models.FormStatesConfig, FormStatesConfig>();

            CreateMap<Data.Models.FormDesignHistory, FormDesign>();
            CreateMap<Data.Models.ProcessorHistory, Processor>();
            CreateMap<Data.Models.DesignerHistory, Designer>();
            CreateMap<Data.Models.FormStatesConfigHistory, FormStatesConfig>();

            CreateMap<User, UserResponse?>();
            CreateMap<UserRequest, User>();

            CreateMap<Models.User.UserSearchRequest, Data.Models.User.UserSearchRequest>()
                .ReverseMap();
            CreateMap<PagingResponse<User>, PagingResponse<UserResponse>>()
                .ReverseMap();

            CreateMap<Tenant, Models.Tenant.TenantResponse?>().ReverseMap();
            CreateMap<TenantRequest, Tenant>();
            CreateMap<Models.Tenant.TenantSearchRequest, Data.Models.Tenant.TenantSearchRequest>()
                .ReverseMap();
            CreateMap<PagingResponse<Tenant>, PagingResponse<Models.Tenant.TenantResponse>>()
                .ReverseMap();

            CreateMap<Integration.Straatos.Models.DocumentResponse, DocumentResponse>()
                .ReverseMap();
        }
    }

    public class Helper
    {
        public static List<string> Split(string input, char separator)
        {
            return input.Split(separator).ToList();
        }
        public static string Join(List<string> input, char separator)
        {
            return string.Join(separator, input);
        }
        
    }

}

