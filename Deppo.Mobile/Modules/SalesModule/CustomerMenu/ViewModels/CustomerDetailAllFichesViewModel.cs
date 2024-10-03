using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels
{
    public partial class CustomerDetailAllFichesViewModel:BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ICustomerDetailAllFichesService _customerDetailAllFichesService;

        
        public CustomerDetailAllFichesViewModel(IHttpClientService httpClientService,ICustomerDetailAllFichesService customerDetailAllFichesService)
        {
            _httpClientService = httpClientService;
            _customerDetailAllFichesService = customerDetailAllFichesService;



        }

        public Page CurrentPage { get; set; }

    }
}
