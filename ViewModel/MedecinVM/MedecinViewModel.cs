using MedManager.Models;
using X.PagedList;


namespace MedManager.ViewModel.MedecinVM
{
    public class MedecinViewModel
    {
        public Medecin medecin { get; set; }
        public IPagedList<Patient> Patients { get; set; } 
    }

}
