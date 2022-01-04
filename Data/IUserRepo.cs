using System.Collections.Generic;
using UserService.Models;

namespace UserService.Data
{
    public interface IUserRepo
    {
        bool SaveChanges();

        IEnumerable<User> GetAllUsers();

        User GetUserById(int id);

        void CreateUser(User user);

        void UpdateUserById(int id);

        void DeleteUserById(int id);

        bool VerifyUserByEmail(string email);
    }
}