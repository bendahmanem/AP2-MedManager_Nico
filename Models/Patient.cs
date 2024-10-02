using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Patient
{
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères.")]
    public string Nom { get; set; }

    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 50 caractères.")]
    public string Prenom { get; set; }

    [Required(ErrorMessage = "Le sexe est obligatoire")]
    [RegularExpression("^(Homme|Femme|Autre)$", ErrorMessage = "Le sexe doit être 'Homme', 'Femme', ou 'Autre'.")]
    public string Sexe { get; set; }

    [Required(ErrorMessage = "La date de naissance est obligatoire")]
    [DataType(DataType.Date, ErrorMessage = "La date de naissance n'est pas valide.")]
    public DateTime DateNaissance { get; set; }

    [Required(ErrorMessage = "L'adresse est obligatoire")]
    [StringLength(100, ErrorMessage = "L'adresse ne peut pas dépasser 100 caractères.")]
    public string Adresse { get; set; }

    [Required(ErrorMessage = "La ville est obligatoire")]
    [StringLength(50, ErrorMessage = "La ville ne peut pas dépasser 50 caractères.")]
    public string Ville { get; set; }

    public List<Allergie> Allergies { get; set; } = new List<Allergie>();

    public List<Antecedent> Antecedents { get; set; } = new List<Antecedent>();
}
