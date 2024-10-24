using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

[QueryProperty(name: nameof(ProcurementCustomerBasketModel), queryId: nameof(ProcurementCustomerBasketModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class ProcurementByCustomerFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IHttpClientSysService _httpClientSysService;

	[ObservableProperty]
	ProcurementCustomerBasketModel procurementCustomerBasketModel = null!;


	[ObservableProperty]
	ObservableCollection<ProcurementCustomerBasketModel> items;

	

	[ObservableProperty]
	private DateTime transactionDate = DateTime.Now;

	[ObservableProperty]
	private string documentNumber = string.Empty;

	[ObservableProperty]
	private string documentTrackingNumber = string.Empty;

	[ObservableProperty]
	private string specialCode = string.Empty;

	[ObservableProperty]
	private string description = string.Empty;

	public ProcurementByCustomerFormViewModel(IHttpClientService httpClientService, IHttpClientSysService httpClientSysService)
	{
		_httpClientService = httpClientService;
		_httpClientSysService = httpClientSysService;

		Title = "Ürün Toplama Formu";
	}

	public Page CurrentPage { get; set; } = null!;

    public Command SaveCommand { get; }
}
