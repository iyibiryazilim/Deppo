using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.OutsourceModels;

public class OutsourceModel : Outsource
{
    private bool _isSelected;

    public OutsourceModel()
    {
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            NotifyPropertyChanged();
        }
    }

}
