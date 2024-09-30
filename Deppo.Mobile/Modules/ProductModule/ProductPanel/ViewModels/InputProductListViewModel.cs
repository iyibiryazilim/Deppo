using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Org.Apache.Http.Conn.Params;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;

public partial class InputProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IProductPanelService _productPanelService;
	private readonly IUserDialogs _userDialogs;

	public ObservableCollection<ProductModel> Items { get; } = new();

	public InputProductListViewModel(IProductPanelService productPanelService, IHttpClientService httpClientService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_productPanelService = productPanelService;
		_userDialogs = userDialogs;
		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
	}

	public Page CurrentPage { get; set; } = null!;

	public Command LoadItemsCommand { get;}
	public Command LoadMoreItemsCommand { get;}
	public Command ItemTappedCommand { get; }

	
	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Load Items...");
			await Task.Delay(1000);
			// List.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _productPanelService.GetInputProduct(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: "",
				skip: 0,
				take: 20
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<ProductModel>(item);
					Items.Add(obj);
				}
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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Load More Items...");
		
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _productPanelService.GetInputProduct(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
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
					var obj = Mapping.Mapper.Map<ProductModel>(item);
					Items.Add(obj);
				}
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

}
