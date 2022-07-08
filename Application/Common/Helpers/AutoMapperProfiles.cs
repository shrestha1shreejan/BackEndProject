﻿using AutoMapper;
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
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhototUrl,
                opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.RecipientPhototUrl,
                opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
            // Making all date time to UTC (moved converison logic to DataContext)
            // CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        }

        #endregion
    }
}
