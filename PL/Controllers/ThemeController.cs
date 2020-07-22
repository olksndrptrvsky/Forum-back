using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PL.ViewModels;

namespace PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly IThemeService themeService;
        private readonly IConfiguration config;
        private readonly IMapper mapper;
        private readonly int pageSize;

        public ThemeController(IThemeService themeService, IConfiguration config, IMapper mapper)
        {
            this.themeService = themeService;
            this.config = config;
            this.mapper = mapper;
            pageSize = Convert.ToInt32(config["Paging:Size"]);
        }


        private int GetCurrentUserId()
        {
            return Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }





        [HttpGet("latest/{pagingNumber}")]
        public ActionResult<IEnumerable<ThemeListItemDTO>> GetLatestThemes(int pagingNumber)
        {
            var result = themeService.GetLatestThemes(pagingNumber, pageSize);
            if (result.Any()) return Ok(result);
            else return NotFound();
        }

        [HttpGet("popular/{pagingNumber}")]
        public ActionResult<IEnumerable<ThemeListItemDTO>> GetMostPopularThemes(int pagingNumber)
        {
            var result = themeService.GetPopularThemes(pagingNumber, pageSize);
            if (result.Any()) return Ok(result);
            else return NotFound();
        }


        [HttpGet("search/{pagingNumber}")]
        public ActionResult<IEnumerable<ThemeListItemDTO>> SearchThemes(string search, int pagingNumber)
        {
            if (search == null)
            {
                return BadRequest();
            }
            var result = themeService.SearchThemes(search, pagingNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public ActionResult<ThemeDTO> GetTheme(int id)
        {
            var theme = themeService.GetThemeById(id);
            if (theme == null) return NotFound();
            return Ok(theme);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ThemeDTO>> CreateThemeAsync(CreateThemeVM themeVM)
        {
            var createdTheme = await themeService
                .CreateAsync(mapper.Map<ThemeDTO>(themeVM), GetCurrentUserId());
            return CreatedAtAction(nameof(GetTheme), new { id = createdTheme.Id }, createdTheme);
        }

        

        [Authorize(Roles = "Administrator")]
        [HttpGet("unmoderated/{pageNumber}")]
        public ActionResult<IEnumerable<ThemeListItemDTO>> GetUnmoderatedThemes(int pageNumber)
        {
            var result = themeService.GetThemesWithoutModers(pageNumber, pageSize);
            if (!result.Any()) return NotFound();
            return Ok(result);
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet("unmoderatedCount")]
        public int GetUnmoderatedThemeCount()
        {
            return themeService.GetUnmoderatedThemeCount();
        }



        [Authorize(Roles = "Administrator")]
        [HttpPost("moder")]
        public async Task<IActionResult> AddModerToThemeAsync(ThemeModerVM themeModerVM)
        {
            await themeService.AddModerToThemeAsync(mapper.Map<ThemeModerDTO>(themeModerVM));
            return NoContent();
        }


        [Authorize(Roles = "Moderator, User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThemeAsync(int id)
        {
            if (themeService.UserCanDeleteTheme(GetCurrentUserId(), id))
            {
                await themeService.DeleteThemeAsync(id);
                return NoContent();
            }
            return Unauthorized();
        }


        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ThemeDTO>> UpdateTheme(int id, ThemeDTO theme)
        {

            if (!themeService.IsThemeExist(id))
            {
                return await CreateThemeAsync(mapper.Map<CreateThemeVM>(theme));
            }
            if (!themeService.UserIsAuthor(id, GetCurrentUserId()))
            {
                return Unauthorized();
            }
            await themeService.UpdateAsync(id, theme);
            return Ok();
        }

    }
}