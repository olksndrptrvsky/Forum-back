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

            CreateMap<ReportVM, ReportDTO>()
                .ForMember(reportDTO => reportDTO.Reporter, (opt) => opt.MapFrom(vm => new AuthorDTO()));

            CreateMap<ReportDTO, ReportTheme>()
                .ForMember(reportTheme => reportTheme.ThemeId, opt => opt.MapFrom(dto => dto.EntityId));
            CreateMap<ReportDTO, ReportMessage>()
                .ForMember(reportMessage => reportMessage.MessageId, opt => opt.MapFrom(dto => dto.EntityId));

            CreateMap<ThemeModerVM, ThemeModerDTO>();

            CreateMap<ThemeModerDTO, ThemeModer>();

            CreateMap<MessageDTO, Message>()
                .ForMember(mes => mes.AuthorId, opt => opt.MapFrom(dto => dto.Author.Id));
            
            CreateMap<CreateMessageVM, MessageDTO>();

            CreateMap<CreateMessageVM, ThemeDTO>();

            CreateMap<AuthorDTO, User>();

            CreateMap<MessageDTO, CreateMessageVM>();

            CreateMap<ReportTheme, ReportDTO>()
                .ForMember(reportDTO => reportDTO.EntityId, opt => opt.MapFrom(rt => rt.ThemeId));

            CreateMap<ReportMessage, ReportDTO>()
                .ForMember(reportDTO => reportDTO.EntityId, opt => opt.MapFrom(rt => rt.MessageId));

        }
    }
}