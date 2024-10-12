using MedManager.Models;
using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public required string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public required string Nom { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public required string Prenom { get; set; }


        [Required]
        public required string Adresse { get; set; }

        [Display(Name = "Numéro de téléphone")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Le numéro de téléphone est obligatoire")]
        public string? NumTel { get; set; }

        [Display(Name = "Spécialité")]
        [Required(ErrorMessage = "La spécialité est obligatoire")]
        public Specialite Specialite { get; set; }


        [Display(Name ="Faculté")]
        [Required]
        public required string Faculte { get; set; }

        [Required]
        [Display(Name = "Ville")]
        public required string Ville { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public required string Password { get; set; }
    }

}
