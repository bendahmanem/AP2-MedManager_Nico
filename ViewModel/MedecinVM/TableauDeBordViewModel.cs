namespace MedManager.ViewModel.MedecinVM
{
    public class TableauDeBordViewModel
    {
        public List<FrequenceAllergie>? FrequenceAllergies { get; set; }
        public List<FrequenceAntecedent>? FrequenceAntecedents { get; set; }
        public List<RepartitionAge>? RepartitionAges { get; set; }
        public List<MedicamentUtilisationViewModel>? MedicamentPlusUtilises { get; set; }
    }
}
