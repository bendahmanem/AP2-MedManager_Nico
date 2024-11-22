using MedManager.Models;
using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel.PatientVM
{
    public class PatientViewModel
    {
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères.")]
        public string? Nom { get; set; }
        [Display(Name = "Prénom")]

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 50 caractères.")]
        public string? Prenom { get; set; }

        [Range(20, 250, ErrorMessage = "La taille doit être comprise entre 20cm et 250cm")]
        [Required(ErrorMessage = "La taille est obligatoire")]
        public int? Taille { get; set; }

        [Range(1, 300, ErrorMessage = "Le poids doit être compris entre 1kg et 300kg")]
        [Required(ErrorMessage = "La taille est obligatoire")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Le poids doit être un nombre valide.")]
        public float? Poids { get; set; }

        [Required(ErrorMessage = "Le sexe est obligatoire")]
        public Sexe? Sexe { get; set; }

        [Display(Name = "Date de naissance")]
        [Required(ErrorMessage = "La date de naissance est obligatoire")]
        [DataType(DataType.Date, ErrorMessage = "La date de naissance n'est pas valide.")]
        public DateTime? DateNaissance { get; set; }

        [Required(ErrorMessage = "L'adresse est obligatoire")]
        [StringLength(100, ErrorMessage = "L'adresse ne peut pas dépasser 100 caractères.")]
        public string? Adresse { get; set; }

        [Display(Name = "Numéro de sécurité sociale")]
        [Required(ErrorMessage = "Le numéro de sécurité sociale est obligatoire")]
        public string? NuméroSécuritéSociale { get; set; }

        [Required(ErrorMessage = "La ville est obligatoire")]
        [StringLength(50, ErrorMessage = "La ville ne peut pas dépasser 50 caractères.")]
        public string? Ville { get; set; }

        [MaxLength(1048576, ErrorMessage = "La taille de la photo ne doit pas dépasser 1 Mo.")]
        public byte[]? Photo { get; set; }

        public string? PhotoBase64 { get; set; }

        public List<Antecedent> Antecedents { get; set; } = new();
        public List<Allergie> Allergies { get; set; } = new();
        public List<int>? AntecedentIdSelectionnes { get; set; } = new();
        public List<int>? AllergieIdSelectionnes { get; set; } = new();
    }
}
