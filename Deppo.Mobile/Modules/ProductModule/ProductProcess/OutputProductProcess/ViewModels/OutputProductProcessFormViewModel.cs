using Android.Net.Wifi;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

[QueryProperty(name: nameof(OutputProductProcessType), queryId: nameof(OutputProductProcessType))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class OutputProductProcessFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IConsumableTransactionService _consumableTransactionService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	OutputProductProcessType outputProductProcessType;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	ObservableCollection<OutputProductBasketModel> items = null!;


	[ObservableProperty]
	string documentNumber = string.Empty;

	[ObservableProperty]
	DateTime transactionDate = DateTime.Now;

	[ObservableProperty]
	string description = string.Empty;

	[ObservableProperty]
	string documentTrackingNumber = string.Empty;

	[ObservableProperty]
	string specialCode = string.Empty;


	public OutputProductProcessFormViewModel(IHttpClientService httpClientService, IConsumableTransactionService consumableTransactionService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_consumableTransactionService = consumableTransactionService;
		_userDialogs = userDialogs;
		Items = new();
		SaveCommand = new Command(async () => await SaveAsync());
		LoadPageCommand = new Command(async () => await LoadPageAsync());
	}

	public Command SaveCommand { get; }
	public Command LoadPageCommand { get; }

	private async Task LoadPageAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			Title = GetEnumDescription(OutputProductProcessType);
			//CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;


		}
		catch (System.Exception)
		{

			throw;
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

			_userDialogs.ShowLoading("İşlem Tamamlanıyor...");
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var consumableTransactionDto = new ConsumableTransactionInsert
			{
				Code = "",
				CurrentCode = "",
				Description = Description,
				DoCode = DocumentNumber,
				DocTrackingNumber = DocumentTrackingNumber,
				TransactionDate = TransactionDate,
				FirmNumber = _httpClientService.FirmNumber,
				SpeCode = SpecialCode,
				WarehouseNumber = WarehouseModel.Number,
				
			};

			foreach(var item in Items)
			{
				var consumableTransactionLineDto = new ConsumableTransactionLineDto
				{
					ProductCode = item.ItemCode,
					WarehouseNumber = WarehouseModel.Number,
					Quantity = item.Quantity,
					ConversionFactor = 1,
					OtherConversionFactor  = 1,
					SubUnitsetCode = item.SubUnitsetCode,
				};

				foreach(var detail in item.Details)
				{
					var serilotTransactionDto = new SeriLotTransactionDto
					{
						StockLocationCode = detail.LocationCode,
						InProductTransactionLineReferenceId = detail.TransactionReferenceId,
						OutProductTransactionLineReferenceId = detail.ReferenceId,
						Quantity = detail.RemainingQuantity,
						SubUnitsetCode = item.SubUnitsetCode,
						DestinationStockLocationCode = string.Empty,
						ConversionFactor = 1,
						OtherConversionFactor = 1,
					};

					consumableTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
				}

				consumableTransactionDto.Lines.Add(consumableTransactionLineDto);
			}
			Console.WriteLine(consumableTransactionDto);

			var result = await _consumableTransactionService.InsertConsumableTransaction(httpClient, consumableTransactionDto, _httpClientService.FirmNumber);

			ResultModel resultModel = new();
			if (result.IsSuccess)
			{
				resultModel.Message = "Başarılı";
				resultModel.Code = result.Data.Code;
				resultModel.PageTitle = Title;
				resultModel.PageCountToBack = 4;

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


	
}
