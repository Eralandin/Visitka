using System.ComponentModel.DataAnnotations;

namespace Visitka.Models
{
    public class Price
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int MinCost { get; set; }
        public byte[]? Image { get; set; }
    }
}