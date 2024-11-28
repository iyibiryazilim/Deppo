using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Deppo.Core.BaseModels;

public class BaseFiche : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private int _ficheType;
    private string ficheNumber = string.Empty;
    private DateTime ficheDate;
    private TimeSpan ficheTime;
    private string documentNumber = string.Empty;
    private DateTime documentDate;
    private string specialCode = string.Empty;
    private int currentReferenceId;
    private string currentCode = string.Empty;
    private string currentName = string.Empty;
    private int warehouseNumber;
    private string warehouseName = string.Empty;
    private string description = string.Empty;
    private string _shipAddressReferenceId;
	private string _shipAddressCode;
	private string _shipAddressName;

	public BaseFiche()
    {
    }

    [Key]
    public int ReferenceId
    {
        get => _referenceId;
        set
        {
            if (_referenceId == value) return;
            _referenceId = value;
            NotifyPropertyChanged(nameof(ReferenceId));
        }
    }

    public int FicheType
    {
        get => _ficheType;
        set
        {
            if (_ficheType == value) return;
            _ficheType = value;
            NotifyPropertyChanged(nameof(FicheType));
        }
    }

    public string FicheTypeName
    {
        get
        {
            switch (_ficheType)
            {
                case 1:
                    return "Satınalma İrsaliyesi";
                case 2:
                    return "Perakende Satış İade İrsaliyesi";
                case 3:
                    return "Toptan Satış İade İrsaliyesi";
                case 4:
                    return "Konsinye Çıkış İade İrsaliyesi";
                case 5:
                    return "Konsinye Giriş İrsaliyesi";
                case 6:
                    return "Satınalma İade İrsaliyesi";
                case 7:
                    return "Perakende Satış İrsaliyesi";
                case 8:
                    return "Toptan Satış İrsaliyesi";
                case 9:
                    return "Konsinye Çıkış İrsaliyesi";
                case 10:
                    return "Konsinye Giriş İade İrsaliyesi";
                case 11:
                    return "Fire Fişi";
                case 12:
                    return "Sarf Fişi";
                case 13:
                    return "Üretimden Giriş Fişi";
                case 14:
                    return "Devir Fişi";
                case 25:
                    return "Ambar Transfer Fişi";
                case 26:
                    return "Mustahsil İrsaliyesi";
                case 50:
                    return "Sayım Fazlası Fişi";
                case 51:
                    return "Sayım Eksiği Fişi";

                default:
                    return "Diğer";
            }
        }
    }


    public string FicheNumber
    {
        get => ficheNumber;
        set
        {
            if (ficheNumber == value) return;
            ficheNumber = value;
            NotifyPropertyChanged(nameof(FicheNumber));
        }
    }

    public DateTime FicheDate
    {
        get => ficheDate;
        set
        {
            if (ficheDate == value) return;
            ficheDate = value;
            NotifyPropertyChanged(nameof(FicheDate));
        }
    }

    public TimeSpan FicheTime
    {
        get => ficheTime;
        set
        {
            if (ficheTime == value) return;
            ficheTime = value;
            NotifyPropertyChanged(nameof(FicheTime));
        }
    }

    public string DocumentNumber
    {
        get => documentNumber;
        set
        {
            if (documentNumber == value) return;
            documentNumber = value;
            NotifyPropertyChanged(nameof(DocumentNumber));
        }
    }

    public DateTime DocumentDate
    {
        get => documentDate;
        set
        {
            if (documentDate == value) return;
            documentDate = value;
            NotifyPropertyChanged(nameof(DocumentDate));
        }
    }

    public string SpecialCode
    {
        get => specialCode;
        set
        {
            if (specialCode == value) return;
            specialCode = value;
            NotifyPropertyChanged(nameof(SpecialCode));
        }
    }

    public int CurrentReferenceId
    {
        get => currentReferenceId;
        set
        {
            if (currentReferenceId == value) return;
            currentReferenceId = value;
            NotifyPropertyChanged(nameof(CurrentReferenceId));
        }
    }

    public string CurrentCode
    {
        get => currentCode;
        set
        {
            if (currentCode == value) return;
            currentCode = value;
            NotifyPropertyChanged(nameof(CurrentCode));
        }
    }

    public string CurrentName
    {
        get => currentName;
        set
        {
            if (currentName == value) return;
            currentName = value;
            NotifyPropertyChanged(nameof(CurrentName));
        }
    }

    public int WarehouseNumber
    {
        get => warehouseNumber;
        set
        {
            if (warehouseNumber == value) return;
            warehouseNumber = value;
            NotifyPropertyChanged(nameof(WarehouseNumber));
        }
    }

    public string WarehouseName
    {
        get => warehouseName;
        set
        {
            if (warehouseName == value) return;
            warehouseName = value;
            NotifyPropertyChanged(nameof(WarehouseName));
        }
    }

    public string Description
    {
        get => description;
        set
        {
            if (description == value) return;
            description = value;
            NotifyPropertyChanged(nameof(Description));
        }
    }

	public string ShipAddressReferenceId
	{
		get => _shipAddressReferenceId;
		set
		{
			if (_shipAddressReferenceId == value) return;
			_shipAddressReferenceId = value;
			NotifyPropertyChanged(nameof(ShipAddressReferenceId));
		}
	}

	public string ShipAddressCode
	{
		get => _shipAddressCode;
		set
		{
			if (_shipAddressCode == value) return;
			_shipAddressCode = value;
			NotifyPropertyChanged(nameof(ShipAddressCode));
		}
	}

	public string ShipAddressName
	{
		get => _shipAddressName;
		set
		{
			if (_shipAddressName == value) return;
			_shipAddressName = value;
			NotifyPropertyChanged(nameof(ShipAddressName));
		}
	}

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
