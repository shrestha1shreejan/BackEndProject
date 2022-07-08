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
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IUserRepository _unitOfWork.UserRepository;
        //private readonly IMessageRepository _unitOfWork.MessageRepository;
        private readonly IMapper _mapper;

        #region Consturctor
        //public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        //{
        //    _unitOfWork.UserRepository = userRepository;
        //    _unitOfWork.MessageRepository = messageRepository;
        //    _mapper = mapper;
        //}

        public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #endregion

        #region Methods

        //[HttpPost]
        //public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        //{
        //    var username = User.GetUsername();
        //    if (username == createMessageDto.RecipientUsername.ToLower())
        //    {
        //        return BadRequest("You cannot send message to yourself");
        //    }

        //    var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        //    var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        //    // check if recipient exists
        //    if (recipient == null)
        //    {
        //        return NotFound();
        //    }

        //    var message = new Message 
        //    {
        //        Sender = sender,
        //        Recipient = recipient,
        //        SenderUserName = sender.UserName,
        //        RecipientUserName = recipient.UserName,
        //        Content = createMessageDto.Content
        //    };

        //    _unitOfWork.MessageRepository.AddMessage(message);

        //    var res = await _unitOfWork.Complete();

        //    if (res)
        //    {
        //        return Ok(_mapper.Map<MessageDto>(message));
        //    }

        //    return BadRequest("Failed to send message");
        //}


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _unitOfWork.MessageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }

        // Using signalR for this
        //[HttpGet("thread/{username}")]
        //public async Task<ActionResult<ActionResult<IEnumerable<MessageDto>>>> GetMessageThread(string username)
        //{
        //    var currentUsername = User.GetUsername();

        //    var messagethread = await _unitOfWork.MessageRepository.GetMessageThread(currentUsername, username);

        //    return Ok(messagethread);
        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username)
            {
                return Unauthorized();
            }

            if (message.Sender.UserName == username)
            {
                message.SenderDeleted = true;
            }

            if (message.Recipient.UserName == username)
            {
                message.RecipientDeletedd= true;
            }

            if (message.SenderDeleted && message.RecipientDeletedd)
            {
                _unitOfWork.MessageRepository.DeleteMessage(message);
            }

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Problem deleting the messge");
        }
        #endregion
    }
}
