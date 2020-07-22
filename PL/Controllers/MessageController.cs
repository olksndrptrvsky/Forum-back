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
    public class MessageController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMessageService messageService;
        private readonly IThemeService themeService;
        private readonly IConfiguration config;
        private readonly int pageSize;


        public MessageController(IMapper mapper, IMessageService messageService, IThemeService themeService, IConfiguration config)
        {
            this.mapper = mapper;
            this.messageService = messageService;
            this.themeService = themeService;
            this.config = config;
            pageSize = Convert.ToInt32(this.config["Paging:Size"]);

        }

        private int GetCurrentUserId()
        {
            return Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }



        [HttpGet("{themeId}/{pageNumber}")]
        public ActionResult<IEnumerable<MessageDTO>> GetMessageForTheme(int themeId, int pageNumber)
        {
            if (!themeService.IsThemeExist(themeId)) return NotFound();
            var messages = messageService.GetMessagesInTheme(themeId, pageNumber, pageSize);
            if (!messages.Any() && pageNumber > 1)
            {
                return NotFound();
            }
            else
                return Ok(messages);
        }


        [Authorize(Roles = "Moderator, User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessageAsync(int id)
        {
            if (messageService.UserCanDeleteMessage(GetCurrentUserId(), id))
            {
                await messageService.DeleteMessageAsync(id);
                return NoContent();
            }
            return Unauthorized();
        }


        [HttpGet("{messageId}/replies/{page}")]
        public ActionResult<IEnumerable<MessageDTO>> GetReplies(int messageId, int page)
        {
            var message = messageService.GetMessage(messageId);
            if (message == null)
            {
                return NotFound();
            }
            var replies = messageService.GetRepliesForMessage(messageId, page, pageSize);
            if (replies.Any())
                return Ok(replies);
            else 
                return NotFound();
        }


        [HttpGet("{id}")]
        public ActionResult<MessageDTO> GetMessage(int id)
        {
            var message = messageService.GetMessage(id);
            if (message == null)
                return NotFound();
            else 
                return message;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessageAsync(CreateMessageVM messageVM)
        {
            var createdMessage = await messageService
                .CreateAsync(mapper.Map<MessageDTO>(messageVM), GetCurrentUserId());
            
            return CreatedAtAction(nameof(GetMessage), new { id = createdMessage.Id }, createdMessage);
        }


        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<ActionResult<MessageDTO>> UpdateMessageAsync(int id, MessageDTO message)
        {

            if (!messageService.IsMessageExist(id))
            {
                return await CreateMessageAsync(mapper.Map<CreateMessageVM>(message));
            }
            await messageService.UpdateAsync(id, message);
            return Ok();
        }


        [HttpGet("pageCount/{themeId}")]
        public ActionResult<int> GetPageCountForTheme(int themeId)
        {
            if (themeService.IsThemeExist(themeId))
            {
                return messageService.GetPagesCountForTheme(themeId, pageSize);
            }
            else 
            {
                return BadRequest($"No theme with id={themeId}");
            }
        }

    }
}