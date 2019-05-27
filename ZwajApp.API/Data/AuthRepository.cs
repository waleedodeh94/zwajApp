using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZwajApp.API.Models;

namespace ZwajApp.API.Data
{
    public class AuthRepository : IAuthrepository
    { 
         private readonly DataContext _context;
        public AuthRepository(DataContext Context)
        {
            _context=Context;
        }
        public async Task<User> Login(string username, string Password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Username==username);
            if(user==null)return null;
            if(!VerfyPasswordHash(Password,user.PasswordSalt,user.PasswordHash))
            return null;
            return user;
        }

        private bool VerfyPasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
        {
                using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
               
               var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
               for (int i = 0; i < computeHash.Length; i++)
               {
                   if(computeHash[i]!=passwordHash[i]){
                       return false;
                   }
                   
               }
               return true;
            }
        }

        public async Task<User> Register(User user, string Password)
        {
            byte[] passwordHash,passwordSalt;
            CreatePasswordHash(Password,out passwordHash,out passwordSalt);
            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

           return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
           
        }

        public async Task<bool> UserExists(string username)
        {
         if(await _context.Users.AnyAsync(x=>x.Username==username))
         return true;
         return false;
        }
    }
}