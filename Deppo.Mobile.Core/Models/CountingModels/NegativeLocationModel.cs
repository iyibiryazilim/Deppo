using CommunityToolkit.Mvvm.ComponentModel;
namespace Deppo.Mobile.Core.Models.CountingModels
{
    public partial class NegativeLocationModel : ObservableObject
    {
        [ObservableProperty]
        public int referenceId;

        [ObservableProperty]
        public string code = string.Empty;

        [ObservableProperty]
        public string name = string.Empty;

        [ObservableProperty]
        public double quantity;
    }
}
