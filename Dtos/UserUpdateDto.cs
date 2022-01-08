using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos
{
    public class UserUpdateDto
    {
        [Required]
        public int Id { get; set; }

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
        public int Experience { get; set; }

        [Required]
        public int SpecializationId { get; set; }
    }
}