using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcServer.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private static readonly List<Movie> _movies = new();

        // Method to get all movies
        public override Task<GetAllMoviesResponse> GetAllMovies(Empty request, ServerCallContext context)
        {
            var response = new GetAllMoviesResponse();
            response.Movies.AddRange(_movies); // Add all movies to the response
            return Task.FromResult(response);
        }

        // Method to add a new movie
        public override Task<AddMovieResponse> AddMovie(AddMovieRequest request, ServerCallContext context)
        {
            var newMovie = new Movie
            {
                Name = request.Name,
                Genre = request.Genre,
                ReleaseYear = request.ReleaseYear
            };

            _movies.Add(newMovie); // Add the new movie to the in-memory store

            return Task.FromResult(new AddMovieResponse
            {
                Message = $"Movie '{newMovie.Name}' added successfully!"
            });
        }

        // Method to get a movie by its name
        public override Task<GetMovieByNameResponse> GetMovieByName(GetMovieByNameRequest request, ServerCallContext context)
        {
            var movie = _movies.Find(m => m.Name.Equals(request.Name, System.StringComparison.OrdinalIgnoreCase));

            if (movie != null)
            {
                return Task.FromResult(new GetMovieByNameResponse
                {
                    Movie = movie
                });
            }

            // Handle the case where the movie is not found
            throw new RpcException(new Status(StatusCode.NotFound, $"Movie '{request.Name}' not found."));
        }
    }
}
