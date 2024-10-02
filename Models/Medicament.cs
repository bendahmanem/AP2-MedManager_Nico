using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Medicament
{
    public int MedicamentId { get; set; }

    [Required(ErrorMessage = "Le nom du médicament est obligatoire")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom du médicament doit contenir entre 2 et 100 caractères.")]
    public string Nom { get; set; }

    [Required(ErrorMessage = "La quantité est obligatoire")]
    public int Quantite { get; set; }

    [Required(ErrorMessage = "La posologie est obligatoire")]
    [StringLength(200, ErrorMessage = "La posologie ne peut pas dépasser 200 caractères.")]
    public string Posologie { get; set; }

    [StringLength(500, ErrorMessage = "La composition ne peut pas dépasser 500 caractères.")]
    public string Composition { get; set; }

    public List<Allergie> Allergie { get; set; } = new List<Allergie>();

    public List<Antecedent> Antecedents { get; set; } = new List<Antecedent>();
}
