using FluentValidation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eShopMVC.App.ViewModels.Category
{
    public class CreateCategoryViewModel
    {
        [DisplayName("Назва категорії")]
        [Required(ErrorMessage = "Поле не може бути пустим")]
        [MinLength(3, ErrorMessage = "Назва категорії повинна бути > 3 символів")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Порядок")]
        [Range(1, 100, ErrorMessage = "Порядок виведення має бути числом з діапазону між 1 та 1000.")]
        public int DisplayOrder { get; set; }        
    }
}
