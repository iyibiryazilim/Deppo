using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ViewModels;

public partial class ProductProcessViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    public ProductProcessViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "İşlemler";

        ProductionInputCommand = new Command(async () => await ProductionInputAsync());
        OverCountCommand = new Command(async () => await OverCountAsync());
        ConsumableProcessCommand = new Command(async () => await ConsumableProcessAsync());
    }

    public Command ProductionInputCommand { get; }
    public Command OverCountCommand { get; }
    public Command ConsumableProcessCommand { get; }

    private async Task ProductionInputAsync()
    {

        await Shell.Current.GoToAsync($"{nameof(InputProductProcessWarehouseListView)}", new Dictionary<string, object>
        {
            {nameof(InputProductProcessType), InputProductProcessType.ProductionInputProcess}
        });
    }

    private async Task OverCountAsync()
    {

        await Shell.Current.GoToAsync($"{nameof(InputProductProcessWarehouseListView)}", new Dictionary<string, object>
        {
            {nameof(InputProductProcessType), InputProductProcessType.OverCountProcess}
        });
    }

    private async Task ConsumableProcessAsync()
    {
		await Shell.Current.GoToAsync($"{nameof(OutputProductProcessWarehouseListView)}", new Dictionary<string, object>
		{
			{nameof(OutputProductProcessType), OutputProductProcessType.ConsumableProcess}
		});
	}


}
