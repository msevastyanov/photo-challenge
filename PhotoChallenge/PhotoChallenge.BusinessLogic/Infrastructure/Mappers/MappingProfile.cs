using AutoMapper;
using PhotoChallenge.Domain.DTO.Area;
using PhotoChallenge.Domain.DTO.Challenge;
using PhotoChallenge.Domain.DTO.UserInteraction;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.BusinessLogic.Infrastructure.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Challenge, ChallengeDto>()
                .ForMember(dest => dest.Award, opt => opt.MapFrom(src => (ChallengeAward)src.Award))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse(typeof(ChallengeStatus), src.Status)))
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Area.Id))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.Name));
            CreateMap<ChallengeDto, Challenge>()
                .ForMember(dest => dest.Award, opt => opt.MapFrom(src => (int)src.Award))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<AreaDto, Area>();
            CreateMap<Area, AreaDto>();

            CreateMap<UserInteraction, UserInteractionDto>()
                .ForMember(dest => dest.ChallengeId, opt => opt.MapFrom(src => src.Challenge.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Challenge.Area.Name))
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Challenge.Area.Id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Challenge.Description))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.ChallengeStart, opt => opt.MapFrom(src => src.Challenge.DateStart))
                .ForMember(dest => dest.ChallengeEnd, opt => opt.MapFrom(src => src.Challenge.DateEnd))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(
                    src => Enum.Parse(typeof(UserInteractionStatus),
                    src.Status == UserInteractionStatus.Pending.ToString() && DateTime.Now.Date > src.Challenge.DateEnd.Date ?
                    UserInteractionStatus.Rejected.ToString() : src.Status)));
            CreateMap<UserInteractionDto, UserInteraction>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<CreateUserInteractionDto, UserInteraction>()
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.FileName));
        }
    }
}
