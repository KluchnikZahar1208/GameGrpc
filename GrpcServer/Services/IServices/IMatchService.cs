using GrpcServer;
using GrpcServer.Protos;

namespace GrpcServer.Services.IServices
{
    public interface IMatchService
    {
        MatchHistory CreateMatch(double amount);
        IEnumerable<MatchHistory> GetWaitingMatches();
        void JoinMatch(int matchId, int playerId);
        MatchHistory MakeMove(int matchId, int playerId, string move);
    }
}
