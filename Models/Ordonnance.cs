using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedManager.Models
{
    public class Ordonnance
    {
        public int OrdonnanceId { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Le date de fin est obligatoire")]
        public DateTime DateDebut { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Le date de fin est obligatoire")]
        public DateTime DateFin { get; set; }

        public string? InfoSupplementaire { get; set; }

        [Required(ErrorMessage ="Les médicaments sont obligatoires")]
        public required List<Medicament> Medicaments { get; set; } 

        [Required(ErrorMessage ="Le patient est obligatoire")]

        public string? MedecinId { get; set; }
        [Required]
        public required Patient Patient { get; set; }

        [Required]
        public int PatientId { get; set; }
        public required Medecin Medecin { get; set; }

    }

}
