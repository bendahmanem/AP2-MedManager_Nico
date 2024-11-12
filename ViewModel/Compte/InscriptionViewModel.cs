using MedManager.Models;
using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel.Compte
{
    public class InscriptionViewModel
    {
		[Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
		[Display(Name = "Nom d'utilisateur")]
        public required string NomUtilisateur { get; set; }

		[Required(ErrorMessage = "L'email est obligatoire")]
		[EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

		[Required(ErrorMessage = "Le nom est obligatoire")]
		[Display(Name = "Nom")]
        public required string Nom { get; set; }

		[Required(ErrorMessage = "Le prénom est obligatoire")]
		[Display(Name = "Prénom")]
        public required string Prenom { get; set; }

		[Required(ErrorMessage = "L'adresse est obligatoire")]
		public required string Adresse { get; set; }

        [Display(Name = "Numéro de téléphone")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Le numéro de téléphone est obligatoire")]
        public string? NumeroTel { get; set; }

        [Display(Name = "Spécialité")]
        [Required(ErrorMessage = "La spécialité est obligatoire")]
        public Specialite Specialite { get; set; }

        [Display(Name ="Faculté")]
		[Required(ErrorMessage = "La faculté est obligatoire")]
		public required string Faculte { get; set; }

		[Required(ErrorMessage = "La ville est obligatoire")]
		[Display(Name = "Ville")]
        public required string Ville { get; set; }

		[Required(ErrorMessage = "Le mot de passe est obligatoire")]
		[DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public required string MotDePasse { get; set; }
    }

}
