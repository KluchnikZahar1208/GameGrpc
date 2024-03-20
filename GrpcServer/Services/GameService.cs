using Grpc.Core;
using GrpcServer;
using GrpcServer.Protos;
using GrpcServer.Services.IServices;

namespace GrpcServer.Services
{
    public class GameService : Game.GameBase
    {
        private readonly IUserService _userService;
        private readonly IMatchService _matchService;
        private readonly ITransactionService _transactionService;

        public GameService(IUserService userService, IMatchService matchService, ITransactionService transactionService)
        {
            _userService = userService;
            _matchService = matchService;
            _transactionService = transactionService;
        }

        public override Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            try
            {
                var user = _userService.CreateUser(request.Username);
                return Task.FromResult(new CreateUserResponse { UserId = user.Id });
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, ex.Message));;
            }
        }

        public override Task<CheckUserExistsResponse> CheckUserExists(CheckUserExistsRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CheckUserExistsResponse { Exists = _userService.CheckUserExists(request.Username) });
        }

        public override async Task<CreateMatchResponse> CreateMatch(CreateMatchRequest request, ServerCallContext context)
        {
            try
            {
                var match = _matchService.CreateMatch(request.Amount);
                return new CreateMatchResponse { Match = match };
            }
            catch (ArgumentException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<GetGamesListResponse> GetGamesList(GetGamesListRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetGamesListResponse { Matches = { _matchService.GetWaitingMatches() } });
        }

        public override Task<JoinGameResponse> JoinGame(JoinGameRequest request, ServerCallContext context)
        {
            try
            {
                _matchService.JoinMatch(request.MatchId, request.PlayerId);
                return Task.FromResult(new JoinGameResponse { Message = "Joined the game successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
            }
            
            catch (ArgumentException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<MakeMoveResponse> MakeMove(MakeMoveRequest request, ServerCallContext context)
        {
            try
            {
                var game = _matchService.MakeMove(request.MatchId, request.PlayerId, request.Move);
                return Task.FromResult(new MakeMoveResponse { Game = game });
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<MakeTransactionResponse> MakeTransaction(MakeTransactionRequest request, ServerCallContext context)
        {
            try
            {
                _transactionService.MakeTransaction(request.MatchId);
                return Task.FromResult(new MakeTransactionResponse { Message = "Transaction completed successfully" });
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<GetUserBalanceResponse> GetUserBalance(GetUserBalanceRequest request, ServerCallContext context)
        {
            try
            {
                var balance = _userService.GetUserBalance(request.Username);
                return Task.FromResult(new GetUserBalanceResponse { Balance = balance });
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
    }
}
