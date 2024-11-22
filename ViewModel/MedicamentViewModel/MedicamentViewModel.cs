using MedManager.Models;

namespace MedManager.ViewModel.MedicamentViewModel
{
    public class MedicamentViewModel
    {
        public required List<Medicament> Medicaments { get; set; }
        public string? FiltreCateActuel { get; set; }
    }
}
