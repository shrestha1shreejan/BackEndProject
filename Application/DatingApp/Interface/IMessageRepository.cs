using Application.Common.Helpers;
using Domain.DatingSite;
using Domain.DatingSite.Dtos;
using Domain.DatingSite.TrackingEntities;

namespace Application.DatingApp.Interface
{
    public interface IMessageRepository
    {
        // For group member tracking
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnectionAsync(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        ///

        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
        Task<bool> SaveAllAsync();
    }
}
