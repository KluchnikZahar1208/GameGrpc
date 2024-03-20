namespace GameGrpc.Api.Models
{
    public class MatchHistory
    {
        public int Id { get; set; }
        public int? Player1Id { get; set; }
        public string? Player1Move { get; set; }
        public int? Player2Id { get; set; }
        public string? Player2Move { get; set; }
        public int? WinnerId { get; set; }
        public double StakeAmount { get; set; }
    }
}
