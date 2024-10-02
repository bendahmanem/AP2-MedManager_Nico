using System.ComponentModel.DataAnnotations;


public class Medecin
{
    public int MedecinId { get; set; }
    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(50, MinimumLength = 2)]
    public string Nom { get; set; }
    [StringLength(50, MinimumLength = 2)]
    [Required(ErrorMessage = "Le prénom est obligatoire")]
    public string Prenom { get; set; }
	public string? Ville { get; set; }
    [StringLength(50, MinimumLength = 2)]
    [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
    public string Utilisateur { get; set; }
    [DataType(Datatype.Password)]
    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    public string MotDePasse { get; set; }	
    public List<Patient> Patients { get; set; }
    public List<Ordonnance> Ordonnances { get; set; }
}
