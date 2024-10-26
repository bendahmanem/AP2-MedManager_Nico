using MedManager.Models;
using X.PagedList;

namespace MedManager.ViewModel.OrdonnanceVM
{
	public class IndexOrdonnanceViewModel
	{
		public required Medecin medecin { get; set; }
		public IPagedList<Ordonnance> Ordonnances { get; set; } = PagedList<Ordonnance>.Empty();
	}
}
