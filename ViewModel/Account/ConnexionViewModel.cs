using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel.Account
{
    public class ConnexionViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public required string MotDePasse { get; set; }

        [Required]
        public required string NomUtilisateur { get; set; }
        [Display(Name = "Remember me?")]
        public bool SeRappelerDeMoi { get; set; }
    }
}
