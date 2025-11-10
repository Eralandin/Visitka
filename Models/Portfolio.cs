using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Visitka.Models
{
    public class Portfolio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string TaskDescription { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string SolutionDescription { get; set; } = string.Empty;

        public byte[]? PreviewImage { get; set; }

        public byte[]? MainImage { get; set; }

        public byte[]? MobileImage { get; set; }
        public int releasedate { get; set; }
    }


    public class PortfolioViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string SolutionDescription { get; set; } = string.Empty;
        public bool HasPreviewImage { get; set; }
        public bool HasMainImage { get; set; }
        public bool HasMobileImage { get; set; }
        public string Category { get; set; } = string.Empty;
        public int releasedate { get; set; }
    }
}