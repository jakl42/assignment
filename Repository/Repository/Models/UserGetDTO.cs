namespace Repository.Models
{
    public class UserGetDTO
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public string? Language { get; set; }
        public string? Culture { get; set; }
    }
}
