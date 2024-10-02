using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Allergie
{
    public int AllergieId { get; set; }
    [Required(ErrorMessage ="Le nom de l'allergie est obligatoire")]
    [StringLength(50, MinimumLength =2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères ")]
    public string nom { get; set; }
    public List<Medicament> Medicaments { get; set; } = new List<Medicament>();
}
