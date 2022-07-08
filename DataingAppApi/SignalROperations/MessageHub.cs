using Application.DatingApp.Interface;
using AutoMapper;
using Domain.DatingSite.Dtos;
using Domain.DatingSite;
using Microsoft.AspNetCore.SignalR;
using Domain.Common.ExtensionMethods;
using Domain.DatingSite.TrackingEntities;

namespace DataingAppApi.SignalROperations
{
    public class MessageHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        // private readonly IMessageRepository _unitOfWork.MessageRepository;
        // private readonly IUserRepository _unitOfWork.UserRepository;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;
        private readonly IMapper _mapper;

        #region Constructor
        //public MessageHub(IMessageRepository messageRepository, IMapper mapper, IUserRepository userRepository
        //    , IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        public MessageHub(IUnitOfWork unitOfWork, IMapper mapper
            , IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            //_unitOfWork.MessageRepository = messageRepository;
            //_unitOfWork.UserRepository = userRepository;
            _unitOfWork = unitOfWork;
            _presenceHub = presenceHub;
            _tracker = tracker;
            _mapper = mapper;
        }
        #endregion

        #region Methods

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            // when connection is made pass name of other user in params users
            var otherUser = httpContext?.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _unitOfWork.MessageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            if (_unitOfWork.HasChanges())
            {
                await _unitOfWork.Complete();
            }

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)          
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Send message to the group of users that are communicting using signalR
        /// </summary>
        /// <param name="createMessageDto"></param>
        /// <returns></returns>
        /// <exception cref="HubException"></exception>
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context?.User?.GetUsername();
            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                throw new HubException("You cannot send message to yourself");
            }

            var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            // check if recipient exists
            if (recipient == null)
            {
                throw new HubException("User not found");
            }

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

            // check if the user is in the group and make the message as read
            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow; 
            }
            else // if user is online but not in the group send notification they have received a message
            {
                var connections = await _tracker.GetConnectionForUser(recipient.UserName);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived" ,
                        new {username = sender.UserName, knownAs = sender.KnownAs});
                }
            }

            _unitOfWork.MessageRepository.AddMessage(message);

            var res = await _unitOfWork.Complete();

            if (res)
            {
                
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        #endregion

        #region Private methods
        /// <summary>
        /// method to consturct group name in alphabetical order of caller and callee name
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}--{other}" : $"{other}--{caller}";
        }

        /// <summary>
        /// Creates a group and adds connection to the group (new or existing)
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName); // creating a new group if it doesn't exist
                _unitOfWork.MessageRepository.AddGroup(group);
            }

            group.Connections.Add(connection); // adding connection to the group

            if (await _unitOfWork.Complete())
                return group;

            throw new HubException("Failed to join group");

        }

        /// <summary>
        /// Remove connection when user disconnects from the hub
        /// </summary>
        /// <returns></returns>
        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _unitOfWork.MessageRepository.RemoveConnection(connection);
            
            if (await _unitOfWork.Complete())
                return group;

            throw new HubException("Failed to remove from group");
        }
        #endregion
    }
}
