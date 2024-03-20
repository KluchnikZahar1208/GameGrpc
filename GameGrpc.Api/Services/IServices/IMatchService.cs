using GameGrpc.Api.Models;

namespace GameGrpc.Api.Services.IServices
{
    public interface IMatchService
    {
        MatchHistory CreateMatch(decimal amount);
        IEnumerable<MatchHistory> GetWaitingMatches();
        void JoinMatch(int matchId, int playerId);
        MatchHistory MakeMove(int matchId, int playerId, string move);
    }
}
