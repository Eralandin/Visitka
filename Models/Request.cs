using System.ComponentModel.DataAnnotations;

namespace Visitka.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ClientName { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Telegram { get; set; }

        [Required]
        [StringLength(150)]
        public string NameOfTask { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}