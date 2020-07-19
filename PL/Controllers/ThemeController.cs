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
using Microsoft.Extensions.Configuration;
using PL.ViewModels;
using System.Net;
using DAL.Entities;

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

        [HttpGet("latest/{pagingNumber}")]
        public IEnumerable<ThemeListItemDTO> GetLatestThemes(int pagingNumber, string search)
        {
            var result = themeService.GetLatestThemes(pagingNumber, pageSize);
            return result;
        }

        [HttpGet("popular/{pagingNumber}")]
        public IEnumerable<ThemeListItemDTO> GetMostPopularThemes(int pagingNumber)
        {
            var result = themeService.GetPopularThemes(pagingNumber, pageSize);
            return result;
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
        public ThemeDTO GetTheme(int id)
        {
            return themeService.GetThemeById(id);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ThemeDTO>> CreateTheme(CreateThemeVM themeVM)
        {
            var createdTheme = await themeService.CreateAsync(
                mapper.Map<ThemeDTO>(themeVM),
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            return CreatedAtAction(nameof(GetTheme), new { id = createdTheme.Id }, createdTheme);
        }

        

        [Authorize(Roles = "Administrator")]
        [HttpGet("unmoderated/{pageNumber}")]
        public IEnumerable<ThemeListItemDTO> GetUnmoderatedThemes(int pageNumber)
        {
            return themeService.GetThemesWithoutModers(pageNumber, pageSize);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("moder")]
        public IActionResult AddModerToTheme(ThemeModerVM themeModerVM)
        {
            themeService.AddModerToTheme(mapper.Map<ThemeModerDTO>(themeModerVM));
            return NoContent();
        }


        [Authorize(Roles = "Moderator, User")]
        [HttpDelete("{id}")]
        public IActionResult DeleteTheme(int id)
        {
            if (themeService.UserCanDeleteTheme(Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value), id))
            {
                themeService.DeleteTheme(id);
                return NoContent();
            }
            return Unauthorized();
        }

    }
}