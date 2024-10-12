using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace MedManager.Models
{

    public enum Specialite { Généraliste, Pédiatre, Cardiologue, Gynécologue}
    public class Medecin : IdentityUser
    {

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères.")]
        public required string Nom { get; set; }

        [Display(Name = "Prénom")]
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 50 caractères.")]
        public required string Prenom { get; set; }

        [StringLength(100, ErrorMessage = "La ville ne peut pas dépasser 100 caractères.")]
        [Required(ErrorMessage = "La ville est obligatoire")]
        public string? Ville { get; set; }

        [StringLength(100, ErrorMessage = "L'adresse ne peut pas dépasser 100 caractères.")]
        [Required(ErrorMessage = "L'adresse est obligatoire")]
        public string? Adresse { get; set; }

        [Display(Name = "Numéro de téléphone")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Le numéro de téléphone est obligatoire")]
        public string? NumTel { get; set; }

        [Display(Name = "Faculté")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "La faculté doit contenir entre 2 et 50 caractères.")]
        [Required(ErrorMessage = "La faculté est obligatoire")]
        public string? Faculte { get; set; }

        [Display(Name = "Spécialité")]
        [Required(ErrorMessage = "La spécialité est obligatoire")]
        public Specialite Specialite { get; set; }

        public List<Patient> Patients { get; set; } = new List<Patient>();

        public List<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();

        public string NomComplet
        {
            get { return $"{Prenom} {Nom}"; }
        }
    }
}

