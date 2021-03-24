using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Client.Services
{
    public class GameServerProxy //: IGameServer
    {
        private readonly HubConnection connection;

        public GameServerProxy(HubConnection connection)
        {
            this.connection = connection;
        }

        public Task JoinGame(string gameCode, string username) => connection.InvokeAsync(nameof(IGameServer.JoinGame), gameCode, username);

        public Task LeaveGame() => connection.InvokeAsync(nameof(IGameServer.LeaveGame));

        public Task PickWinningAnswer(IEnumerable<AnswerCard> answerCards) => connection.InvokeAsync(nameof(IGameServer.PickWinningAnswer), answerCards);

        public Task PlayAnswer(IEnumerable<AnswerCard> answerCards) => connection.InvokeAsync(nameof(IGameServer.PlayAnswer), answerCards);

        public Task StartRound() => connection.InvokeAsync(nameof(IGameServer.StartRound));
    }
}
