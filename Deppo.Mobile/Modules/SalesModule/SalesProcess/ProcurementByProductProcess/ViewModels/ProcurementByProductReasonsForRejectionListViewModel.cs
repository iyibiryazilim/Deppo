using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcurementModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Sys.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

[QueryProperty(name: nameof(ProcurementProductBasketProductModel), queryId: nameof(ProcurementProductBasketProductModel))]
public partial class ProcurementByProductReasonsForRejectionListViewModel : BaseViewModel
{
	private readonly IHttpClientSysService _httpClientSysService;
	private readonly IUserDialogs _userDialogs;
	private readonly IReasonsForRejectionProcurementService _reasonsForRejectionProcurementService;

	[ObservableProperty]
	ProcurementProductBasketProductModel procurementProductBasketProductModel = null!;

	public ObservableCollection<ReasonsForRejectionProcurementModel> Items { get; } = new();

	[ObservableProperty]
	ReasonsForRejectionProcurementModel? selectedItem;

	public ProcurementByProductReasonsForRejectionListViewModel(IHttpClientSysService httpClientSysService, IUserDialogs userDialogs, IReasonsForRejectionProcurementService reasonsForRejectionProcurementService)
	{
		_httpClientSysService = httpClientSysService;
		_userDialogs = userDialogs;
		_reasonsForRejectionProcurementService = reasonsForRejectionProcurementService;

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		ItemTappedCommand = new Command<ReasonsForRejectionProcurementModel>(async (item) => await ItemTappedAsync(item));
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		CloseCommand = new Command(async () => await CloseAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command ConfirmCommand { get; }
	public Command CloseCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClientSys = _httpClientSysService.GetOrCreateHttpClient();
			var result = await _reasonsForRejectionProcurementService.GetAllAsync(httpClientSys);

			if (result.Any())
			{
				foreach (var item in result)
				{
					var obj = new ReasonsForRejectionProcurementModel
					{
						Oid = item.Oid,
						Code = item.Code,
						Name = item.Name,
						IsSelected = false,
					};

					Items.Add(obj);
				}
			}

			if (_userDialogs.IsHudShowing)
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

	private async Task ItemTappedAsync(ReasonsForRejectionProcurementModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedItem == item)
			{
				SelectedItem.IsSelected = false;
				SelectedItem = null;
			}
			else
			{
				if (SelectedItem != null)
					SelectedItem.IsSelected = false;

				SelectedItem = item;
				SelectedItem.IsSelected = true;
			}
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

	private async Task ConfirmAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedItem is null)
			{
				await _userDialogs.AlertAsync("Lütfen bir hata kodu seçiniz.", "Uyarı", "Tamam");
				return;
			}

			ProcurementProductBasketProductModel.RejectionOid = SelectedItem.Oid;
			ProcurementProductBasketProductModel.RejectionCode = SelectedItem.Code;
			ProcurementProductBasketProductModel.RejectionName = SelectedItem.Name;

			SelectedItem.IsSelected = false;
			SelectedItem = null;

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

	private async Task CloseAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedItem is not null)
			{
				var confirm = await _userDialogs.ConfirmAsync("Seçili hata kodu silinecektir. Onaylıyor musunuz?", "Uyarı", "Evet", "Hayır");

				if (!confirm)
					return;

				SelectedItem.IsSelected = false;
				SelectedItem = null;
			}

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
