namespace GameGrpc.Api.Models
{
    public class GameTransactions
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MatchId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
    }
}
