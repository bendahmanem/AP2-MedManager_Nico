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
        public required List<Medicament> Medicaments { get; set; } = new List<Medicament>();


        public byte[]? Pdf { get; set; }

        public int? PatientId { get; set; }
        public required string MedecinId { get; set; }
        
        public Patient? Patient { get; set; }

        public Medecin Medecin { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateCreation { get; set; } = DateTime.Now;

    }

}
