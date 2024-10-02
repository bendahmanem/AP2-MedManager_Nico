using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Medecin
{
    public int MedecinId { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères.")]
    public string Nom { get; set; }

    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 50 caractères.")]
    public string Prenom { get; set; }

    [StringLength(100, ErrorMessage = "La ville ne peut pas dépasser 100 caractères.")]
    public string? Ville { get; set; }

    [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom d'utilisateur doit contenir entre 2 et 50 caractères.")]
    [EmailAddress(ErrorMessage = "Veuillez entrer une adresse e-mail valide.")]
    public string Utilisateur { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
        ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères, incluant une lettre, un chiffre et un caractère spécial.")]
    public string MotDePasse { get; set; }

    public List<Patient> Patients { get; set; } = new List<Patient>();

    public List<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
}
