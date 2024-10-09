using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.InCountingTransaction;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.ResultModule;
using DevExpress.Maui.Controls;
using static Android.Util.EventLogTags;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Deppo.Core.DTOs.Demand;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    [QueryProperty(name: nameof(Items), queryId: nameof(Items))]
    public partial class DemandProcessFormViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDemandService _demandService;
        private readonly IUserDialogs _userDialogs;


        [ObservableProperty]
        WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        DateTime ficheDate = DateTime.Now;

        [ObservableProperty]
        string documentNumber = string.Empty;

        [ObservableProperty]
        string documentTrackingNumber = string.Empty;

        [ObservableProperty]
        string specialCode = string.Empty;

        [ObservableProperty]
        string description = string.Empty;

        [ObservableProperty]
        ObservableCollection<DemandProcessBasketModel> items = null!;


        public DemandProcessFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs,IDemandService demandService, IServiceProvider serviceProvider)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _demandService = demandService;
            _serviceProvider = serviceProvider;
            Items = new();

            LoadPageCommand = new Command(async () => await LoadPageAsync());
            ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
            SaveCommand = new Command(async () => await SaveAsync());
            BackCommand = new Command(async () => await BackAsync());
        }

        public Page CurrentPage { get; set; }

        public Command LoadPageCommand { get; }
        public Command BackCommand { get; }
        public Command SaveCommand { get; }
        public Command ShowBasketItemCommand { get; }

        private async Task ShowBasketItemAsync()
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

                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
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

                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        private async Task SaveAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var confirm = await _userDialogs.ConfirmAsync("Fiş oluşturulacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!confirm)
                    return;

                _userDialogs.Loading("İşlem tamamlanıyor");
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                await DemandInsertAsync(httpClient);

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DemandInsertAsync(HttpClient httpClient)
        {
            var dto = new DemandInsert
            {
                Date = FicheDate,
                DocumentNumber = DocumentNumber,
                WarehouseNumber = (short)WarehouseModel.Number,
                ProjectCode = string.Empty,
                
            };

            foreach (var item in Items)
            {
                var lineDto = new DemandLineDto();
                if (item.IsVariant)
                {
                    lineDto.VariantCode = item.ItemCode;
                    lineDto.ProductCode = item.MainItemCode;
                }
                else
                {
                    lineDto.VariantCode = string.Empty;
                    lineDto.ProductCode = item.ItemCode;
                }
                lineDto.Description = Description;
                lineDto.Quantity = item.Quantity;
                lineDto.Unitset = item.SubUnitsetCode;

                dto.Lines.Add(lineDto);
            }

            var result = await _demandService.DemandInsert(httpClient, dto, _httpClientService.FirmNumber);

            ResultModel resultModel = new();
            if (result.IsSuccess)
            {
                resultModel.Message = "Başarılı";
                resultModel.Code = result.Data.Code;
                resultModel.PageTitle = Title;
                resultModel.IsSuccess = true;
                resultModel.PageCountToBack = 4;

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                var basketViewModel = _serviceProvider.GetRequiredService<DemandProcessBasketListViewModel>();
                basketViewModel.Items.Clear();

                await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
            }
            else
            {

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                resultModel.Message = "Başarısız";
                resultModel.PageTitle = Title;
                resultModel.ErrorMessage = result.Message;
                resultModel.IsSuccess = false;
                resultModel.PageCountToBack = 1;
                await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
            }
        }

        private async Task BackAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var confirm = await _userDialogs.ConfirmAsync("Form verileriniz silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!confirm)
                    return;

                DocumentNumber = string.Empty;
                DocumentTrackingNumber = string.Empty;
                Description = string.Empty;
                SpecialCode = string.Empty;
                FicheDate = DateTime.Now;

                await Shell.Current.GoToAsync("..");
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
    }
}
