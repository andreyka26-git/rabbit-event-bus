using System.Threading.Tasks;

namespace EventBus.Sample.ChildService.Services
{
    public interface IChildService
    {
        Task DoSomethingAsync();
    }
}