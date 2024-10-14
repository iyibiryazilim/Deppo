using Deppo.Core.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.QuicklyModels;

public class BOMSubProductModel : Product
{
    private bool _isSelected;
    private string _locTrackingIcon;
    private string _locTrackingIconColor;
    private string _variantIcon;
    private string _variantIconColor;
    private string _trackingTypeIcon;
    private string _trackingTypeIconColor;
    private double _amount = default;
    private string _warehouseName = string.Empty;
    private int _warehouseNumber = default;
    private int _mainProductReferenceId = default;
    private string _mainProductCode = string.Empty;
    private string _mainProductName = string.Empty;



    public BOMSubProductModel()
    {

    }


    public double Amount
    {
        get => _amount;
        set
        {
            if (_amount == value) return;
            _amount = value;
            NotifyPropertyChanged();
        }
    }
   

    public string WarehouseName
    {
        get => _warehouseName = _warehouseName ?? string.Empty;
        set
        {
            if (_warehouseName == value) return;
            _warehouseName = value;
            NotifyPropertyChanged();
        }
    }
   
    public int WarehouseNumber
    {
        get => _warehouseNumber;
        set
        {
            if (_warehouseNumber == value) return;
            _warehouseNumber = value;
            NotifyPropertyChanged();
        }
    }
   
    public int MainProductReferenceId
    {
        get => _mainProductReferenceId;
        set
        {
            if (_mainProductReferenceId == value) return;
            _mainProductReferenceId = value;
            NotifyPropertyChanged();
        }
    }
    
    public string MainProductCode
    {
        get => _mainProductCode;
        set
        {
            if (_mainProductCode == value) return;
            _mainProductCode = value;
            NotifyPropertyChanged();
        }
    }

    public string MainProductName
	{
		get => _mainProductName;
		set
		{
			if (_mainProductName == value) return;
			_mainProductName = value;
			NotifyPropertyChanged();
		}
	}
    
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            NotifyPropertyChanged();
        }
    }

    public string LocTrackingIcon
    {
        get => _locTrackingIcon = _locTrackingIcon ?? "location-dot";
        set
        {
            if (_locTrackingIcon == value) return;
            _locTrackingIcon = value;
            NotifyPropertyChanged();
        }
    }

    public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

    public string VariantIcon
    {
        get => _variantIcon = _variantIcon ?? "bookmark";
        set
        {
            if (_variantIcon == value) return;
            _variantIcon = value;
            NotifyPropertyChanged();
        }
    }

    public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

    public string TrackingTypeIcon
    {
        get => _trackingTypeIcon = _trackingTypeIcon ?? "box-archive";
        set
        {
            if (_trackingTypeIcon == value) return;
            _trackingTypeIcon = value;
            NotifyPropertyChanged();
        }
    }

    public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";
}
