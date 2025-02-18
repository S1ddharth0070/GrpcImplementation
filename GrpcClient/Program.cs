using Grpc.Net.Client;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;
using GrpcServer;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create a channel to connect to the server
            using var channel = GrpcChannel.ForAddress("https://localhost:7205"); // Adjust the URL if needed
            var client = new Greeter.GreeterClient(channel);

            while (true)
            {
                Console.WriteLine("\n--- Movie Service Menu ---");
                Console.WriteLine("1. Add Movie");
                Console.WriteLine("2. Get All Movies");
                Console.WriteLine("3. Get Movie By Name");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AddMovie(client);
                        break;

                    case "2":
                        await GetAllMovies(client);
                        break;

                    case "3":
                        await GetMovieByName(client);
                        break;

                    case "4":
                        Console.WriteLine("Exiting... Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static async Task AddMovie(Greeter.GreeterClient client)
        {
            Console.WriteLine("\n--- Add Movie ---");
            Console.Write("Enter movie name: ");
            var name = Console.ReadLine();

            Console.Write("Enter movie genre: ");
            var genre = Console.ReadLine();

            Console.Write("Enter release year: ");
            if (!int.TryParse(Console.ReadLine(), out var releaseYear))
            {
                Console.WriteLine("Invalid release year. Please try again.");
                return;
            }

            var request = new AddMovieRequest
            {
                Name = name,
                Genre = genre,
                ReleaseYear = releaseYear
            };

            var response = await client.AddMovieAsync(request);
            Console.WriteLine($"Response: {response.Message}");
        }

        private static async Task GetAllMovies(Greeter.GreeterClient client)
        {
            Console.WriteLine("\n--- Get All Movies ---");

            var response = await client.GetAllMoviesAsync(new Empty());

            if (response.Movies.Count == 0)
            {
                Console.WriteLine("No movies found.");
                return;
            }

            Console.WriteLine("Movies:");
            foreach (var movie in response.Movies)
            {
                Console.WriteLine($"- {movie.Name} ({movie.ReleaseYear}) [{movie.Genre}]");
            }
        }

        private static async Task GetMovieByName(Greeter.GreeterClient client)
        {
            Console.WriteLine("\n--- Get Movie By Name ---");
            Console.Write("Enter movie name: ");
            var movieName = Console.ReadLine();

            try
            {
                var request = new GetMovieByNameRequest { Name = movieName };
                var response = await client.GetMovieByNameAsync(request);

                var movie = response.Movie;
                Console.WriteLine($"Found: {movie.Name} ({movie.ReleaseYear}) [{movie.Genre}]");
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                Console.WriteLine($"Error: {ex.Status.Detail}");
            }
        }
    }
}
