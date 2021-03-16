namespace CardsAgainstWhatever.Shared.Models
{
    public enum PlayerState
    {
        InLobby,
        PlayingAnswer,
        AnswerPlayed,
        AwatingAnswers,
        PickingWinner
    }

    public class Player
    {
        public string Username { get; set; }
        public int Score { get; set; }
        public PlayerState State { get; set; }

        public Player(string username)
        {
            Username = username;
            State = PlayerState.InLobby;
        }
    }
}
