using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel.Compte
{
    public class ConnexionViewModel
    {
        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public required string NomUtilisateur { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public required string MotDePasse { get; set; }

        [Display(Name = "Se souvenir de moi ?")]
        public bool SeRappelerDeMoi { get; set; }
    }
}
