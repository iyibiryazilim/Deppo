using System;
using System.Collections.ObjectModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.ViewModels;

public partial class QuicklyProductionOutputProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IQuicklyProductionPanelService _quicklyProductionPanelService;
    private readonly IUserDialogs _userDialogs;

    public QuicklyProductionOutputProductListViewModel(
        IHttpClientService httpClientService,
        IQuicklyProductionPanelService quicklyProductionPanelService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _quicklyProductionPanelService = quicklyProductionPanelService;
        _userDialogs = userDialogs;

        Title = "Üretilen Malzeme Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
    }

    public Page CurrentPage { get; set; }
    public ObservableCollection<ProductModel> Items { get; } = new();

    public Command LoadItemsCommand { get; set; }
    public Command LoadMoreItemsCommand { get; set; }
    public Command CloseCommand { get; set; }

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            Items.Clear();
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _quicklyProductionPanelService.GetQuicklyProductionOutputProductListAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                search: "",
                skip: 0,
                take: 20
            );
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var data = Mapping.Mapper.Map<ProductModel>(item);
                        Items.Add(data);
                    }
                }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert("Bir hata oluştu. Lütfen tekrar deneyiniz.");
        }
        finally
        {
            IsBusy = false;
        }


    }

}
