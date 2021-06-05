using System.Threading.Tasks;

namespace chat
{
    public interface ICommandHandler
    {
        Task HandleInput(string input);
    }
}