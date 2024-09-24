using AutoMapper;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.Shared.DTO;

namespace Xellarium.Server;

public static class AutoMapperConfiguration
{
    public static void ConfigureMapping(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        
        services.AddTransient<IValueResolver<UserDTO, BusinessLogic.Models.User, ICollection<Rule>>, MappingProfile.UserRulesResolver>();
        services.AddTransient<IValueResolver<UserDTO, BusinessLogic.Models.User, ICollection<Collection>>, MappingProfile.UserCollectionsResolver>();
        services.AddTransient<MappingProfile>();

        var provider = services.BuildServiceProvider();
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(provider.GetRequiredService<MappingProfile>());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);
    }
    
    public class MappingProfile : Profile
    {
        public MappingProfile(
            IValueResolver<UserDTO, BusinessLogic.Models.User, ICollection<Collection>> userCollectionsResolver,
            IValueResolver<UserDTO, BusinessLogic.Models.User, ICollection<Rule>> userRulesResolver)
        {
            CreateMap<Collection, CollectionReferenceDTO>();
            CreateMap<Rule, RuleReferenceDTO>();
            CreateMap<Neighborhood, NeighborhoodDTO>();
            CreateMap<NeighborhoodDTO, Neighborhood>();
            CreateMap<UserDTO, BusinessLogic.Models.User>()
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(userRulesResolver))
                .ForMember(dest => dest.Collections, opt => opt.MapFrom(userCollectionsResolver));
            CreateMap<BusinessLogic.Models.User, UserDTO>()
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules))
                .ForMember(dest => dest.Collections, opt => opt.MapFrom(src => src.Collections));;
            CreateMap<Collection, CollectionDTO>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Owner.Id))
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules));
            CreateMap<Rule, RuleDTO>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Owner.Id))
                .ForMember(dest => dest.Collections, opt => opt.MapFrom(src => src.Collections));
        }
        
        public class UserRulesResolver : IValueResolver<UserDTO, BusinessLogic.Models.User, ICollection<Rule>>
        {
            private readonly IRuleRepository _ruleRepository;

            public UserRulesResolver(IRuleRepository ruleRepository)
            {
                _ruleRepository = ruleRepository;
            }
            
            public ICollection<Rule> Resolve(UserDTO source, BusinessLogic.Models.User destination, ICollection<Rule> destMember, ResolutionContext context)
            {
                return Task.Run(async () => await _ruleRepository.GetAllByIds(source.Rules.Select(r => r.Id))).Result.ToList();
            }
        }

        public class UserCollectionsResolver : IValueResolver<UserDTO, BusinessLogic.Models.User, ICollection<Collection>>
        {
            private readonly ICollectionRepository _collectionRepository;

            public UserCollectionsResolver(ICollectionRepository collectionRepository)
            {
                _collectionRepository = collectionRepository;
            }
            
            public ICollection<Collection> Resolve(UserDTO source, BusinessLogic.Models.User destination, ICollection<Collection> destMember, ResolutionContext context)
            {
                return Task.Run(async () => await _collectionRepository.GetAllByIds(source.Rules.Select(r => r.Id))).Result.ToList();
            }
        }
    }
}