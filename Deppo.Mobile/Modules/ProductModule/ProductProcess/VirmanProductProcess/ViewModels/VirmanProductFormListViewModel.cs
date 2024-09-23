using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

public partial class VirmanProductFormListViewModel : BaseViewModel
{
    public VirmanProductFormListViewModel()
    {
    }

    public Page CurrentPage { get; set; }
}