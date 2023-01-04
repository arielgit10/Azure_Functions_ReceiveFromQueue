

using FunctionAppTimerTriggerReceiver;
using System.Threading.Tasks;

namespace TimerTriggerReceiver
{
    public interface IServerRepository
    {
        Task PostAccount(Account account);
    }
}