using Domain.Common.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DataingAppApi.SignalROperations
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        #region Constructor
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }
        #endregion

        /// <summary>
        /// method to send all clients notificaiton of the people that are online
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {

            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            await SendOnlineDetailsAsync();
        }

        /// <summary>
        /// method to send all client notification of the people that are offline
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            if(isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
           
            /// Passing the exception to the base class
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// method to get currentley logged in users and 
        /// send the online user information to them
        /// </summary>
        /// <returns></returns>
        private async Task SendOnlineDetailsAsync()
        {
            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }
    }
}
