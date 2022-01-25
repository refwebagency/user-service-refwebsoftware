using System.Collections.Generic;
using UserService.Models;

namespace UserService.Data
{
    public interface IUserRepo
    {
        bool SaveChanges();

        IEnumerable<User> GetAllUsers();

        User GetUserById(int id);

        Specialization GetSpecializationById(int id);

        public bool IfSpecializationExist(int id);

        IEnumerable<User> GetUserByMeetId(int id);

        IEnumerable<User> GetUserByExpIdAndSpecId(int Xp, int SpecId);

        void CreateUser(User user);

        void CreateSpecialization(Specialization specialization);

        void UpdateUserById(int id);

        void DeleteUserById(int id);

        bool VerifyUserByEmail(string email);
    }
}