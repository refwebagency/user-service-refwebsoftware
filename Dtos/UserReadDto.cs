using UserService.Models;

namespace UserService.Dtos
{
    public class UserReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int Experience { get; set; }

        public int SpecializationId { get; set; }

        public Specialization Specialization { get; set; }
    }
}