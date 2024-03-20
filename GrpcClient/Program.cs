using GrpcServer.Services;
using Grpc.Net.Client;
using GrpcServer.Protos;
using Grpc.Core;
using System.Net.Http;


internal class Program
{
    static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7172");
        var client = new Game.GameClient(channel);

        Console.WriteLine("Write Username");
        var username = Console.ReadLine();
        int userId = 0;

        try
        {
            var createUserRequest = new CreateUserRequest { Username = username };
            var createUserResponse = await client.CreateUserAsync(createUserRequest);
            userId = createUserResponse.UserId;
            Console.WriteLine($"User ID: {userId}");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists)
        {
            Console.WriteLine($"User with username '{username}' already exists.");
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Status.Detail}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }

        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. View balance");
            Console.WriteLine("2. Get list of games");
            Console.WriteLine("3. Connect to a game");
            Console.WriteLine("4. Create game");
            Console.WriteLine("5. Make move");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":

                    var getUserBalanceRequest = new GetUserBalanceRequest { Username = username };
                    var getUserBalanceResponse = await client.GetUserBalanceAsync(getUserBalanceRequest);
                    Console.WriteLine($"User balance: {getUserBalanceResponse.Balance}");
                    break;

                case "2":

                    var getGamesListRequest = new GetGamesListRequest();
                    var getGamesListResponse = await client.GetGamesListAsync(getGamesListRequest);
                    if( getGamesListResponse is null )
                    {
                        Console.WriteLine("No waiting games");
                    }
                    foreach (var match in getGamesListResponse.Matches)
                    {
                        Console.WriteLine($"Match ID: {match.Id}, Stake Amount: {match.StakeAmount}");
                    }
                    break;

                case "3":
                    int matchid;
                    Console.WriteLine("Enter  match id to join:");
                    while (!int.TryParse(Console.ReadLine(), out matchid))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number:");
                    }
                    try
                    {
                        var joinGameRequest = new JoinGameRequest { MatchId = matchid, PlayerId = userId };
                        var joinGameResponse = await client.JoinGameAsync(joinGameRequest);
                        Console.WriteLine(joinGameResponse.Message);
                    }
                    catch (RpcException ex)
                    {
                        if (ex.Status.StatusCode == StatusCode.InvalidArgument)
                        {
                            Console.WriteLine("Error: Invalid argument. This user is already connected to the game.");
                        }
                        else
                        {
                            Console.WriteLine($"RPC Exception: {ex.Status.StatusCode}: {ex.Status.Detail}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                    }
                    break;
                case "4":

                    double amount;
                    Console.WriteLine("Enter the amount for the match:");
                    while (!Double.TryParse(Console.ReadLine(), out amount))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number:");
                    }
                    try
                    {
                        var createMatchRequest = new CreateMatchRequest { Amount = amount }; 
                        var createMatchResponse = await client.CreateMatchAsync(createMatchRequest);
                        var match = createMatchResponse.Match;
                        Console.WriteLine($"Match created successfully. Match ID: {match.Id}, Stake Amount: {match.StakeAmount}");
                    }
                    catch (RpcException ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Status.Detail}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    }

                    break;
                case "5":
                    Console.WriteLine("Write match id: ");
                    int matchId;
                    while (!int.TryParse(Console.ReadLine(), out matchId))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid integer match id:");
                    }
                    Console.WriteLine("Enter your move (К/Н/Б):");
                    var move = Console.ReadLine().ToUpper(); // Convert to uppercase for consistency

                    switch (move)
                    {
                        case "К":
                        case "Н":
                        case "Б":
                            try
                            {
                                var makeMoveRequest = new MakeMoveRequest
                                {
                                    MatchId = matchId,
                                    PlayerId = userId,
                                    Move = move
                                };

                                var makeMoveResponse = await client.MakeMoveAsync(makeMoveRequest);
                                Console.WriteLine($"Move made successfully");
                            }
                            catch (RpcException ex)
                            {
                                if (ex.Status.StatusCode == StatusCode.NotFound)
                                {
                                    Console.WriteLine("Error: Game not found.");
                                }
                                else if (ex.Status.StatusCode == StatusCode.FailedPrecondition)
                                {
                                    Console.WriteLine("Error: Game is over.");
                                }
                                else if (ex.Status.StatusCode == StatusCode.InvalidArgument)
                                {
                                    Console.WriteLine("Error: Player not found in this game.");
                                }
                                else
                                {
                                    Console.WriteLine($"RPC Exception: {ex.Status.StatusCode}: {ex.Status.Detail}");
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Exception: {ex.Message}");
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid move. Please enter К, Н, or Б.");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    break;
            }
        }
    }
}