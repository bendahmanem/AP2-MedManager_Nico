using MedManager.Models;
using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel.OrdonnanceVM
{
    public class OrdonnanceViewModel
    {
        public int? OrdonnanceId { get; set; }

        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Le date de fin est obligatoire")]
        public DateTime DateDebut { get; set; }

        [Display(Name ="Date de fin")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Le date de fin est obligatoire")]
        public DateTime DateFin { get; set; }

        public string? InfoSupplementaire { get; set; }

        [Required(ErrorMessage = "Les médicaments sont obligatoires")]
        public required List<Medicament> Medicaments { get; set; }
        public List<Medicament>? MedicamentIdSelectionnes { get; set; } = new List<Medicament>();

        public byte[]? Pdf { get; set; }

        public int? PatientId { get; set; }

        public required Medecin Medecin { get; set; }

    }
}


