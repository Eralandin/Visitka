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
        [Range(0, 999999999999.99)]
        public decimal MinCost { get; set; }
    }
}