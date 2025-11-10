using System.ComponentModel.DataAnnotations;

namespace Visitka.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }

    public class PortfolioCategories {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PortfolioId { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
