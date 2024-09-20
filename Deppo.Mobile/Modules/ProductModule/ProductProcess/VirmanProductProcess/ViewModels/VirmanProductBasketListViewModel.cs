using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

[QueryProperty(name: nameof(VirmanBasketModel), queryId: nameof(VirmanBasketModel))]
public partial class VirmanProductBasketListViewModel : BaseViewModel
{

}
