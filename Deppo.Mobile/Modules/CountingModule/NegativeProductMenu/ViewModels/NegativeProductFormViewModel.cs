using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.InCountingTransaction;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Data.Async.Helpers;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.ViewModels;


[QueryProperty(name: nameof(NegativeProductBasketModel), queryId: nameof(NegativeProductBasketModel))]
public partial class NegativeProductFormViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;
    private readonly IInCountingTransactionService _inCountingTransactionService;
    private readonly IHttpClientService _httpClientService;


    [ObservableProperty]
    ObservableCollection<NegativeProductBasketModel> negativeProductBasketModel = null!;

    [ObservableProperty]
    NegativeProductBasketModel? selectedNegativeProductBasketModel;

    public ObservableCollection<NegativeProductModel> NegativeProductItems { get; } = new();


	[ObservableProperty]
    DateTime transactionDate = DateTime.Now;
    [ObservableProperty]
    string description = string.Empty;
    [ObservableProperty]
    string documentNumber = string.Empty;
    [ObservableProperty]
    string documentTrackingNumber = string.Empty;
    [ObservableProperty]
    string specialCode = string.Empty;

	public NegativeProductFormViewModel(IUserDialogs userDialogs, IInCountingTransactionService inCountingTransactionService, IHttpClientService httpClientService)
	{
		_userDialogs = userDialogs;
		_inCountingTransactionService = inCountingTransactionService;
		_httpClientService = httpClientService;

		Title = "Form";

		ItemTappedCommand = new Command<NegativeProductBasketModel>(async (item) => await ItemTappedAsync(item));
        SaveCommand = new Command(async () => await SaveAsync());
		BottomSheetCloseCommand = new Command(async () => await BottomSheetCloseAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	public Page CurrentPage { get; set; } = null!;

    public Command<NegativeProductBasketModel> ItemTappedCommand { get; }
    public Command SaveCommand { get; }
	public Command BottomSheetCloseCommand { get; }
	public Command BackCommand { get; }

    private async Task ItemTappedAsync(NegativeProductBasketModel basketModel)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            SelectedNegativeProductBasketModel = basketModel;
            await LoadItemsAsync();

            CurrentPage.FindByName<BottomSheet>("productList").State = BottomSheetState.HalfExpanded; 
        }
        catch (Exception ex)
        {
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadItemsAsync()
    {
        try
        {
            NegativeProductItems.Clear();

            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);

			foreach (var item in SelectedNegativeProductBasketModel.NegativeProducts)
            {
                NegativeProductItems.Add(item);
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task SaveAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("İşlem tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            foreach(var basketModel in NegativeProductBasketModel)
            {
				var inCountingTransactionDto = new InCountingTransactionInsert
				{
					SpeCode = SpecialCode,
					CurrentCode = string.Empty,
					Code = string.Empty,
					DocTrackingNumber = DocumentTrackingNumber,
					DoCode = DocumentNumber,
					TransactionDate = TransactionDate,
					FirmNumber = _httpClientService.FirmNumber,
					WarehouseNumber = basketModel.NegativeWarehouseModel.WarehouseNumber,
					Description = Description,
				};

				foreach (var item in basketModel.NegativeProducts)
				{
					var inCountingTransactionLineDto = new InCountingTransactionLineDto
					{
						ProductCode = item.ProductCode,
						WarehouseNumber = basketModel.NegativeWarehouseModel.WarehouseNumber,
						Quantity = item.StockQuantity,
						ConversionFactor = 1,
						OtherConversionFactor = 1,
						SubUnitsetCode = item.SubUnitsetCode,
					};

					//foreach (var detail in item.)
					//{
					//	var seriLotTransactionDto = new SeriLotTransactionDto
					//	{
					//		StockLocationCode = detail.LocationCode,
					//		Quantity = detail.Quantity,
					//		ConversionFactor = 1,
					//		OtherConversionFactor = 1,
					//		DestinationStockLocationCode = string.Empty,
					//	};

					//	productionTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
					//}

					inCountingTransactionDto.Lines.Add(inCountingTransactionLineDto);
				}

                var result = await _inCountingTransactionService.InsertInCountingTransaction(httpClient, inCountingTransactionDto, _httpClientService.FirmNumber);

                ResultModel resultModel = new();
               
				if (result.IsSuccess)
				{
					resultModel.Message = "Başarılı";
					resultModel.Code = result.Data.Code;
					resultModel.PageTitle = "Ambar Sayım fişi";
					resultModel.PageCountToBack = 3;

					await ClearFormAsync();

					if (_userDialogs.IsHudShowing)
						_userDialogs.HideHud();

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
					resultModel.PageCountToBack = 1;
					await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
					{
						[nameof(ResultModel)] = resultModel
					});
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task BottomSheetCloseAsync()
    {
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("productList").State = BottomSheetState.Hidden;
		});
	}

	private async Task ClearFormAsync()
	{
		try
		{
			SpecialCode = string.Empty;
			DocumentNumber = string.Empty;
			DocumentTrackingNumber = string.Empty;
			Description = string.Empty;
			TransactionDate = DateTime.Now;
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
			
		}
	}

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("Form verileri silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;

			await ClearFormAsync();

			await Shell.Current.GoToAsync("..");
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
}
