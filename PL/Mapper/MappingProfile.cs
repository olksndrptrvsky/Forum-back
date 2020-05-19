using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PL.ViewModels;
using BLL.DTO;
using DAL.Entities;
using BLL.Interfaces;

namespace PL.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Theme, ThemeListItemDTO>()
                .ForMember(dto => dto.AuthorUsername, opt => opt.MapFrom(theme => theme.Author.UserName));

            CreateMap<User, AuthorDTO>();

            CreateMap<Theme, ThemeDTO>();

            CreateMap<ThemeDTO, Theme>();

            CreateMap<Message, MessageDTO>();

            CreateMap<User, UserDTO>();

            CreateMap<CreateThemeVM, ThemeDTO>();
        }
    }
}