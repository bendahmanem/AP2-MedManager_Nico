using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Medecin
{
    public int MedecinId { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(50, MinimumLength = 2)]
    public string Nom { get; set; }

    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [StringLength(50, MinimumLength = 2)]
    public string Prenom { get; set; }

    public string? Ville { get; set; }

    [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
    [StringLength(50, MinimumLength = 2)]
    public string Utilisateur { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    public string MotDePasse { get; set; }

    public List<Patient> Patients { get; set; } = new List<Patient>();

    public List<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
}
