using CardsAgainstWhatever.Server.Services;
using CardsAgainstWhatever.Server.Services.Interfaces;
using CardsAgainstWhatever.Shared.Interfaces;
using CardsAgainstWhatever.Shared.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CardsAgainstWhatever.Server.Tests.Unit
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepositoy> gameRepositoryMock;
        private readonly Mock<IHubContextFascade<IGameClient>> hubContextFascadeMock;
        private readonly IGameService sut;

        public GameServiceTests()
        {
            gameRepositoryMock = new Mock<IGameRepositoy>();
            hubContextFascadeMock = new Mock<IHubContextFascade<IGameClient>>();

            sut = new GameService(gameRepositoryMock.Object, hubContextFascadeMock.Object);
        }


        [Fact]
        public async void Create_DelgatesToRepo_ReturnsGeneratorCode()
        {
            var questionCards = new List<QuestionCard>();
            var answerCards = new List<AnswerCard>();
            var gameCode = "CODEE";

            gameRepositoryMock
                .Setup(x => x.Create(questionCards, answerCards))
                .ReturnsAsync(gameCode)
                .Verifiable();

            var result = await sut.Create(questionCards, answerCards);

            gameRepositoryMock.Verify();
            Assert.Equal(gameCode, result);
        }
    }
}
