using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MessageHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace Deppo.Mobile.Modules.CameraModule.ViewModels;

public partial class CameraReaderViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseTotalService _warehouseTotalService;
    private readonly IProductService _productService;
    private readonly IServiceProvider _serviceProvider;
	public CameraReaderViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IProductService productService, IServiceProvider serviceProvider)
	{
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_warehouseTotalService = warehouseTotalService;
		_productService = productService;
        _serviceProvider = serviceProvider;

		BackCommand = new Command(async () => await BackAsync());
		CameraDetectedCommand = new Command<BarcodeDetectionEventArgs>(async (e) => await CameraDetectedAsync(e));
	}

	public CameraBarcodeReaderView BarcodeReader { get; set; } = null!;

    public Command BackCommand { get;}
    public Command CameraDetectedCommand { get; }

    private async Task CameraDetectedAsync(BarcodeDetectionEventArgs e)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

			var first = e.Results?.FirstOrDefault();

			if (first is null)
			{
				await _userDialogs.AlertAsync("Barkod Bulunamadı", "Hata", "Tamam");
				return;
			}

            await FindProductOutputProductProcessBasketListAsync(first.Value);

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

    private async Task FindProductOutputProductProcessBasketListAsync(string barcodeValue)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();

			var viewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();

			var result = await _warehouseTotalService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: viewModel.WarehouseModel.Number,
                search: barcodeValue,
                skip: 0,
                take: 1);

            if(result.IsSuccess)
			{
				if(result.Data is not null)
                {
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach (var product in result.Data)
                    {
						var item = Mapping.Mapper.Map<WarehouseTotal>(product);
                        WarehouseTotalModel warehouseTotalModel = new WarehouseTotalModel
                        {
                            ProductReferenceId = item.ProductReferenceId,
                            ProductCode = item.ProductCode,
                            ProductName = item.ProductName,
                            UnitsetReferenceId = item.UnitsetReferenceId,
                            UnitsetCode = item.UnitsetCode,
                            UnitsetName = item.UnitsetName,
                            SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                            SubUnitsetCode = item.SubUnitsetCode,
                            SubUnitsetName = item.SubUnitsetName,
                            StockQuantity = item.StockQuantity,
                            WarehouseReferenceId = item.WarehouseReferenceId,
                            WarehouseName = item.WarehouseName,
                            WarehouseNumber = item.WarehouseNumber,
                            LocTracking = item.LocTracking,
                            IsVariant = item.IsVariant,
                            TrackingType = item.TrackingType,
                            IsSelected = false,
                            LocTrackingIcon = product.LocTrackingIcon,
                            VariantIcon = product.VariantIcon,
                            TrackingTypeIcon = product.TrackingTypeIcon,
                        };

						var basketItem = new OutputProductBasketModel
						{
							ItemReferenceId = warehouseTotalModel.ProductReferenceId,
							ItemCode = warehouseTotalModel.ProductCode,
							ItemName = warehouseTotalModel.ProductName,
							UnitsetReferenceId = warehouseTotalModel.UnitsetReferenceId,
							UnitsetCode = warehouseTotalModel.UnitsetCode,
							UnitsetName = warehouseTotalModel.UnitsetName,
							SubUnitsetReferenceId = warehouseTotalModel.SubUnitsetReferenceId,
							SubUnitsetCode = warehouseTotalModel.SubUnitsetCode,
							SubUnitsetName = warehouseTotalModel.SubUnitsetName,
							MainItemReferenceId = default,  //
							MainItemCode = string.Empty,    //
							MainItemName = string.Empty,    //
							StockQuantity = warehouseTotalModel.StockQuantity,
							IsSelected = false,   //
							IsVariant = warehouseTotalModel.IsVariant,
							LocTracking = warehouseTotalModel.LocTracking,
							TrackingType = warehouseTotalModel.TrackingType,
							Quantity = warehouseTotalModel.LocTracking == 0 ? 1 : 0,
							LocTrackingIcon = warehouseTotalModel.LocTrackingIcon,
							VariantIcon = warehouseTotalModel.VariantIcon,
							TrackingTypeIcon = warehouseTotalModel.TrackingTypeIcon,
						};

                        if(viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
                        {
                            _userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepette Zaten Var");
                        }
                        else
                        {
                            viewModel.Items.Add(basketItem);
                            _userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepete Eklendi");
                        }


					}
                }
            }
        }
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
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

            await Shell.Current.GoToAsync("..");
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
}
