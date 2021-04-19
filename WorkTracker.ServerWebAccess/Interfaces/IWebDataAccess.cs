using System.Net.Http;
using System.Threading.Tasks;

namespace WorkTracker.WebAccessLayer.Interfaces
{
    public interface IWebDataAccess
    {
        Task<T> GetAsync<T>(string requestUrl);
        Task<T> PostAsync<T>(string requestUrl, HttpContent content);
        Task<T> PutAsync<T>(string requestUrl, HttpContent obj);
        Task<T> DeleteAsync<T>(string requestUrl);
    }
}
