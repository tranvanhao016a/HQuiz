using System.Threading.Tasks;

namespace BlazorQuiz.Shared
{
    public interface IStorageService
    {
        ValueTask SetItem(string key, string value);
        ValueTask<string> GetItem(string key);
        ValueTask RemoveItem(string key);
    }
}
