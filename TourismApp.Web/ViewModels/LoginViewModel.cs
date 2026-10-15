using System.ComponentModel.DataAnnotations;

namespace TourismApp.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Veuillez saisir votre nom d'utilisateur")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veuillez saisir votre mot de passe")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Se souvenir de moi")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
