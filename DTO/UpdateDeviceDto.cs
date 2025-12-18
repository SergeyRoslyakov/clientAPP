using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientAPP.DTO
{
    public class UpdateDeviceDto
    {
        [Required(ErrorMessage = "Тип устройства обязателен")]
        [StringLength(50, ErrorMessage = "Тип не должен превышать 50 символов")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Бренд обязателен")]
        [StringLength(50, ErrorMessage = "Бренд не должен превышать 50 символов")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Модель обязательна")]
        [StringLength(50, ErrorMessage = "Модель не должна превышать 50 символов")]
        public string Model { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Серийный номер не должен превышать 50 символов")]
        public string SerialNumber { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Описание проблемы не должно превышать 500 символов")]
        public string ProblemDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID клиента обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "ID клиента должен быть положительным числом")]
        public int ClientId { get; set; }
    }
}