using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel.Compte
{
    public class ConnexionViewModel
    {
		[Required(ErrorMessage = "Le nom d'utilisateur est requis")]
		[Display(Name = "Nom d'utilisateur")]
        public required string NomUtilisateur { get; set; }
        [Required(ErrorMessage ="Le mot de passe est requis")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public required string MotDePasse { get; set; }

        [Display(Name = "Se souvenir de moi ?")]
        public bool SeRappelerDeMoi { get; set; }
    }
}
