﻿using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Services.Interfaces
{
    public interface IHubContextFacade<T>
    {
        Task JoinGroup(string groupName, string connectionId);
        Task LeaveGroup(string groupName, string connectionId);
        T GetClient(string connectionId);
        T GetGroup(string groupName);
    }
}
