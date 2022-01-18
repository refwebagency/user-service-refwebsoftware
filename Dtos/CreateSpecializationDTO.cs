using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos
{
    public class CreateSpecializationDTO
    {   
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
}