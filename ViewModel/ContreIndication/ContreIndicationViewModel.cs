using MedManager.Models;
using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel.ContreIndication
{
    public class ContreIndicationViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères")]
        public string Nom { get; set; }
        public string Type { get; set; }
        public List<Medicament> Medicaments { get; set; } = new List<Medicament>();

        [Display(Name = "Medicaments")]
        public List<int>? SelectedMedicamentIds { get; set; } = new List<int>();
    }
}

