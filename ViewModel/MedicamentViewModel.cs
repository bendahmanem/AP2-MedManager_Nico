using MedManager.Models;

namespace MedManager.ViewModel
{
	public class MedicamentViewModel
	{
		public required List<Medicament> Medicaments { get; set; }
		public string? FiltreCateActuel { get; set; }
	}
}
