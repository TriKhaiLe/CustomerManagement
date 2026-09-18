using System.Threading.Tasks;

namespace CustomerManagement.Client.Services
{
    public interface ILanguageService
    {
        string CurrentCultureName { get; }
        Task InitializeAsync();
        Task SetCultureAsync(string cultureName);
    }
}
