using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;
using Newtonsoft.Json;
using ZXing.Net.Maui;

namespace Deppo.Mobile.Helpers.BarcodeHelper;

public class BarcodeSearchHelper : IBarcodeSearchHelper
{
	private readonly IBarcodeSearchService _barcodeSearchService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;

	public BarcodeSearchHelper(IBarcodeSearchService barcodeSearchService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
	{
		_barcodeSearchService = barcodeSearchService;
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;

		isFind = false;
	}

	bool isFind = false;

	public async Task BarcodeDetectedAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			Task searchByProductCodeTask = SearchByProductCodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByVariantCodeTask = SearchByVariantCodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByProductMainBarcodeTask = SearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByVariantMainBarcodeTask = SearchByVariantMainBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByProductSubBarcodeTask = SearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByVariantSubBarcodeTask = SearchByVariantSubBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByProductSeriNumberTask = SearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByVariantSeriNumberTask = SearchByVariantSeriNumberAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByProductLotNumberTask = SearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByVariantLotNumberTask = SearchByVariantLotNumberAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchBySupplierProductCodeTask = SearchBySupplierProductCodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchBySupplierVariantCodeTask = SearchBySupplierVariantCodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);

			await Task.WhenAll(
				searchByProductCodeTask,
				searchByVariantCodeTask,
				searchByProductMainBarcodeTask,
				searchByVariantMainBarcodeTask,
				searchByProductSubBarcodeTask,
				searchByVariantSubBarcodeTask,
				searchByProductSeriNumberTask,
				searchByVariantSeriNumberTask,
				searchByProductLotNumberTask,
				searchByVariantLotNumberTask,
				searchBySupplierProductCodeTask,
				searchBySupplierVariantCodeTask
			);
		}
		catch (Exception ex)
		{
			throw;
		}
		finally
		{
			
		}
	}


	public async Task SendProductBasketPageAsync(ProductModel productModel, string comingPage)
	{
		try
		{
			switch (comingPage)
			{
				case "InputProductProcessBasketListViewModel":
					var inputProductProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>();
					var inputProductBasketItem = await ConvertInputProductBasketAsync(productModel);
					if (inputProductProcessBasketListViewModel.Items.Any(x => x.ItemCode == inputProductBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductProcessBasketListViewModel.Items.Add(inputProductBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductProcessBasketListViewModel":
					var outputProductProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();
					var outputProductBasketItem = await ConvertOutputProductBasketAsync(productModel);

					if (outputProductProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputProductBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductProcessBasketListViewModel.Items.Add(outputProductBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "TransferOutBasketViewModel":
					var transferOutBasketViewModel = _serviceProvider.GetRequiredService<TransferOutBasketViewModel>();
					var outProductItem = await ConvertOutProductAsync(productModel);
					if (transferOutBasketViewModel.TransferBasketModel.OutProducts.Any(x => x.ItemCode == outProductItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						transferOutBasketViewModel.TransferBasketModel.OutProducts.Add(outProductItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductSalesProcessBasketListViewModel":
					var outputProductSalesProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();
					var outputSalesBasketItem = await ConvertOutputSalesBasketAsync(productModel);
					if (outputProductSalesProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputSalesBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductSalesProcessBasketListViewModel.Items.Add(outputSalesBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "InputProductPurchaseProcessBasketListViewModel":
					var inputProductPurchaseProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseProcessBasketListViewModel>();
					var inputPurchaseBasketItem = await ConvertInputPurchaseBasketAsync(productModel);

					if (inputProductPurchaseProcessBasketListViewModel.Items.Any(x => x.ItemCode == inputPurchaseBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductPurchaseProcessBasketListViewModel.Items.Add(inputPurchaseBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "ReturnPurchaseBasketViewModel":
					var returnPurchaseBasketViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseBasketViewModel>();
					var returnPurchaseBasketItem = await ConvertReturnPurchaseBasketAsync(productModel);

					if (returnPurchaseBasketViewModel.Items.Any(x => x.ItemCode == returnPurchaseBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						returnPurchaseBasketViewModel.Items.Add(returnPurchaseBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "ReturnSalesBasketViewModel":
					var returnSalesBasketViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();
					var returnSalesBasketItem = await ConvertReturnSalesBasketAsync(productModel);
					if (returnSalesBasketViewModel.Items.Any(x => x.ItemCode == returnSalesBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						returnSalesBasketViewModel.Items.Add(returnSalesBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "InputProductPurchaseOrderProcessBasketListViewModel":
					var inputProductPurchaseOrderBasketListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketListViewModel>();
					var inputPurchaseOrderBasketItem = await ConvertInputPurchaseBasketAsync(productModel);
					if (inputProductPurchaseOrderBasketListViewModel.Items.Any(x => x.ItemCode == inputPurchaseOrderBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductPurchaseOrderBasketListViewModel.Items.Add(inputPurchaseOrderBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductSalesOrderProcessBasketListViewModel":
					var outputProductSalesOrderProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessBasketListViewModel>();
					var outputSalesOrderBasketItem = await ConvertOutputSalesBasketAsync(productModel);
					if (outputProductSalesOrderProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputSalesOrderBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductSalesOrderProcessBasketListViewModel.Items.Add(outputSalesOrderBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "DemandProcessBasketListViewModel":
					var demandProcessBasketListViewModel = _serviceProvider.GetRequiredService<DemandProcessBasketListViewModel>();
					var demandProcessBasketItem = await ConvertDemandProcessBasketAsync(productModel);
					if (demandProcessBasketListViewModel.Items.Any(x => x.ItemCode == demandProcessBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						demandProcessBasketListViewModel.Items.Add(demandProcessBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputOutsourceTransferBasketListViewModel":
					var outputOutsourceTransferBasketListViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferBasketListViewModel>();
					var outputOutsourceTransferBasketItem = await ConvertOutputOutsourceTransferBasketAsync(productModel);

					if (outputOutsourceTransferBasketListViewModel.Items.Any(x => x.ItemCode == outputOutsourceTransferBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputOutsourceTransferBasketListViewModel.Items.Add(outputOutsourceTransferBasketItem);
						await _userDialogs.AlertAsync($"Ürün Sepete Eklendi");		
					}
					break;
			}

			isFind = false;
		}
		catch (Exception)
		{

			throw;
		}
	}

	public async Task SendVariantBasketPageAsync(VariantModel variantModel, string comingPage)
	{
		try
		{
			switch(comingPage)
			{
				case "InputProductProcessBasketListViewModel":
					var inputProductProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>();
					var inputProductBasketItem = await ConvertInputProductBasketAsync(variantModel);
					if (inputProductProcessBasketListViewModel.Items.Any(x => x.ItemCode == inputProductBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductProcessBasketListViewModel.Items.Add(inputProductBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductProcessBasketListViewModel":
					var outputProductProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();
					var outputProductBasketItem = await ConvertOutputProductBasketAsync(variantModel);

					if (outputProductProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputProductBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductProcessBasketListViewModel.Items.Add(outputProductBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "TransferOutBasketViewModel":
					var transferOutBasketViewModel = _serviceProvider.GetRequiredService<TransferOutBasketViewModel>();
					var outProductItem = await ConvertOutProductAsync(variantModel);
					if (transferOutBasketViewModel.TransferBasketModel.OutProducts.Any(x => x.ItemCode == outProductItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						transferOutBasketViewModel.TransferBasketModel.OutProducts.Add(outProductItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductSalesProcessBasketListViewModel":
					var outputProductSalesProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();
					var outputSalesBasketItem = await ConvertOutputSalesBasketAsync(variantModel);
					if (outputProductSalesProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputSalesBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductSalesProcessBasketListViewModel.Items.Add(outputSalesBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "InputProductPurchaseProcessBasketListViewModel":
					var inputProductPurchaseProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseProcessBasketListViewModel>();
					var inputPurchaseBasketItem = await ConvertInputPurchaseBasketAsync(variantModel);

					if (inputProductPurchaseProcessBasketListViewModel.Items.Any(x => x.ItemCode == inputPurchaseBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductPurchaseProcessBasketListViewModel.Items.Add(inputPurchaseBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "ReturnPurchaseBasketViewModel":
					var returnPurchaseBasketViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseBasketViewModel>();
					var returnPurchaseBasketItem = await ConvertReturnPurchaseBasketAsync(variantModel);

					if (returnPurchaseBasketViewModel.Items.Any(x => x.ItemCode == returnPurchaseBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						returnPurchaseBasketViewModel.Items.Add(returnPurchaseBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "ReturnSalesBasketViewModel":
					var returnSalesBasketViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();
					var returnSalesBasketItem = await ConvertReturnSalesBasketAsync(variantModel);
					if (returnSalesBasketViewModel.Items.Any(x => x.ItemCode == returnSalesBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						returnSalesBasketViewModel.Items.Add(returnSalesBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "InputProductPurchaseOrderProcessBasketListViewModel":
					var inputProductPurchaseOrderBasketListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketListViewModel>();
					var inputPurchaseOrderBasketItem = await ConvertInputPurchaseBasketAsync(variantModel);
					if (inputProductPurchaseOrderBasketListViewModel.Items.Any(x => x.ItemCode == inputPurchaseOrderBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductPurchaseOrderBasketListViewModel.Items.Add(inputPurchaseOrderBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductSalesOrderProcessBasketListViewModel":
					var outputProductSalesOrderProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessBasketListViewModel>();
					var outputSalesOrderBasketItem = await ConvertOutputSalesBasketAsync(variantModel);
					if (outputProductSalesOrderProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputSalesOrderBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductSalesOrderProcessBasketListViewModel.Items.Add(outputSalesOrderBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "DemandProcessBasketListViewModel":
					var demandProcessBasketListViewModel = _serviceProvider.GetRequiredService<DemandProcessBasketListViewModel>();
					var demandProcessBasketItem = await ConvertDemandProcessBasketAsync(variantModel);
					if (demandProcessBasketListViewModel.Items.Any(x => x.ItemCode == demandProcessBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						demandProcessBasketListViewModel.Items.Add(demandProcessBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputOutsourceTransferBasketListViewModel":
					var outputOutsourceTransferBasketViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferBasketListViewModel>();
					var outputOutsourceTransferBasketItem = await ConvertOutputOutsourceTransferBasketAsync(variantModel);
					if (outputOutsourceTransferBasketViewModel.Items.Any(x => x.ItemCode == outputOutsourceTransferBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputOutsourceTransferBasketViewModel.Items.Add(outputOutsourceTransferBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
			}

			isFind = false;
		}
		catch (Exception)
		{

			throw;
		}
	}

	#region InputProductBasketModel
	private async Task<InputProductBasketModel> ConvertInputProductBasketAsync(ProductModel productModel)
	{

		try
		{
			return await Task.Run(() =>
			{

				var basketItem = new InputProductBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					Image = productModel.ImageData,
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

	private async Task<InputProductBasketModel> ConvertInputProductBasketAsync(VariantModel variantModel)
	{

		try
		{
			return await Task.Run(() =>
			{

				var basketItem = new InputProductBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.Code,
					MainItemName = variantModel.Name,
					MainItemReferenceId = variantModel.ReferenceId,
					StockQuantity = variantModel.StockQuantity,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					IsVariant = true,
					//VariantIcon = variantModel.VariantIcon,
					//LocTrackingIcon = variantModel.LocTrackingIcon,
					//TrackingTypeIcon = variantModel.TrackingTypeIcon
				};

				return basketItem;
			});



		}
		catch (System.Exception)
		{

			throw;
		}

	}
	#endregion

	#region OutputProductBasketModel
	private async Task<OutputProductBasketModel> ConvertOutputProductBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputProductBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					//Image = productModel.ImageData,
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
		catch (System.Exception ex)
		{
			throw;
		}

	}
	private async Task<OutputProductBasketModel> ConvertOutputProductBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputProductBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					//Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.Code,
					MainItemName = variantModel.Name,
					MainItemReferenceId = variantModel.ReferenceId,
					StockQuantity = variantModel.StockQuantity,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					IsVariant = true,
					//VariantIcon = variantModel.VariantIcon,
					//LocTrackingIcon = variantModel.LocTrackingIcon,
					//TrackingTypeIcon = variantModel.TrackingTypeIcon
				};

				return basketItem;
			});
		}
		catch (System.Exception ex)
		{
			throw;
		}

	}
	#endregion

	#region OutputSalesBasketModel
	private async Task<OutputSalesBasketModel> ConvertOutputSalesBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputSalesBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					//Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					MainItemReferenceId = default,  //
					MainItemCode = string.Empty,    //
					MainItemName = string.Empty,    //
					StockQuantity = productModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					OutputQuantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTrackingIcon = productModel.LocTrackingIcon,
					VariantIcon = productModel.VariantIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<OutputSalesBasketModel> ConvertOutputSalesBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputSalesBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					//Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					MainItemReferenceId = variantModel.ReferenceId,
					MainItemCode = variantModel.Code,
					MainItemName = variantModel.Name,
					StockQuantity = variantModel.StockQuantity,
					IsSelected = false,
					IsVariant = true,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
					OutputQuantity = variantModel.LocTracking == 0 ? 1 : 0,
					//LocTrackingIcon = variantModel.LocTrackingIcon,
					//VariantIcon = variantModel.VariantIcon,
					//TrackingTypeIcon = variantModel.TrackingTypeIcon,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	#endregion

	#region InputPurchaseBasketModel
	private async Task<InputPurchaseBasketModel> ConvertInputPurchaseBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new InputPurchaseBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					Image = productModel.ImageData,
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
					VariantIcon = productModel.VariantIcon,
					LocTrackingIcon = productModel.LocTrackingIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<InputPurchaseBasketModel> ConvertInputPurchaseBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new InputPurchaseBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.Code,
					MainItemName = variantModel.Name,
					MainItemReferenceId = variantModel.ReferenceId,
					StockQuantity = variantModel.StockQuantity,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
					InputQuantity = variantModel.LocTracking == 0 ? 1 : 0,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					IsVariant = true,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	#endregion

	#region ReturnPurchaseBasketModel
	private async Task<ReturnPurchaseBasketModel> ConvertReturnPurchaseBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new ReturnPurchaseBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					MainItemReferenceId = default,  //
					MainItemCode = string.Empty,    //
					MainItemName = string.Empty,    //
					StockQuantity = productModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTrackingIcon = productModel.LocTrackingIcon,
					VariantIcon = productModel.VariantIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<ReturnPurchaseBasketModel> ConvertReturnPurchaseBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new ReturnPurchaseBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					MainItemReferenceId = variantModel.ReferenceId,
					MainItemCode = variantModel.Code,
					MainItemName = variantModel.Name,
					StockQuantity = variantModel.StockQuantity,
					IsSelected = false,
					IsVariant = true,
					//Image = string.Empty,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	#endregion

	#region ReturnSalesBasketModel
	private async Task<ReturnSalesBasketModel> ConvertReturnSalesBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new ReturnSalesBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					Image = productModel.ImageData,
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
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<ReturnSalesBasketModel> ConvertReturnSalesBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new ReturnSalesBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.Code,
					MainItemName = variantModel.Name,
					MainItemReferenceId = variantModel.ReferenceId,
					StockQuantity = variantModel.StockQuantity,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
					InputQuantity = variantModel.LocTracking == 0 ? 1 : 0,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					IsVariant = true,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	#endregion

	#region OutProductModel
	private async Task<OutProductModel> ConvertOutProductAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutProductModel
				{
					//ReferenceId = productModel.ReferenceId,
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					//Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					StockQuantity = productModel.StockQuantity,
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					LocTrackingIcon = productModel.LocTrackingIcon,
					VariantIcon = productModel.VariantIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
					OutputQuantity = productModel.LocTracking == 0 ? 1 : 0,
					IsSelected = productModel.IsSelected,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{
			throw;
		}
	}

	private async Task<OutProductModel> ConvertOutProductAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutProductModel
				{
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					MainItemReferenceId = variantModel.ProductReferenceId,
					MainItemCode = variantModel.ProductCode,
					MainItemName = variantModel.ProductName,
					// Image = variantModel.ImageData,
					//VatRate = variantModel.VatRate,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					StockQuantity = variantModel.StockQuantity,
					IsVariant = true,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					//LocTrackingIcon = variantModel.LocTrackingIcon,
					//VariantIcon = variantModel.VariantIcon,
					//TrackingTypeIcon = variantModel.TrackingTypeIcon,
					OutputQuantity = variantModel.LocTracking == 0 ? 1 : 0,
					IsSelected = false,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{
			throw;
		}
	}
	#endregion

	#region OutputOutsourceTransferBasketModel
	private async Task<OutputOutsourceTransferBasketModel> ConvertOutputOutsourceTransferBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputOutsourceTransferBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					//Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					MainItemReferenceId = default,  //
					MainItemCode = string.Empty,    //
					MainItemName = string.Empty,    //
					StockQuantity = productModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					//LocTrackingIcon = productModel.LocTrackingIcon,
					//VariantIcon = productModel.VariantIcon,
					//TrackingTypeIcon = productModel.TrackingTypeIcon,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{
			throw;
		}
	}
	private async Task<OutputOutsourceTransferBasketModel> ConvertOutputOutsourceTransferBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputOutsourceTransferBasketModel
				{
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					MainItemReferenceId = variantModel.ProductReferenceId,
					MainItemCode = variantModel.ProductCode,
					MainItemName = variantModel.ProductName,
					StockQuantity = variantModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = true,
					LocTracking = variantModel.LocTracking,
					//Image = variantModel.ImageData,
					TrackingType = variantModel.TrackingType,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{
			throw;
		}
	}
	#endregion

	#region DemandProcessBasketModel
	private async Task<DemandProcessBasketModel> ConvertDemandProcessBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new DemandProcessBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					//Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					MainItemReferenceId = default,  //
					MainItemCode = string.Empty,    //
					MainItemName = string.Empty,    //
					StockQuantity = productModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					//SafeLevel = item.SafeLevel,
					//Quantity = item.SafeLevel - productModel.StockQuantity,
					LocTrackingIcon = productModel.LocTrackingIcon,
					VariantIcon = productModel.VariantIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<DemandProcessBasketModel> ConvertDemandProcessBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new DemandProcessBasketModel
				{
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					//Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					MainItemReferenceId = variantModel.ProductReferenceId,
					MainItemCode = variantModel.ProductCode,
					MainItemName = variantModel.ProductName,
					StockQuantity = variantModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = true,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					//SafeLevel = item.SafeLevel,
					//Quantity = item.SafeLevel - productModel.StockQuantity,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	#endregion



	private async Task SearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind) {
				var result = await _barcodeSearchService.SearchByProductCode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);
						var productModel = productModelList?.First();
						isFind = true;
						await SendProductBasketPageAsync(productModel, comingPage);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchByVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchByVariantCode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();

						isFind = true;
						await SendVariantBasketPageAsync(variantModel, comingPage);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task SearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchByProductMainBarcode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendProductBasketPageAsync(productModel, comingPage);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchByVariantMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchByVariantMainBarcode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();

						isFind = true;
						await SendVariantBasketPageAsync(variantModel, comingPage);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchByProductSubBarcode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendProductBasketPageAsync(productModel, comingPage);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchByVariantSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchByVariantSubBarcode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();

						isFind = true;
						await SendVariantBasketPageAsync(variantModel, comingPage);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchByProductSeriNumber(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendProductBasketPageAsync(productModel, comingPage);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchByVariantSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchByVariantSeriNumber(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();

						isFind = true;
						await SendVariantBasketPageAsync(variantModel, comingPage);
					}
				}
			}
		}
		catch (Exception)
		{

			throw;
		}
	}

	private async Task SearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchByProductLotNumber(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendProductBasketPageAsync(productModel, comingPage);
					}
				}
			}
			
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchByVariantLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchByVariantLotNumber(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();

						isFind = true;
						await SendVariantBasketPageAsync(variantModel, comingPage);
					}
				}
			}
			
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchBySupplierProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchBySupplierProductCode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendProductBasketPageAsync(productModel, barcode);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchBySupplierVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchBySupplierVariantCode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();

						isFind = true;
						await SendVariantBasketPageAsync(variantModel, comingPage);
					}
				}
			}
			

		}
		catch (System.Exception)
		{

			throw;
		}
	}
}
