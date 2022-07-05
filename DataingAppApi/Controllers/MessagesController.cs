using Application.Common.Helpers;
using Application.DatingApp.Interface;
using AutoMapper;
using DataingAppApi.Extensions;
using Domain.Common.ExtensionMethods;
using Domain.DatingSite;
using Domain.DatingSite.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataingAppApi.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        #region Consturctor
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }
        #endregion

        #region Methods

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();
            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send message to yourself");
            }

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            // check if recipient exists
            if (recipient == null)
            {
                return NotFound();
            }

            var message = new Message 
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            var res = await _messageRepository.SaveAllAsync();

            if (res)
            {
                return Ok(_mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _messageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }


        [HttpGet("thread/{username}")]
        public async Task<ActionResult<ActionResult<IEnumerable<MessageDto>>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            var messagethread = await _messageRepository.GetMessageThread(currentUsername, username);

            return Ok(messagethread);
        }


        #endregion
    }
}
