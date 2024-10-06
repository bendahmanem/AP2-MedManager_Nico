using MedManager.Models;
using X.PagedList;


namespace MedManager.ViewModel.PatientVM
{
	public class IndexPatientViewModel
	{
		public required Medecin medecin { get; set; }
		public IPagedList<Patient> Patients { get; set; } = PagedList<Patient>.Empty();
	}

}
