﻿using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels.ActionViewModels;

[QueryProperty(name: nameof(SupplierDetailModel), queryId: nameof(SupplierDetailModel))]
public partial class SupplierDetailApprovedProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ISupplierDetailActionService _supplierDetailActionService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	SupplierDetailModel supplierDetailModel = null!;

	public ObservableCollection<ProductModel> Items { get; } = new();

	public SupplierDetailApprovedProductListViewModel(IHttpClientService httpClientService, ISupplierDetailActionService supplierDetailActionService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_supplierDetailActionService = supplierDetailActionService;
		_userDialogs = userDialogs;

		Title = "Tedarik Edilebilen Ürünler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		CloseCommand = new Command(async () => await CloseAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command CloseCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Items");
			await Task.Delay(1000);
			Items.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _supplierDetailActionService.GetApprovedProductsBySupplier(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId,
				search: "",
				skip: 0,
				take: 20
			);


			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<ProductModel>(item));
				}
			}

			_userDialogs.HideHud();
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

	private async Task LoadMoreItemsAsync()
	{
		if (Items.Count < 18)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading More Items");

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _supplierDetailActionService.GetApprovedProductsBySupplier(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId,
				search: "",
				skip: Items.Count,
				take: 20
			);


			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<ProductModel>(item));
				}
			}

			_userDialogs.HideHud();
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

	private async Task CloseAsync()
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
}
