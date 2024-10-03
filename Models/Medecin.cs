using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace MedManager.Models
{
    public class Medecin : IdentityUser
    {
        public int MedecinId { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères.")]
        public required string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 50 caractères.")]
        public required string Prenom { get; set; }

        [StringLength(100, ErrorMessage = "La ville ne peut pas dépasser 100 caractères.")]
        public string? Ville { get; set; }

        public List<Patient> Patients { get; set; } = new List<Patient>();

        public List<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();

        public string NomComplet
        {
            get { return $"{Prenom} {Nom}"; }
        }
    }
}

