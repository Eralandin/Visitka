using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Visitka.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [StringLength(150, ErrorMessage = "Email не должен превышать 150 символов")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Номер телефона обязателен")]
        [StringLength(20, ErrorMessage = "Номер телефона не должен превышать 20 символов")]
        [RegularExpression(@"^[\+]?[7-8]?[0-9\s\-\(\)]{10,15}$", 
            ErrorMessage = "Введите корректный номер телефона")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "Telegram не должен превышать 150 символов")]
        public string? Telegram { get; set; }

        [Required(ErrorMessage = "Выберите спектр услуг")]
        [StringLength(150, ErrorMessage = "Название услуги не должно превышать 150 символов")]
        public string NameOfTask { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описание бизнеса обязательно")]
        [StringLength(300, ErrorMessage = "Описание не должно превышать 300 символов")]
        [MinLength(5, ErrorMessage = "Описание должно содержать минимум 5 символов")]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        [Required(ErrorMessage = "Необходимо согласие с политикой конфиденциальности")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Необходимо согласие с политикой конфиденциальности")]
        public bool AgreeToPrivacy { get; set; }
    }
}