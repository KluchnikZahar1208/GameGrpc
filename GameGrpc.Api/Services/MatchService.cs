using GameGrpc.Api.Models;
using GameGrpc.Api.Services.IServices;

namespace GameGrpc.Api.Services
{
    public class MatchService : IMatchService
    {
        private readonly DataContext.DataContext _context;
        private readonly ITransactionService _transferService;

        public MatchService(DataContext.DataContext context, ITransactionService transferService)
        {
            _context = context;
            _transferService = transferService;
        }

        public MatchHistory CreateMatch(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Negative amount.");
            }

            var match = new MatchHistory { StakeAmount = amount };
            _context.MatchHistories.Add(match);
            _context.SaveChanges();

            return match;
        }

        public IEnumerable<MatchHistory> GetWaitingMatches()
        {
            return _context.MatchHistories.Where(m => m.Player1Id == null || m.Player2Id == null).ToList();
        }

        public void JoinMatch(int matchId, int playerId)
        {
            var game = _context.MatchHistories.Find(matchId);

            if (game == null)
            {
                throw new KeyNotFoundException("Game not found");
            }

            if (game.Player1Id == playerId)
            {
                throw new InvalidOperationException("You cannot connect to your own game");
            }

            var waitingMatch = _context.MatchHistories.FirstOrDefault(m => m.Id == matchId && (m.Player1Id == null || m.Player2Id == null));

            if (waitingMatch == null)
            {
                throw new KeyNotFoundException("No waiting player for this game");
            }

            var player = _context.Users.First(u => u.Id == playerId);

            if (waitingMatch.StakeAmount > player.Balance)
            {
                throw new ArgumentException("Amount is bigger to connect");
            }

            if (waitingMatch.Player1Id is null)
            {
                waitingMatch.Player1Id = playerId;
            }
            else if (waitingMatch.Player2Id is null)
            {
                waitingMatch.Player2Id = playerId;
            }

            _context.MatchHistories.Update(waitingMatch);
            _context.SaveChanges();
        }

        public MatchHistory MakeMove(int matchId, int playerId, string move)
        {
            var match = _context.MatchHistories.Find(matchId);

            // Check if match exists
            if (match == null)
            {
                throw new KeyNotFoundException("Game not found");
            }

            // Check if the game is already over
            if (match.WinnerId != null)
            {
                throw new InvalidOperationException("Game is over");
            }

            // Check if the player is part of the match
            if (match.Player1Id != playerId && match.Player2Id != playerId)
            {
                throw new InvalidOperationException("Player not found in this game");
            }

            // Record the move
            if (match.Player1Id == playerId)
            {
                match.Player1Move = move;
            }
            else
            {
                match.Player2Move = move;
            }

            // Check if both players have made their moves
            if (!string.IsNullOrEmpty(match.Player1Move) && !string.IsNullOrEmpty(match.Player2Move))
            {
                // Determine the winner
                string winner = DetermineWinner(match.Player1Move, match.Player2Move);

                // Update the match with the winner
                match.WinnerId = (winner == "Draw") ? 0 : ((winner == "Player1") ? match.Player1Id : match.Player2Id);

                // Update the match in the database
                _context.MatchHistories.Update(match);
                _context.SaveChanges();

                // Perform the transaction
                _transferService.MakeTransaction(matchId);

                return match;
            }

            // Update the match in the database
            _context.MatchHistories.Update(match);
            _context.SaveChanges();

            return match;
        }

        private string DetermineWinner(string move1, string move2)
        {
            if (move1 == "К" && move2 == "Н" ||
                move1 == "Н" && move2 == "Б" ||
                move1 == "Б" && move2 == "К")
            {
                return "Player1";
            }
            else if (move1 == move2)
            {
                return "Draw";
            }
            else
            {
                return "Player2";
            }
        }
    }
}
