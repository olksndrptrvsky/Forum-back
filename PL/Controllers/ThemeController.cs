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

namespace PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly IThemeService themeService;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public ThemeController(IThemeService themeService, IConfiguration config, IMapper mapper)
        {
            this.themeService = themeService;
            this.config = config;
            this.mapper = mapper;
        }

        [HttpGet("latest/{pagingNumber}")]
        public IEnumerable<ThemeListItemDTO> GetLatestThemes(int pagingNumber)
        {
            var result = themeService.GetLatestThemes(pagingNumber, Convert.ToInt32(config["Paging:Size"]));
            return result;
        }

        [HttpGet("popular/{pagingNumber}")]
        public IEnumerable<ThemeListItemDTO> GetMostPopularThemes(int pagingNumber)
        {
            var result = themeService.GetPopularThemes(pagingNumber, Convert.ToInt32(config["Paging:Size"]));
            return result;
        }

        [HttpGet("{id}")]
        public ThemeDTO GetTheme(int id)
        {
            return themeService.GetThemeById(id, Convert.ToInt32(config["Paging:Size"]));
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
    }
}