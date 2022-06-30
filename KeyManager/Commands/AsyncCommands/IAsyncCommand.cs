using System.Threading.Tasks;
using System.Windows.Input;

namespace KeyManager.Commands.AsyncCommands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
