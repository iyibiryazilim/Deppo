using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels
{
    public partial class InputOutsourceTransferOutsourceSupplierListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IOutsourceService _outsourceService;
        private readonly IUserDialogs _userDialogs;
        private readonly IShipAddressService _shipAddressService;

        public InputOutsourceTransferOutsourceSupplierListViewModel(IHttpClientService httpClientService, IOutsourceService outsourceService, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _outsourceService = outsourceService;
            _userDialogs = userDialogs;

            Title = "Fason Cariler";
        }
    }
}