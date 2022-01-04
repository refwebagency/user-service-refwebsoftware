using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos
{
    public class UserCreateDto
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Experience { get; set; }
    }
}