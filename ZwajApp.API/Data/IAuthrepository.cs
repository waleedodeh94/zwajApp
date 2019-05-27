using System.Threading.Tasks;
using ZwajApp.API.Models;

namespace ZwajApp.API.Data
{
    public interface IAuthrepository
    {
         Task<User> Register (User user,string Password);
         Task<User> Login (string username,string Password);
         Task<bool> UserExists(string userbane);
    }
}