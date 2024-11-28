using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Deppo.Core.Models;

public  class WaitingProduct : INotifyPropertyChanged, IDisposable
{
	private int _productReferenceId;
	private string _productCode = string.Empty;
	private string _productName = string.Empty;
	private int _subUnitsetReferenceId = default;
	private string _subUnitsetCode = string.Empty;
	private string _subUnitsetName = string.Empty;
	private int _unitsetReferenceId = default;
	private string _unitsetCode = string.Empty;
	private string _unitsetName = string.Empty;
	private int _variantReferenceId;
	private string _variantCode = string.Empty;
	private string _variantName = string.Empty;
	private bool _isVariant;
	private int _trackingType;
	private int _locTracking;
	private string? _image;
	private double _waitingQuantity;

	private string _locTrackingIcon;
	private string _locTrackingIconColor;
	private string _variantIcon;
	private string _variantIconColor;
	private string _trackingTypeIcon;
	private string _trackingTypeIconColor;

	[Key]
	public int ProductReferenceId
	{
		get => _productReferenceId;
		set
		{
			if (_productReferenceId == value) return;
			_productReferenceId = value;
			NotifyPropertyChanged();
		}
	}

	public string ProductCode
	{
		get => _productCode;
		set
		{
			if (_productCode == value) return;
			_productCode = value;
			NotifyPropertyChanged();
		}
	}

	public string ProductName
	{
		get => _productName;
		set
		{
			if (_productName == value) return;
			_productName = value;
			NotifyPropertyChanged();
		}
	}

	
	public int VariantReferenceId
	{
		get => _variantReferenceId;
		set
		{
			if (_variantReferenceId == value) return;
			_variantReferenceId = value;
			NotifyPropertyChanged();
		}
	}
	

	public string VariantCode
	{
		get => _variantCode;
		set
		{
			if (_variantCode == value) return;
			_variantCode = value;
			NotifyPropertyChanged();
		}
	}

	public string VariantName
	{
		get => _variantName;
		set
		{
			if (_variantName == value) return;
			_variantName = value;
			NotifyPropertyChanged();
		}
	}


	[Browsable(false)]
	public int SubUnitsetReferenceId
	{
		get => _subUnitsetReferenceId;
		set
		{
			if (_subUnitsetReferenceId == value) return;
			_subUnitsetReferenceId = value;
			NotifyPropertyChanged();
		}
	}

	public string SubUnitsetCode
	{
		get => _subUnitsetCode;
		set
		{
			if (_subUnitsetCode == value) return;
			_subUnitsetCode = value;
			NotifyPropertyChanged();
		}
	}

	public string SubUnitsetName
	{
		get => _subUnitsetName;
		set
		{
			if (_subUnitsetName == value) return;
			_subUnitsetName = value;
			NotifyPropertyChanged();
		}
	}

	[Browsable(false)]
	public int UnitsetReferenceId
	{
		get => _unitsetReferenceId;
		set
		{
			if (_unitsetReferenceId == value) return;
			_unitsetReferenceId = value;
			NotifyPropertyChanged();
		}
	}

	public string UnitsetCode
	{
		get => _unitsetCode;
		set
		{
			if (_unitsetCode == value) return;
			_unitsetCode = value;
			NotifyPropertyChanged();
		}
	}

	public string UnitsetName
	{
		get => _unitsetName;
		set
		{
			if (_unitsetName == value) return;
			_unitsetName = value;
			NotifyPropertyChanged();
		}
	}

	public bool IsVariant
	{
		get => _isVariant;
		set
		{
			if (_isVariant == value) return;
			_isVariant = value;
			NotifyPropertyChanged();
		}
	}

	public int TrackingType
	{
		get => _trackingType;
		set
		{
			if (_trackingType == value) return;
			_trackingType = value;
			NotifyPropertyChanged();
		}
	}

	public int LocTracking
	{
		get => _locTracking;
		set
		{
			if (_locTracking == value) return;
			_locTracking = value;
			NotifyPropertyChanged();
		}
	}

	public double WaitingQuantity
	{
		get => _waitingQuantity;
		set
		{
			if (_waitingQuantity == value) return;
			_waitingQuantity = value;
			NotifyPropertyChanged();
		}
	}

	public string? Image
	{
		get => _image;
		set
		{
			if (_image == value) return;
			_image = value;
			NotifyPropertyChanged();
		}
	}

	public byte[] ImageData
	{
		get
		{
			if (string.IsNullOrEmpty(Image))
				return Array.Empty<byte>();
			else
			{
				return Convert.FromBase64String(Image);
			}
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


	public event PropertyChangedEventHandler? PropertyChanged;

	protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			PropertyChanged = null;
		}
	}
}
