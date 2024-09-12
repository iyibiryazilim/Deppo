using System;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.AnalysisModule.SalesAnalysis.ViewModels;

public partial class SalesAnalysisViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISalesAnalysisService _salesAnalysisService;
    private readonly IUserDialogs _userDialogs;

    public SalesAnalysisViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService , ISalesAnalysisService salesAnalysisService)
    {
        _httpClientService = httpClientService;
        _salesAnalysisService = salesAnalysisService;
        _userDialogs = userDialogs;


        Title = "Satış Analizi";
    }

}
