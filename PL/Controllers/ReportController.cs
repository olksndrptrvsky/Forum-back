using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PL.ViewModels;

namespace PL.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IThemeService themeService;
        private readonly IMessageService messageService;
        private readonly IMapper mapper;

        public ReportController(IThemeService themeService, IMessageService messageService, IMapper mapper)
        {
            this.themeService = themeService;
            this.messageService = messageService;
            this.mapper = mapper;
        }


        [HttpPost("theme")]
        public IActionResult ReportTheme(ReportVM report)
        {
            var reportDTO = mapper.Map<ReportDTO>(report);
            reportDTO.ReporterId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            themeService.ReportTheme(reportDTO);
            return NoContent();
        }
    }
}