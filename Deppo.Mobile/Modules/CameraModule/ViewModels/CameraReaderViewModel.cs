using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace Deppo.Mobile.Modules.CameraModule.ViewModels;


[QueryProperty(name: nameof(ComingPage), queryId: nameof(ComingPage))]
public partial class CameraReaderViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;
	private readonly IHttpClientService _httpClientService;
	private readonly IWarehouseTotalService _warehouseTotalService;
	private readonly IProductService _productService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IBarcodeSearchService _barcodeSearchService;

	[ObservableProperty]
	string comingPage = null!;

	bool isFind = false;
	public CameraReaderViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IProductService productService, IServiceProvider serviceProvider, IBarcodeSearchService barcodeSearchService)
	{
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_warehouseTotalService = warehouseTotalService;
		_productService = productService;
		_serviceProvider = serviceProvider;
		_barcodeSearchService = barcodeSearchService;

		BackCommand = new Command(async () => await BackAsync());
		CameraDetectedCommand = new Command<BarcodeDetectionEventArgs>(async (e) => await CameraDetectedAsync(e));
		SwitchCameraTappedCommand = new Command(async () => await SwitchCameraTappedAsync());
		FlashlightTappedCommand = new Command(async () => await FlashlightTappedAsync());
	
	
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_warehouseTotalService = warehouseTotalService;
		_productService = productService;
		_serviceProvider = serviceProvider;

		BackCommand = new Command(async () => await BackAsync());
		CameraDetectedCommand = new Command<BarcodeDetectionEventArgs>(async (e) => await CameraDetectedAsync(e));
		SwitchCameraTappedCommand = new Command(async () => await SwitchCameraTappedAsync());
		FlashlightTappedCommand = new Command(async () => await FlashlightTappedAsync());
	}

	public CameraBarcodeReaderView BarcodeReader { get; set; } = null!;

	public Command BackCommand { get; }
	public Command CameraDetectedCommand { get; }
	public Command SwitchCameraTappedCommand { get; }
	public Command FlashlightTappedCommand { get; }

	private async Task SearchBarcodeAsync(BarcodeResult[] readBarcodes) {
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var firmNumber = _httpClientService.FirmNumber;
			var periodNumber = _httpClientService.PeriodNumber;

			for(int i = 0; i < readBarcodes.Length; i++) {
				var first = readBarcodes[i];

				if(first is null) {
					await _userDialogs.AlertAsync("Barkod Bulunamadı", "Hata", "Tamam");
					return;
				}
				Task searchByProductCodeTask = SearchByProductCodeAsync(httpClient, firmNumber, periodNumber, first.Value);


				await Task.WhenAll(
					searchByProductCodeTask

				);
			
			}
		}
		catch (System.Exception)
		{
			
			throw;
		}
	}

	private async Task ReadBarcodeAsync(BarcodeResult[] readBarcodes)
	{
		try
		{
			for (int i = 0; i < readBarcodes.Length; i++)
			{
				var first = readBarcodes[i];

				if (first is null)
				{
					await _userDialogs.AlertAsync("Barkod Bulunamadı", "Hata", "Tamam");
					return;
				}
				switch (ComingPage)
				{
					case "InputProductProcessBasket": // Üretimden Giriş, Sayım Fazlası
						await FindProductInputProductProcessBasketListAsync(first.Value);
						break;
					case "OutputProductProcessBasket": // Sarf, Sayım Eksiği, Fire Fişi
						await FindProductOutputProductProcessBasketListAsync(first.Value);
						break;
					case "TransferOutBasket": // Ambar Transferi
						await FindProductTransferOutBasketListAsync(first.Value);
						break;
					case "OutputProductSalesProcessBasket": // Sevk İşlemi
						await FindProductOutputProductSalesProcessBasketListAsync(first.Value);
						break;
					case "InputProductPurchaseProcessBasket": // Mal Kabul İşlemi
						await FindProductInputProductPurchaseProcessBasketListAsync(first.Value);
						break;
					case "ReturnPurchaseBasket": // Satınalma İade İşlemi
						await FindProductReturnPurchaseBasketListAsync(first.Value);
						break;
					case "ReturnSalesBasket": // Satış İade İşlemi
						await FindProductReturnSalesBasketListAsync(first.Value);
						break;
				}
			}
		}
		catch (Exception)
		{

			throw;
		}
	}

	private async Task CameraDetectedAsync(BarcodeDetectionEventArgs e)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await ReadBarcodeAsync(e.Results);
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

	private async Task FindProductInputProductProcessBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var viewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>();

			var result = await _productService.GetObjects(
				httpClient,
				_httpClientService.FirmNumber,
				_httpClientService.PeriodNumber,
				barcodeValue,
				0,
				1
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
				{
					await _userDialogs.AlertAsync("Bir hata oluştu", "Hata", "Tamam");
					return;
				}
				if (!(result.Data.Count() > 0))
				{
					_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
					return;
				}

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<Product>(product);

					ProductModel productModel = new ProductModel
					{
						ReferenceId = item.ReferenceId,
						Code = item.Code,
						Name = item.Name,
						UnitsetReferenceId = item.UnitsetReferenceId,
						UnitsetCode = item.UnitsetCode,
						UnitsetName = item.UnitsetName,
						SubUnitsetReferenceId = item.SubUnitsetReferenceId,
						SubUnitsetCode = item.SubUnitsetCode,
						SubUnitsetName = item.SubUnitsetName,
						StockQuantity = item.StockQuantity,
						TrackingType = item.TrackingType,
						LocTracking = item.LocTracking,
						GroupCode = item.GroupCode,
						BrandReferenceId = item.BrandReferenceId,
						BrandCode = item.BrandCode,
						BrandName = item.BrandName,
						VatRate = item.VatRate,
						Image = item.Image,
						IsVariant = item.IsVariant,
						IsSelected = false
					};

					var basketItem = new InputProductBasketModel
					{
						ItemReferenceId = productModel.ReferenceId,
						ItemCode = productModel.Code,
						ItemName = productModel.Name,
						UnitsetReferenceId = productModel.UnitsetReferenceId,
						UnitsetCode = productModel.UnitsetCode,
						UnitsetName = productModel.UnitsetName,
						SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
						SubUnitsetCode = productModel.SubUnitsetCode,
						SubUnitsetName = productModel.SubUnitsetName,
						IsSelected = false,
						MainItemCode = string.Empty,
						MainItemName = string.Empty,
						MainItemReferenceId = default,
						StockQuantity = productModel.StockQuantity,
						Quantity = productModel.LocTracking == 0 ? 1 : 0,
						LocTracking = productModel.LocTracking,
						TrackingType = productModel.TrackingType,
						IsVariant = productModel.IsVariant,
						VariantIcon = productModel.VariantIcon,
						LocTrackingIcon = productModel.LocTrackingIcon,
						TrackingTypeIcon = productModel.TrackingTypeIcon
					};

					if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
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
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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

			if (result.IsSuccess)
			{
				if (result.Data is not null)
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
						};

						var basketItem = new OutputProductBasketModel()
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
							Quantity = warehouseTotalModel.LocTracking == 0 ? 1 : 0
						};

						if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task FindProductTransferOutBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var viewModel = _serviceProvider.GetRequiredService<TransferOutBasketViewModel>();

			var result = await _warehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: viewModel.TransferBasketModel.OutWarehouse.Number,
				search: barcodeValue,
				skip: 0,
				take: 1);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach (var product in result.Data)
					{
						var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);


						var basketItem = new OutProductModel()
						{
							ReferenceId = item.ProductReferenceId,
							Code = item.ProductCode,
							Name = item.ProductName,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							StockQuantity = item.StockQuantity,
							IsVariant = item.IsVariant,
							LocTracking = item.LocTracking,
							TrackingType = item.TrackingType,
							LocTrackingIcon = item.LocTrackingIcon,
							VariantIcon = item.VariantIcon,
							TrackingTypeIcon = item.TrackingTypeIcon,
							OutputQuantity = item.LocTracking == 0 ? 1 : 0,
							IsSelected = item.IsSelected,
						};

						if (viewModel.TransferBasketModel.OutProducts.Any(x => x.Code == basketItem.Code))
						{
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepette Zaten Var");
						}
						else
						{
							viewModel.TransferBasketModel.OutProducts.Add(basketItem);
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepete Eklendi");
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task FindProductOutputProductSalesProcessBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var viewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();

			var result = await _warehouseTotalService.GetObjects(
				httpClient,
				_httpClientService.FirmNumber,
				_httpClientService.PeriodNumber,
				warehouseNumber: viewModel.WarehouseModel.Number,
				search: barcodeValue,
				skip: 0,
				take: 1
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach (var product in result.Data)
					{
						var item = new WarehouseTotalModel
						{
							ProductReferenceId = product.ProductReferenceId,
							ProductCode = product.ProductCode,
							ProductName = product.ProductName,
							UnitsetReferenceId = product.UnitsetReferenceId,
							UnitsetCode = product.UnitsetCode,
							UnitsetName = product.UnitsetName,
							SubUnitsetReferenceId = product.SubUnitsetReferenceId,
							SubUnitsetCode = product.SubUnitsetCode,
							SubUnitsetName = product.SubUnitsetName,
							StockQuantity = product.StockQuantity,
							WarehouseReferenceId = product.WarehouseReferenceId,
							WarehouseName = product.WarehouseName,
							WarehouseNumber = product.WarehouseNumber,
							LocTracking = product.LocTracking,
							IsVariant = product.IsVariant,
							TrackingType = product.TrackingType,
							IsSelected = false,
							LocTrackingIcon = product.LocTrackingIcon,
							VariantIcon = product.VariantIcon,
							TrackingTypeIcon = product.TrackingTypeIcon,
						};

						var basketItem = new OutputSalesBasketModel
						{
							ItemReferenceId = item.ProductReferenceId,
							ItemCode = item.ProductCode,
							ItemName = item.ProductName,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							MainItemReferenceId = default,  //
							MainItemCode = string.Empty,    //
							MainItemName = string.Empty,    //
							StockQuantity = item.StockQuantity,
							IsSelected = false,   //
							IsVariant = item.IsVariant,
							LocTracking = item.LocTracking,
							TrackingType = item.TrackingType,
							Quantity = item.LocTracking == 0 ? 1 : 0,
							LocTrackingIcon = item.LocTrackingIcon,
							VariantIcon = item.VariantIcon,
							TrackingTypeIcon = item.TrackingTypeIcon,
						};

						if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task FindProductInputProductPurchaseProcessBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var viewModel = _serviceProvider.GetRequiredService<InputProductPurchaseProcessBasketListViewModel>();

			var result = await _productService.GetObjectsPurchaseProduct(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: barcodeValue,
				skip: 0,
				take: 1
			);

			if(result.IsSuccess)
			{
				if(result.Data is not null)
				{
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach(var product in result.Data)
					{
						var item = Mapping.Mapper.Map<Product>(product);

						ProductModel productModel = new ProductModel
						{
							ReferenceId = item.ReferenceId,
							Code = item.Code,
							Name = item.Name,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							StockQuantity = item.StockQuantity,
							TrackingType = item.TrackingType,
							LocTracking = item.LocTracking,
							IsVariant = item.IsVariant,
							IsSelected = false
						};

						var basketItem = new InputPurchaseBasketModel
						{

							ItemReferenceId = productModel.ReferenceId,
							ItemCode = productModel.Code,
							ItemName = productModel.Name,
							UnitsetReferenceId = productModel.UnitsetReferenceId,
							UnitsetCode = productModel.UnitsetCode,
							UnitsetName = productModel.UnitsetName,
							SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
							SubUnitsetCode = productModel.SubUnitsetCode,
							SubUnitsetName = productModel.SubUnitsetName,
							IsSelected = false,
							MainItemCode = string.Empty,
							MainItemName = string.Empty,
							MainItemReferenceId = default,
							StockQuantity = productModel.StockQuantity,
							Quantity = productModel.LocTracking == 0 ? 1 : 0,
							InputQuantity = productModel.LocTracking == 0 ? 1 : 0,
							LocTracking = productModel.LocTracking,
							TrackingType = productModel.TrackingType,
							IsVariant = productModel.IsVariant
						};

						if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
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

	private async Task FindProductReturnPurchaseBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var viewModel = _serviceProvider.GetRequiredService<ReturnPurchaseBasketViewModel>();

			var result = await _warehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: viewModel.WarehouseModel.Number,
				search: barcodeValue,
				skip: 0,
				take: 1
			);

			if(result.IsSuccess)
			{
				if(result.Data is not null)
				{
					if(!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach(var product in result.Data)
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

						var basketItem = new ReturnPurchaseBasketModel
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

						if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		

	}
	private async Task FindProductReturnSalesBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var viewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();

			var result = await _productService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: barcodeValue,
				skip: 0,
				take: 1
			);

			if(result.IsSuccess)
			{
				if(result.Data is not null)
				{
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach(var item in result.Data)
					{
						ProductModel productModel = new ProductModel
						{
							IsSelected = false,
							BrandCode = item.BrandCode,
							BrandName = item.BrandName,
							BrandReferenceId = item.BrandReferenceId,
							Code = item.Code,
							Image = item.Image,
							IsVariant = item.IsVariant,
							LocTracking = item.LocTracking,
							Name = item.Name,
							ReferenceId = item.ReferenceId,
							StockQuantity = item.StockQuantity,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							TrackingType = item.TrackingType,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							UnitsetReferenceId = item.UnitsetReferenceId,
							VatRate = item.VatRate,
							GroupCode = item.GroupCode,
							LocTrackingIcon = item.LocTrackingIcon,
							VariantIcon = item.VariantIcon,
							TrackingTypeIcon = item.TrackingTypeIcon,
						};

						ReturnSalesBasketModel basketItem = new ReturnSalesBasketModel
						{
							ItemReferenceId = productModel.ReferenceId,
							ItemCode = productModel.Code,
							ItemName = productModel.Name,
							UnitsetReferenceId = productModel.UnitsetReferenceId,
							UnitsetCode = productModel.UnitsetCode,
							UnitsetName = productModel.UnitsetName,
							SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
							SubUnitsetCode = productModel.SubUnitsetCode,
							SubUnitsetName = productModel.SubUnitsetName,
							IsSelected = false,
							MainItemCode = string.Empty,
							MainItemName = string.Empty,
							MainItemReferenceId = default,
							StockQuantity = productModel.StockQuantity,
							Quantity = productModel.LocTracking == 0 ? 1 : 0,
							InputQuantity = productModel.LocTracking == 0 ? 1 : 0,
							LocTracking = productModel.LocTracking,
							TrackingType = productModel.TrackingType,
							IsVariant = productModel.IsVariant,
							LocTrackingIcon = productModel.LocTrackingIcon,
							VariantIcon = productModel.VariantIcon,
							TrackingTypeIcon = productModel.TrackingTypeIcon,
							Image = productModel.Image,
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

	private async Task FindProductReturnPurchaseDispatchBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var viewModel = _serviceProvider.GetRequiredService<ReturnPurchaseDispatchBasketViewModel>();

		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
			{
				_userDialogs.HideHud();
			}

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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task SwitchCameraTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			BarcodeReader.CameraLocation = BarcodeReader.CameraLocation == CameraLocation.Rear ? CameraLocation.Front : CameraLocation.Rear;
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

	private async Task FlashlightTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			BarcodeReader.IsTorchOn = !BarcodeReader.IsTorchOn;
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

	private async Task SearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			

			if(!isFind){
				var result = await _barcodeSearchService.SearchByProductCode(httpClient, firmNumber, periodNumber, barcode);

				if(result.IsSuccess) {
					if(result.Data is not null) {
						var item = Mapping.Mapper.Map<ProductModel>(result.Data);
						if(item is not null)
						{
							isFind = true;
							await SendProductBasketPageAsync(item);
						}
						
					}


				} 
			}
		}
		catch (System.Exception)
		{
			
			throw;
		}
	}

	private async Task SendProductBasketPageAsync(ProductModel productModel)
	{
		try
		{
			switch (ComingPage)
			{
				case "InputProductProcessBasket":
					var inputProductProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>();
					var basketItem = await ConvertInputProductProcessBasketAsync(productModel);
					inputProductProcessBasketListViewModel.Items.Add(basketItem);
					break;
				case "OutputProductProcessBasket":
					var outputProductProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();
					var outputProductBasketItem = await ConvertOutputProductProcessBasketAsync(productModel);
					outputProductProcessBasketListViewModel.Items.Add(outputProductBasketItem);
					break;
				case "TransferOutBasket":
					var transferOutBasketViewModel = _serviceProvider.GetRequiredService<TransferOutBasketViewModel>();
					//await transferOutBasketViewModel.AddProductToBasketAsync(productModel);
					break;
				case "OutputProductSalesProcessBasket":
					var outputProductSalesProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();
					//await outputProductSalesProcessBasketListViewModel.AddProductToBasketAsync(productModel);
					break;
				case "InputProductPurchaseProcessBasket":
					var inputProductPurchaseProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseProcessBasketListViewModel>();
					//await inputProductPurchaseProcessBasketListViewModel.AddProductToBasketAsync(productModel);
					break;
				case "ReturnPurchaseBasket":
					var returnPurchaseBasketViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseBasketViewModel>();
					//await returnPurchaseBasketViewModel.AddProductToBasketAsync(productModel);
					break;
				case "ReturnSalesBasket":
					var returnSalesBasketViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();
					//await returnSalesBasketViewModel.AddProductToBasketAsync(productModel);
					break;
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task<InputProductBasketModel> ConvertInputProductProcessBasketAsync(ProductModel productModel) {
		
		try
		{
			return await Task.Run(() => {

					var basketItem = new InputProductBasketModel
					{
						ItemReferenceId = productModel.ReferenceId,
						ItemCode = productModel.Code,
						ItemName = productModel.Name,
						UnitsetReferenceId = productModel.UnitsetReferenceId,
						UnitsetCode = productModel.UnitsetCode,
						UnitsetName = productModel.UnitsetName,
						SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
						SubUnitsetCode = productModel.SubUnitsetCode,
						SubUnitsetName = productModel.SubUnitsetName,
						IsSelected = false,
						MainItemCode = string.Empty,
						MainItemName = string.Empty,
						MainItemReferenceId = default,
						StockQuantity = productModel.StockQuantity,
						Quantity = productModel.LocTracking == 0 ? 1 : 0,
						LocTracking = productModel.LocTracking,
						TrackingType = productModel.TrackingType,
						IsVariant = productModel.IsVariant,
						VariantIcon = productModel.VariantIcon,
						LocTrackingIcon = productModel.LocTrackingIcon,
						TrackingTypeIcon = productModel.TrackingTypeIcon
					};

				return basketItem;
			});
			
			

		}
		catch (System.Exception)
		{
			
			throw;
		}

	} 

	private async Task<OutputProductBasketModel> ConvertOutputProductProcessBasketAsync(ProductModel productModel) {
		
		try
		{
			return await Task.Run(() => {

					var basketItem = new OutputProductBasketModel
					{
						ItemReferenceId = productModel.ReferenceId,
						ItemCode = productModel.Code,
						ItemName = productModel.Name,
						UnitsetReferenceId = productModel.UnitsetReferenceId,
						UnitsetCode = productModel.UnitsetCode,
						UnitsetName = productModel.UnitsetName,
						SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
						SubUnitsetCode = productModel.SubUnitsetCode,
						SubUnitsetName = productModel.SubUnitsetName,
						IsSelected = false,
						MainItemCode = string.Empty,
						MainItemName = string.Empty,
						MainItemReferenceId = default,
						StockQuantity = productModel.StockQuantity,
						Quantity = productModel.LocTracking == 0 ? 1 : 0,
						LocTracking = productModel.LocTracking,
						TrackingType = productModel.TrackingType,
						IsVariant = productModel.IsVariant,
						VariantIcon = productModel.VariantIcon,
						LocTrackingIcon = productModel.LocTrackingIcon,
						TrackingTypeIcon = productModel.TrackingTypeIcon
					};

				return basketItem;
			});
			
			

		}
		catch (System.Exception)
		{
			
			throw;
		}

	} 		
}
