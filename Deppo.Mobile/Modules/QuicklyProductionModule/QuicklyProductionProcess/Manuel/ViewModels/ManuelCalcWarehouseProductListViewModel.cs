using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

using System;
using System.Collections.Generic;

using System.Collections.ObjectModel;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Deppo.Core.Models;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Core.BaseModels;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

public partial class ManuelCalcWarehouseProductListViewModel : BaseViewModel
{
    public ManuelCalcWarehouseProductListViewModel()
    {
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        //ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }

    public async Task LoadItemsAsync()
    {
    }

    public async Task LoadMoreItemsAsync()
    {
    }

    public async Task ItemTappedAsync()
    {
    }

    public async Task NextViewAsync()
    {
    }
}