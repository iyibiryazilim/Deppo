using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.OutsourceModels;

public class OutsourceModel : Outsource
{
    private bool _isSelected;
    private int _shipAddressReferenceId;
    private string _shipAddressCode;
    private string _shipAddressName;


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

    public int ShipAddressReferenceId
    {
        get => _shipAddressReferenceId;
        set
        {
            _shipAddressReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public string ShipAddressCode
    {
        get => _shipAddressCode;
        set
        {
            _shipAddressCode = value;
            NotifyPropertyChanged();
        }
    }

    public string ShipAddressName
    {
        get => _shipAddressName;
        set
        {
            _shipAddressName = value;
            NotifyPropertyChanged();
        }
    }




}
