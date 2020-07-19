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
    public class MessageController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMessageService messageService;
        private readonly IConfiguration config;
        private readonly int pageSize;


        public MessageController(IMapper mapper, IMessageService messageService, IConfiguration config)
        {
            this.mapper = mapper;
            this.messageService = messageService;
            this.config = config;
            pageSize = Convert.ToInt32(this.config["Paging:Size"]);

        }



        [HttpGet("{themeId}/{pageNumber}")]
        public ActionResult<IEnumerable<MessageDTO>> GetMessageForTheme(int themeId, int pageNumber)
        {
            return Ok(messageService.GetMessagesInTheme(themeId, pageNumber, pageSize));
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


        [HttpGet("{messageId}/replies/{page}")]
        public ActionResult<IEnumerable<MessageDTO>> GetReplies(int messageId, int page)
        {
            return Ok(messageService.GetRepliesForMessage(messageId, page, pageSize));
        }


        [HttpGet("{id}")]
        public MessageDTO GetMessage(int id)
        {
            return messageService.GetMessageAsync(id).Result;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageVM messageVM)
        {
            var createdMessage = await messageService.CreateAsync(
                mapper.Map<MessageDTO>(messageVM),
                Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            
            return CreatedAtAction(nameof(GetMessage), new { id = createdMessage.Id }, createdMessage);
        }


        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<ActionResult<MessageDTO>> UpdateMessage(int id, MessageDTO message)
        {

            if (!messageService.IsMessageExist(id))
            {
                return await CreateMessage(mapper.Map<CreateMessageVM>(message));
            }
            await messageService.UpdateAsync(id, mapper.Map<MessageDTO>(message));
            return Ok();
        }


    }
}