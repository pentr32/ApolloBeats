using System.Threading.Tasks;

namespace Application
{
    class Program
    {
        static async Task Main(string[] args)
            => await new ApolloBeatsClient().InitializeAsync();
    }
}
