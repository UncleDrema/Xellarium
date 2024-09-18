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
        
        services.AddTransient<IValueResolver<UserDTO, User, ICollection<Rule>>, MappingProfile.UserRulesResolver>();
        services.AddTransient<IValueResolver<UserDTO, User, ICollection<Collection>>, MappingProfile.UserCollectionsResolver>();
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
            IValueResolver<UserDTO, User, ICollection<Collection>> userCollectionsResolver,
            IValueResolver<UserDTO, User, ICollection<Rule>> userRulesResolver)
        {
            CreateMap<Neighborhood, NeighborhoodDTO>();
            CreateMap<NeighborhoodDTO, Neighborhood>();
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(userRulesResolver))
                .ForMember(dest => dest.Collections, opt => opt.MapFrom(userCollectionsResolver));
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules))
                .ForMember(dest => dest.Collections, opt => opt.MapFrom(src => src.Collections));;
            CreateMap<Collection, CollectionDTO>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Owner.Id))
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules.Select(rule => rule.Id)));
            CreateMap<Rule, RuleDTO>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Owner.Id))
                .ForMember(dest => dest.Collections, opt => opt.MapFrom(src => src.Collections.Select(collection => collection.Id)));
        }
        
        public class UserRulesResolver : IValueResolver<UserDTO, User, ICollection<Rule>>
        {
            private readonly IRuleRepository _ruleRepository;

            public UserRulesResolver(IRuleRepository ruleRepository)
            {
                _ruleRepository = ruleRepository;
            }
            
            public ICollection<Rule> Resolve(UserDTO source, User destination, ICollection<Rule> destMember, ResolutionContext context)
            {
                return Task.Run(async () => await _ruleRepository.GetAllByIds(source.Rules.Select(r => r.Id))).Result.ToList();
            }
        }

        public class UserCollectionsResolver : IValueResolver<UserDTO, User, ICollection<Collection>>
        {
            private readonly ICollectionRepository _collectionRepository;

            public UserCollectionsResolver(ICollectionRepository collectionRepository)
            {
                _collectionRepository = collectionRepository;
            }
            
            public ICollection<Collection> Resolve(UserDTO source, User destination, ICollection<Collection> destMember, ResolutionContext context)
            {
                return Task.Run(async () => await _collectionRepository.GetAllByIds(source.Rules.Select(r => r.Id))).Result.ToList();
            }
        }
    }
}