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
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public ReportController(IThemeService themeService, IMessageService messageService,
            IUserService userService, IMapper mapper)
        {
            this.themeService = themeService;
            this.messageService = messageService;
            this.userService = userService;
            this.mapper = mapper;
        }

        private int GetCurrentUserId()
        {
            return Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }


        [HttpPost("theme")]
        public IActionResult ReportTheme(ReportVM report)
        {
            var reportDTO = mapper.Map<ReportDTO>(report);
            reportDTO.Reporter.Id = GetCurrentUserId();
            themeService.ReportTheme(reportDTO);
            return NoContent();
        }

        [HttpPost("message")]
        public IActionResult ReportMessage(ReportVM report)
        {
            var reportDTO = mapper.Map<ReportDTO>(report);
            reportDTO.Reporter.Id = GetCurrentUserId();
            messageService.ReportMessage(reportDTO);
            return NoContent();
        }

        [Authorize(Roles = "Moderator")]
        [HttpGet("themes")]
        public ActionResult<IEnumerable<EntityReportDTO<ThemeListItemDTO>>> GetReportedThemesWithReports()
        {
            var moderId = GetCurrentUserId();
            return Ok(themeService.GetReportedThemesWithReports(moderId));
        }

        [Authorize(Roles = "Moderator")]
        [HttpGet("messages")]
        public ActionResult<IEnumerable<EntityReportDTO<MessageDTO>>> GetReportedMessagesWithReports()
        {
            var moderId = GetCurrentUserId();
            return Ok(messageService.GetReportedMessagesWithReports(moderId));
        }


        [Authorize(Roles = "Moderator")]
        [HttpPatch("check/theme/{reportId}")]
        public ActionResult CheckThemeReport(int reportId)
        {
            var moderId = GetCurrentUserId();
            if (!themeService.IsModeratingThemeReport(moderId, reportId)) return Forbid();
            themeService.CheckReport(reportId);
            return Ok();
        }


        [Authorize(Roles = "Moderator")]
        [HttpPatch("check/message/{reportId}")]
        public ActionResult CheckMessageReport(int reportId)
        {
            var moderId = GetCurrentUserId();
            if (!messageService.IsModeratingMessageReport(moderId, reportId)) return Forbid();
            messageService.CheckReport(reportId);
            return Ok();
        }




    }
}