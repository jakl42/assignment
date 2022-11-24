using System.ComponentModel.DataAnnotations;

namespace Repository.Models
{
    public class UserPostDTO
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? FullName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [Phone]
        public string? MobileNumber { get; set; }
        [Required]
        public string? Language { get; set; }
        [Required]
        public string? Culture { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
