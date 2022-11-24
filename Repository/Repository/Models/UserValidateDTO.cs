using System.ComponentModel.DataAnnotations;

namespace Repository.Models
{
    public class UserValidateDTO
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
