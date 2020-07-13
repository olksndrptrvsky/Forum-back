using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMessageService messageService;

        public MessageController(IMapper mapper, IMessageService messageService)
        {
            this.mapper = mapper;
            this.messageService = messageService;
        }

        [Authorize(Roles = "Moderator, User")]
        [HttpDelete("{id}")]
        public IActionResult DeleteMessage(int id)
        {
            if (messageService.UserCanDeleteMessage(Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value), id))
            {
                messageService.DeleteMessage(id);
                return NoContent();
            }
            return Unauthorized();
        }
    }
}