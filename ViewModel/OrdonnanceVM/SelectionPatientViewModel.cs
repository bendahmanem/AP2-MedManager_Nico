using MedManager.Models;
using System.ComponentModel.DataAnnotations;
namespace MedManager.ViewModel.OrdonnanceVM
{
	public class SelectionPatientViewModel
	{
		public required List<Patient>? Patients { get; set; }
		[Required(ErrorMessage = "Veuillez sélectionner un patient.")]

		[Display(Name ="Patients")]
		public int? PatientId { get; set; }
	}
}
