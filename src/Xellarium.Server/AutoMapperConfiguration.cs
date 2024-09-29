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
        services.AddTransient<IValueResolver<RuleDTO, Rule, User>, MappingProfile.RuleOwnerResolver>();
        services.AddTransient<IValueResolver<RuleDTO, Rule, ICollection<Collection>>, MappingProfile.RuleCollectionsResolver>();
        services.AddTransient<IValueResolver<CollectionDTO, Collection, User>, MappingProfile.CollectionOwnerResolver>();
        services.AddTransient<IValueResolver<CollectionDTO, Collection, ICollection<Rule>>, MappingProfile.CollectionRulesResolver>();
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
            IValueResolver<UserDTO, User, ICollection<Rule>> userRulesResolver,
            IValueResolver<RuleDTO, Rule, User> ruleOwnerResolver,
            IValueResolver<RuleDTO, Rule, ICollection<Collection>> ruleCollectionResolver,
            IValueResolver<CollectionDTO, Collection, User> collectionOwnerResolver,
            IValueResolver<CollectionDTO, Collection, ICollection<Rule>> collectionRulesResolver)
        {
            CreateMap<Collection, CollectionReferenceDTO>();
            CreateMap<Rule, RuleReferenceDTO>();
            CreateMap<Neighborhood, NeighborhoodDTO>();
            /*
            CreateMap<NeighborhoodDTO, Neighborhood>();
            */
            /*
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(userRulesResolver))
                .ForMember(dest => dest.Collections, opt => opt.MapFrom(userCollectionsResolver));
                */
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules))
                .ForMember(dest => dest.Collections, opt => opt.MapFrom(src => src.Collections));;
            CreateMap<Collection, CollectionDTO>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Owner.Id))
                .ForMember(dest => dest.RuleReferences, opt => opt.MapFrom(src => src.Rules));
            /*
            CreateMap<CollectionDTO, Collection>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(collectionOwnerResolver))
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(collectionRulesResolver));
                */
            CreateMap<Rule, RuleDTO>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Owner.Id))
                .ForMember(dest => dest.CollectionReferences, opt => opt.MapFrom(src => src.Collections));
            /*
            CreateMap<RuleDTO, Rule>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(ruleOwnerResolver))
                .ForMember(dest => dest.Collections, opt => opt.MapFrom(ruleCollectionResolver));
                */
        }
        
        public class CollectionOwnerResolver : IValueResolver<CollectionDTO, Collection, User>
        {
            private readonly IUserRepository _userRepository;

            public CollectionOwnerResolver(IUnitOfWork unitOfWork)
            {
                _userRepository = unitOfWork.Users;
            }

            public User Resolve(CollectionDTO source, Collection destination, User destMember, ResolutionContext context)
            {
                return Task.Run(async () => await _userRepository.Get(source.Id)).Result!;
            }
        }

        public class CollectionRulesResolver : IValueResolver<CollectionDTO, Collection, ICollection<Rule>>
        {
            private readonly IRuleRepository _ruleRepository;

            public CollectionRulesResolver(IUnitOfWork unitOfWork)
            {
                _ruleRepository = unitOfWork.Rules;
            }

            public ICollection<Rule> Resolve(CollectionDTO source, Collection destination, ICollection<Rule> destMember,
                ResolutionContext context)
            {
                return Task.Run(async () => await _ruleRepository.GetAllByIdsInclude(source.RuleReferences.Select(colRef => colRef.Id))).Result.ToList();
            }
        }
        
        public class RuleOwnerResolver : IValueResolver<RuleDTO, Rule, User>
        {
            private readonly IUserRepository _userRepository;

            public RuleOwnerResolver(IUnitOfWork unitOfWork)
            {
                _userRepository = unitOfWork.Users;
            }

            public User Resolve(RuleDTO source, Rule destination, User destMember, ResolutionContext context)
            {
                return Task.Run(async () => await _userRepository.Get(source.Id)).Result!;
            }
        }

        public class RuleCollectionsResolver : IValueResolver<RuleDTO, Rule, ICollection<Collection>>
        {
            private readonly ICollectionRepository _collectionRepository;

            public RuleCollectionsResolver(IUnitOfWork unitOfWork)
            {
                _collectionRepository = unitOfWork.Collections;
            }

            public ICollection<Collection> Resolve(RuleDTO source, Rule destination, ICollection<Collection> destMember,
                ResolutionContext context)
            {
                return Task.Run(async () => await _collectionRepository.GetAllByIdsInclude(source.CollectionReferences.Select(colRef => colRef.Id))).Result.ToList();
            }
        }
        
        public class UserRulesResolver : IValueResolver<UserDTO, User, ICollection<Rule>>
        {
            private readonly IRuleRepository _ruleRepository;

            public UserRulesResolver(IUnitOfWork unitOfWork)
            {
                _ruleRepository = unitOfWork.Rules;
            }
            
            public ICollection<Rule> Resolve(UserDTO source, User destination, ICollection<Rule> destMember, ResolutionContext context)
            {
                return Task.Run(async () => await _ruleRepository.GetAllByIdsInclude(source.Rules.Select(r => r.Id))).Result.ToList();
            }
        }

        public class UserCollectionsResolver : IValueResolver<UserDTO, User, ICollection<Collection>>
        {
            private readonly ICollectionRepository _collectionRepository;

            public UserCollectionsResolver(IUnitOfWork unitOfWork)
            {
                _collectionRepository = unitOfWork.Collections;
            }
            
            public ICollection<Collection> Resolve(UserDTO source, User destination, ICollection<Collection> destMember, ResolutionContext context)
            {
                return Task.Run(async () => await _collectionRepository.GetAllByIdsInclude(source.Rules.Select(r => r.Id))).Result.ToList();
            }
        }
    }
}