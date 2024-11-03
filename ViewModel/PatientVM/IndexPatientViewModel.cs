using MedManager.Models;
using X.PagedList;


namespace MedManager.ViewModel.PatientVM
{
	public class IndexPatientViewModel
	{
		public IPagedList<Patient> Patients { get; set; } = PagedList<Patient>.Empty();
	}
}
