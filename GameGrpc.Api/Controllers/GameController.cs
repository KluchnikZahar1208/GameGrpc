using GameGrpc.Api.Models;
using GameGrpc.Api.Services;
using GameGrpc.Api.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace GameGrpc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMatchService _matchService;
        private readonly ITransactionService _transactionService;

        public GameController(IUserService userService, IMatchService matchService, ITransactionService transactionService)
        {
            _userService = userService;
            _matchService = matchService;
            _transactionService = transactionService;
        }

        [HttpPost("createUser")]
        public IActionResult CreateUser(string username)
        {
            try
            {
                var user = _userService.CreateUser(username);
                return Ok(new { UserId = user.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{username}/exists")]
        public ActionResult<bool> CheckUserExists(string username)
        {
            return Ok(_userService.CheckUserExists(username));
        }

        [HttpPost("create-match")]
        public async Task<IActionResult> CreateMatch( decimal amount)
        {
            try
            {
                var match = _matchService.CreateMatch(amount);
                return Ok(match);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("matches-list")]
        public ActionResult<IEnumerable<MatchHistory>> GetGamesList()
        {
            return Ok(_matchService.GetWaitingMatches());
        }

        [HttpPost("join-match/{matchId}")]
        public ActionResult JoinGame(int matchId, int playerId)
        {
            try
            {
                _matchService.JoinMatch(matchId, playerId);
                return Ok("Joined the game successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("make-move/{matchId}/{playerId}")]
        public ActionResult MakeMove(int matchId, int playerId, [FromBody] string move)
        {
            try
            {
                var game = _matchService.MakeMove(matchId, playerId, move);
                return Ok(game);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transaction/{matchId}")]
        public ActionResult MakeTransaction(int matchId)
        {
            try
            {
                _transactionService.MakeTransaction(matchId);
                return Ok("Transaction completed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpGet("balance/{username}")]
        public IActionResult GetUserBalance(string username)
        {
            try
            {
                var balance = _userService.GetUserBalance(username);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> cb10fb4f4e743e69358d3df42a53d401db0c6382
