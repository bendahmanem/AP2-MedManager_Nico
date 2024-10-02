using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    
    public List<Medicament> Medicamennts { get; set; } 
    public Patient Patient { get; set; }

}
