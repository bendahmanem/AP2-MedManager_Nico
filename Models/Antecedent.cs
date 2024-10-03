using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MedManager.Models
{
    public class Antecedent
    {
        public int AntecedentId { get; set; }
        [Required(ErrorMessage = "Le nom de l'allergie est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères ")]
        public required string Nom { get; set; }
        public List<Patient> Patients { get; set; } = new();
        public List<Medicament> Medicaments { get; set; } = new List<Medicament>();
    }
}

