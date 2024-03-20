using GameGrpc.Api.DataContext;
using Newtonsoft.Json.Linq;
using System.Text;

public class Program
{
    static async Task Main(string[] args)
    {
        var baseUrl = "https://localhost:7222/api/Game/";

        using var httpClient = new HttpClient();

        Console.WriteLine("Write Username");
        var username = Console.ReadLine();
        int userId = 0;

        var createUserContent = new StringContent("", Encoding.UTF8, "application/json"); // No content required for POST
        var createUserResponse = await httpClient.PostAsync(baseUrl + $"createUser?username={username}", createUserContent);
        if (createUserResponse.IsSuccessStatusCode)
        {
            string responseData = await createUserResponse.Content.ReadAsStringAsync();
            var responseObject = JObject.Parse(responseData);
            userId = (int)responseObject["userId"];
            Console.WriteLine($"User ID: {userId}");
        }
        else
        {
            Console.WriteLine($"Wrong");
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
                    var getUserBalanceResponse = await httpClient.GetAsync(baseUrl + $"balance/{username}");
                    if (getUserBalanceResponse.IsSuccessStatusCode)
                    {
                        var balance = await getUserBalanceResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"User balance: {balance}");
                    }
                    else
                    {
                        var errorMessage = await getUserBalanceResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to get balance: {getUserBalanceResponse.ReasonPhrase}. Reason: {errorMessage}");
                    }
                    break;
                case "2":
                    var getGamesListResponse = await httpClient.GetAsync(baseUrl + "matches-list");
                    if (getGamesListResponse.IsSuccessStatusCode)
                    {
                        var gamesList = await getGamesListResponse.Content.ReadAsStringAsync();
                        JArray jsonArray = JArray.Parse(gamesList);

                        foreach (JObject item in jsonArray)
                        {
                            int id = (int)item["id"];
                            int stakeAmount = (int)item["stakeAmount"];
                            Console.WriteLine($"ID: {id}, Stake Amount: {stakeAmount}");
                        }
                    }
                    else
                    {
                        var errorMessage = await getGamesListResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to get list of matches: {getGamesListResponse.ReasonPhrase}. Reason: {errorMessage}");
                    }
                    break;
                case "3":
                    Console.WriteLine("Write math id: ");
                    var matchId = Console.ReadLine();
                    var joinMatchResponse = await httpClient.PostAsync($"{baseUrl}join-match/{matchId}?playerId={userId}", null);

                    if (joinMatchResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Joined the match successfully.");
                    }
                    else
                    {
                        var errorMessage = await joinMatchResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to join the match: {joinMatchResponse.ReasonPhrase}. Reason: {errorMessage}");
                    }
                    break;
                case "4":
                    Console.WriteLine("Write amount of match: ");
                    var amount = Console.ReadLine();
                    var createMatchResponse = await httpClient.PostAsync(baseUrl + $"create-match?amount={amount}", null);
                    if (createMatchResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Match created successfully.");
                    }
                    else
                    {
                        var errorMessage = await createMatchResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to create match: {createMatchResponse.ReasonPhrase}. Reason: {errorMessage}");
                    }
                    break;
                case "5":
                    Console.WriteLine("Write math id: ");
                    matchId = Console.ReadLine();
                    Console.WriteLine("Enter your move (К/Н/Б):");
                    var move = Console.ReadLine().ToUpper(); // Convert to uppercase for consistency

                    switch (move)
                    {
                        case "К":
                        case "Н":
                        case "Б":
                            var moveData = new StringContent($"\"{move}\"", Encoding.UTF8, "application/json");

                            var makeMoveResponse = await httpClient.PostAsync($"{baseUrl}make-move/{matchId}/{userId}", moveData);

                            // Check if the request was successful
                            if (makeMoveResponse.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"Move made successfully");
                            }
                            else
                            {
                                // Output the error message
                                var errorMessage = await makeMoveResponse.Content.ReadAsStringAsync();
                                Console.WriteLine($"Failed to make move: {makeMoveResponse.ReasonPhrase}. Reason: {errorMessage}");
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