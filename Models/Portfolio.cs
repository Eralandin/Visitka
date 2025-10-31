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
    }
}