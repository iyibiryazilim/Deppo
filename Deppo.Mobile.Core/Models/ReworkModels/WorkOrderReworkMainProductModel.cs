using Deppo.Mobile.Core.Models.QuicklyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.ReworkModels;

public class WorkOrderReworkMainProductModel : QuicklyBOMProductModel
{
	private List<WorkOrderReworkMainProductDetailModel> _details = new();
	public WorkOrderReworkMainProductModel()
    {
        
    }

    public List<WorkOrderReworkMainProductDetailModel> Details
    {
        get => _details;
        set
        {
            if(_details == value) return;
            _details = value;
            NotifyPropertyChanged();
        }
    }
}
