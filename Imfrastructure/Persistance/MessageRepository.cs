using Application.Common.Helpers;
using Application.Common.Interface;
using Application.DatingApp.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.DatingSite;
using Domain.DatingSite.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    internal class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        #region Constructor
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion

        #region Implementation
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" =>  query.Where(u => u.Recipient.UserName == messageParams.Username 
                    && u.RecipientDeletedd == false), // return messages that recipient hasn't deleted
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username 
                    && u.SenderDeleted == false), // return messages that sender hasn't deleted
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.DateRead == null 
                    && u.RecipientDeletedd == false)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            /// Get the message conversation between two users that are not deleted by either user
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(
                    m => m.Recipient.UserName == currentUsername &&
                    m.RecipientDeletedd == false &&
                    m.Sender.UserName == recipientUsername ||
                    m.Recipient.UserName == recipientUsername &&
                    m.Sender.UserName == currentUsername &&
                    m.SenderDeleted == false
                )
                .OrderBy( m => m.MessageSent)
                .ToListAsync();
            ///

            /// check if unread messages for current user are there and mark them as read
            var unreadMessages = messages.Where( m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

    }
}
