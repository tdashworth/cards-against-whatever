using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameService GameService;

        public GameHub(IGameService gameService)
        {
            GameService = gameService;
        }

        public Task<string> CreateGame(IEnumerable<QuestionCard> questions, IEnumerable<AnswerCard> answers)
        {
            return GameService.Create(questions, answers);
        }

        public async Task JoinGame(string gameCode, string username)
        {
            await GameService.Join(gameCode, username);
            
            await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
        }

        

    }
}
