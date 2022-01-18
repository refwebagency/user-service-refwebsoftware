using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos
{
    public class UpdateSpecializationDTO
    {   
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}