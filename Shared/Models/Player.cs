namespace CardsAgainstWhatever.Shared.Models
{


    public class Player
    {
        public string Username { get; set; }
        public int Score { get; set; }
        public PlayerStatus Status { get; set; }

        public Player(string username)
        {
            Username = username;
            Status = PlayerStatus.Lobby;
        }
    }
}
