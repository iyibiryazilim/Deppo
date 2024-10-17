using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.AnalysisModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;

public partial class OverviewAnalysisViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IHttpClientSysService _httpClientSysService;
    private readonly ITransactionAuditService _transactionAuditService;
    private readonly IUserDialogs _userDialogs;
    private readonly IOverviewAnalysisService _overviewAnalysisService;
    private readonly IApplicationUserService _applicationUserService;

    [ObservableProperty]
    ApplicationUser currentUser;

    public OverviewAnalysisViewModel(
        IUserDialogs userDialogs, 
    IHttpClientService httpClientService, 
    IOverviewAnalysisService overviewAnalysisService,
    ITransactionAuditService transactionAuditService,
    IHttpClientSysService httpClientSysService,
    IApplicationUserService applicationUserService
    )
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;
        _transactionAuditService = transactionAuditService;
        _httpClientSysService = httpClientSysService;
        _overviewAnalysisService = overviewAnalysisService;
        _applicationUserService = applicationUserService;


        Title = "Genel Analiz";        

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
    }

    [ObservableProperty]
    public OverviewAnalysisModel overviewAnalysisModel = new();

    public ObservableCollection<TransactionAudit> Items { get; } = new();

    public Command LoadItemsCommand { get; }

    [ObservableProperty]
    public DateTime selectedDate;

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            var httpClient = _httpClientSysService.GetOrCreateHttpClient();

            string filter = $"expand=Image&$orderby=TransactionDate desc&$top=20";
            var result = await _transactionAuditService.GetAllAsync(httpClient,filter);
            if(result.Any())
            {
                Items.Clear();
                foreach (var item in result)                
                    Items.Add(item);
                
            }
            
            await Task.WhenAll( GetInputTransactionCountAsync(),GetOutputTransactionCountAsync(),GetCurrentUserAsync());

            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            

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

    private async Task GetCurrentUserAsync()
    {
        try
        {
            var httpClient = _httpClientSysService.GetOrCreateHttpClient();
            string filter = $"filter=UserName eq '{_httpClientSysService.UserName}'";
            var result = await _applicationUserService.GetAllAsync(httpClient,filter);

            if(result.Any())            
                CurrentUser = result.FirstOrDefault();
            
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetTotalProductCountAsync()
    {

        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetTotalProductCountAsync(httpClient, _httpClientService.FirmNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<OverviewAnalysisModel>(item);
                    OverviewAnalysisModel.TotalProductCount = obj.TotalProductCount;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }

    }

    private async Task GetTotalInputProductCountAsync()
    {

        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetTotalInputProductCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<OverviewAnalysisModel>(item);
                    OverviewAnalysisModel.TotalInputProductCount = obj.TotalInputProductCount;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }

    }

    private async Task GetTotalOutputProductCountAsync()
    {

        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetTotalOutputProductCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<OverviewAnalysisModel>(item);
                    OverviewAnalysisModel.TotalOutputProductCount = obj.TotalOutputProductCount;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }

    }

    private async Task GetInputTransactionCountAsync()
    {

        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetInputTransactionCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<OverviewAnalysisModel>(item);
                    OverviewAnalysisModel.InputTransactionCount = obj.InputTransactionCount;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }

    }

    private async Task GetOutputTransactionCountAsync()
    {

        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetOutputTransactionCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<OverviewAnalysisModel>(item);
                    OverviewAnalysisModel.OutputTransactionCount = obj.OutputTransactionCount;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }

    }

    private async Task GetProductsWithNoTransactionsCountAsync()
    {

        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetProductsWithNoTransactionsCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber,SelectedDate);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<OverviewAnalysisModel>(item);
                    OverviewAnalysisModel.ProductsWithNoTransactionsCount = obj.ProductsWithNoTransactionsCount;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }

    }

    private async Task GetProductsWithNoTransactionsAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetProductsWithNoTransactionsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedDate, string.Empty, 0, 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                OverviewAnalysisModel.ProductsWithNoTransactions.Clear();
                foreach (var item in result.Data)
                    OverviewAnalysisModel.ProductsWithNoTransactions.Add(Mapping.Mapper.Map<ProductModel>(item));
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

}
