using GrpcServer.Protos;
using GrpcServer.Services.IServices;

namespace GrpcServer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly DataContext.DataContext _context;

        public TransactionService(DataContext.DataContext context)
        {
            _context = context;
        }

        public void MakeTransaction(int matchId)
        {
            var transaction = _context.GameTransactions.FirstOrDefault(t => t.MatchId == matchId);
            if (transaction != null)
            {
                throw new InvalidOperationException("Transaction already exists");
            }

            var match = _context.MatchHistories.FirstOrDefault(m => m.Id == matchId);
            if (match == null)
            {
                throw new KeyNotFoundException("Match not found");
            }

            if (match.WinnerId is null)
            {
                throw new InvalidOperationException("Match is not ended");
            }

            if (match.WinnerId == 0)
            {
                _context.GameTransactions.Add(new GameTransactions()
                {
                    UserId = (int)match.Player1Id,
                    MatchId = matchId,
                    Amount = 0,
                    TransactionType = "Draw"
                });

                _context.GameTransactions.Add(new GameTransactions()
                {
                    UserId = (int)match.Player2Id,
                    MatchId = matchId,
                    Amount = 0,
                    TransactionType = "Draw"
                });

                _context.SaveChanges();
                return;
            }

            int winnerId = _context.Users.First(u => u.Id == match.WinnerId).Id;
            int loserId = match.Player1Id == match.WinnerId ? (int)match.Player2Id : (int)match.Player1Id;
            GameTransactions winnerTransaction = new GameTransactions()
            {
                UserId = winnerId,
                MatchId = matchId,
                Amount = match.StakeAmount,
                TransactionType = "Addition"
            };

            User winner = _context.Users.First(u => u.Id == winnerId);
            winner.Balance += match.StakeAmount;
            _context.Users.Update(winner);

            _context.GameTransactions.Add(winnerTransaction);

            GameTransactions loserTransaction = new GameTransactions()
            {
                UserId = loserId,
                MatchId = matchId,
                Amount = match.StakeAmount,
                TransactionType = "Write-Off"
            };

            User loser = _context.Users.First(u => u.Id == loserId);
            loser.Balance -= match.StakeAmount;
            _context.Users.Update(loser);

            _context.GameTransactions.Add(loserTransaction);

            _context.SaveChanges();
        }
    }
}
