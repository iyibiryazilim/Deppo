using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.ViewModels
{
    [QueryProperty(name: nameof(QuicklyBomProductBasketModel), queryId: nameof(QuicklyBomProductBasketModel))]
    public partial class WorkOrderFormViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IHttpClientService _httpClientService;

        [ObservableProperty]
        private QuicklyBomProductBasketModel quicklyBomProductBasketModel = null!;

        [ObservableProperty]
        private DateTime ficheDate = DateTime.Now;

        [ObservableProperty]
        private string documentNumber = string.Empty;

        [ObservableProperty]
        private string documentTrackingNumber = string.Empty;

        [ObservableProperty]
        private string specialCode = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        public WorkOrderFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
        {
            Title = "Form Sayfası";

            _httpClientService = httpClientService;
            _userDialogs = userDialogs;

            LoadPageCommand = new Command(async () => await LoadPageAsync());
            ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
            SaveCommand = new Command(async () => await SaveAsync());
        }

        public Page CurrentPage { get; set; }

        public Command LoadPageCommand { get; }
        public Command BackCommand { get; }
        public Command SaveCommand { get; }
        public Command ShowBasketItemCommand { get; }

        public async Task ShowBasketItemAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadPageAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveAsync()
        {
        }
    }
}