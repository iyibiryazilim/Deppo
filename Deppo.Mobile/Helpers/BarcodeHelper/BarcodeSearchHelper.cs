using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
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

	public async Task<dynamic> BarcodeDetectedAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		isFind = false;
		try
		{
			var cts = new CancellationTokenSource();

			var codeTasks = new List<Task<dynamic>>
			{
				SearchByProductCodeAsync(httpClient, firmNumber, periodNumber, barcode),
				SearchByVariantCodeAsync(httpClient, firmNumber, periodNumber, barcode),
			 };

			var barcodeTasks = new List<Task<dynamic>>
			{
				
				SearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, barcode),
				SearchByVariantMainBarcodeAsync(httpClient, firmNumber, periodNumber, barcode),
				SearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, barcode),
				SearchByVariantSubBarcodeAsync(httpClient, firmNumber, periodNumber, barcode),
				
			 };

			var seriTasks = new List<Task<dynamic>>
			{
				
				SearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, barcode),
				SearchByVariantSeriNumberAsync(httpClient, firmNumber, periodNumber, barcode),
				
				
			 };

			var lotTasks = new List<Task<dynamic>>
			{

				
				SearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, barcode),
				SearchByVariantLotNumberAsync(httpClient, firmNumber, periodNumber, barcode),

			 };

			var supplierTasks = new List<Task<dynamic>>
			{
				SearchBySupplierProductCodeAsync(httpClient, firmNumber, periodNumber, barcode),
				SearchBySupplierVariantCodeAsync(httpClient, firmNumber, periodNumber, barcode),

			 };


			var codeTaskResults = await Task.WhenAll(
				codeTasks
			);

			if(codeTaskResults.Any())
			{
				foreach (var result in codeTaskResults)
				{
					if (result != null) return result;
				}
			}
			else
			{
				var barcodeTaskResults = await Task.WhenAll(barcodeTasks);

				if(barcodeTaskResults.Any())
				{
                    foreach (var result in barcodeTaskResults)
                    {
						if (result != null) return result;
					}
                }
				else
				{
					var seriTaskResults = await Task.WhenAll(seriTasks);

					if(seriTaskResults.Any())
					{
						foreach (var result in seriTaskResults)
						{
							if (result != null) return result;
						}
					}
					else
					{
						var lotTaskResult = await Task.WhenAll(lotTasks);

						if(lotTaskResult.Any())
						{
                            foreach (var result in lotTaskResult)
                            {
								if (result != null) return result;
							}
                        }
						else
						{
							var supplierTaskResult = await Task.WhenAll(supplierTasks);

							if(supplierTaskResult.Any())
							{
                                foreach (var result in supplierTaskResult)
                                {
									if (result != null) return result;
								}
                            }
						}
					}
				}
			}

			return null;
        }
		catch (Exception ex)
		{
			throw;
		}
		finally
		{
			
		}
	}


	#region InputProductBasketModel
	private async Task<InputProductBasketModel> ConvertInputProductBasketAsync(BarcodeInProductModel productModel)
	{

		try
		{
			return await Task.Run(() =>
			{

				var basketItem = new InputProductBasketModel
				{
					ReferenceId = Guid.NewGuid(),
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
					ReferenceId = Guid.NewGuid(),
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.ProductCode,
					MainItemName = variantModel.ProductName,
					MainItemReferenceId = variantModel.ProductReferenceId,
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
	private async Task<OutputProductBasketModel> ConvertOutputProductBasketAsync(BarcodeInProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputProductBasketModel
				{
					ReferenceId = Guid.NewGuid(),
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
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.ProductCode,
					MainItemName = variantModel.ProductName,
					MainItemReferenceId = variantModel.ProductReferenceId,
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
	private async Task<OutputSalesBasketModel> ConvertOutputSalesBasketAsync(BarcodeInProductModel productModel)
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
	private async Task<InputPurchaseBasketModel> ConvertInputPurchaseBasketAsync(BarcodeInProductModel productModel)
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
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.ProductCode,
					MainItemName = variantModel.ProductName,
					MainItemReferenceId = variantModel.ProductReferenceId,
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
	private async Task<ReturnPurchaseBasketModel> ConvertReturnPurchaseBasketAsync(BarcodeInProductModel productModel)
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
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					Image = variantModel.ImageData,
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
	private async Task<ReturnSalesBasketModel> ConvertReturnSalesBasketAsync(BarcodeInProductModel productModel)
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
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.ProductCode,
					MainItemName = variantModel.ProductName,
					MainItemReferenceId = variantModel.ProductReferenceId,
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
	private async Task<OutProductModel> ConvertOutProductAsync(BarcodeInProductModel productModel)
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
					ReferenceId = new Guid(),
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



	public async Task<dynamic> SearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);
						var productModel = productModelList?.First();
						isFind = true;
						return productModel;
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	public async Task<dynamic> SearchByVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						return variantModel;
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	public async Task<dynamic> SearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						return productModel;
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	public async Task<dynamic> SearchByVariantMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						return variantModel;
					}
				}
			}
			return null;
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	public async Task<dynamic> SearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						return productModel;
					}
				}
			}
			return null;
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	public async Task<dynamic> SearchByVariantSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						return variantModel;
					}
				}
			}
			return null;

		}
		catch (System.Exception)
		{

			throw;
		}
	}

	public async Task<dynamic> SearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						return productModel;
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	public async Task<dynamic> SearchByVariantSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						return variantModel;
					}
				}
			}
			return null;
		}
		catch (Exception)
		{

			throw;
		}
	}

	public async Task<dynamic> SearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						return productModel;
					}
				}
			}
			return null;
			
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	public async Task<dynamic> SearchByVariantLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						return variantModel;
					}
				}
			}
			return null;
			
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	public async Task<dynamic> SearchBySupplierProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						return productModel;
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	public async Task<dynamic> SearchBySupplierVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						return variantModel;
					}
				}
			}
			return null;

		}
		catch (System.Exception)
		{

			throw;
		}
	}
}
