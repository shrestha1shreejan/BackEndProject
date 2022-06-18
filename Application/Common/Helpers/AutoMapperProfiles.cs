using AutoMapper;
using Domain.Common.Auth;
using Domain.Common.ExtensionMethods;
using Domain.DatingSite;
using Domain.DatingSite.Dtos;

namespace Application.Common.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        #region Constructor
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(destinationMember => destinationMember.PhotoUrl,
                sourceMember => sourceMember.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(destinationMember => destinationMember.Age,
                sourceMember => sourceMember.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
        }

        #endregion
    }
}
