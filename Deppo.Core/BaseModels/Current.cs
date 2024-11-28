using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Deppo.Core.BaseModels;

public class Current : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private string _code = string.Empty;
    private string _title = string.Empty;
    private bool isPersonal;
    private string _tckn = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private string _telephone = string.Empty;
    private string _address = string.Empty;
    private string _city = string.Empty;
    private string _country = string.Empty;
    private string _postalCode = string.Empty;
    private string _taxOffice = string.Empty;
    private string _taxNumber = string.Empty;
    private int _orderReferenceCount;
    private int _shipAddressCount;
    private bool _isActive;
    private bool _isEDispatch;

    //
    private string _name = string.Empty;

    public string TitleName => Name?.Length > 2 ? Name.Substring(0, 2) : Name;

    public Current()
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
            NotifyPropertyChanged();
        }
    }

    public string Code
    {
        get => _code;
        set
        {
            if (_code == value) return;
            _code = value;
            NotifyPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) return;
            _title = value;
            NotifyPropertyChanged();
        }
    }

    public bool IsPersonal
    {
        get => isPersonal;
        set
        {
            if (isPersonal == value) return;
            isPersonal = value;
            NotifyPropertyChanged();
        }
    }

    public string Tckn
    {
        get => _tckn;
        set
        {
            if (_tckn == value) return;
            _tckn = value;
            NotifyPropertyChanged();
        }
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (_firstName == value) return;
            _firstName = value;
            NotifyPropertyChanged();
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if (_lastName == value) return;
            _lastName = value;
            NotifyPropertyChanged();
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (_email == value) return;
            _email = value;
            NotifyPropertyChanged();
        }
    }

    public string Telephone
    {
        get => _telephone;
        set
        {
            if (_telephone == value) return;
            _telephone = value;
            NotifyPropertyChanged();
        }
    }

    public string Address
    {
        get => _address;
        set
        {
            if (_address == value) return;
            _address = value;
            NotifyPropertyChanged();
        }
    }

    public string City
    {
        get => _city;
        set
        {
            if (_city == value) return;
            _city = value;
            NotifyPropertyChanged();
        }
    }

    public string Country
    {
        get => _country;
        set
        {
            if (_country == value) return;
            _country = value;
            NotifyPropertyChanged();
        }
    }

    public string PostalCode
    {
        get => _postalCode;
        set
        {
            if (_postalCode == value) return;
            _postalCode = value;
            NotifyPropertyChanged();
        }
    }

    public string TaxOffice
    {
        get => _taxOffice;
        set
        {
            if (_taxOffice == value) return;
            _taxOffice = value;
            NotifyPropertyChanged();
        }
    }

    public string TaxNumber
    {
        get => _taxNumber;
        set
        {
            if (_taxNumber == value) return;
            _taxNumber = value;
            NotifyPropertyChanged();
        }
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;
            _isActive = value;
            NotifyPropertyChanged();
        }
    }

    public bool IsEDispatch
    {
        get => _isEDispatch;
        set
        {
            if (_isEDispatch == value) return;
            _isEDispatch = value;
            NotifyPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value) return;
            _name = value;
            NotifyPropertyChanged();
        }
    }

    public int OrderReferenceCount
    {
        get => _orderReferenceCount;
        set
        {
            if (_orderReferenceCount == value) return;
            _orderReferenceCount = value;
            NotifyPropertyChanged();
        }
    }

    public int ShipAddressCount
    {
        get => _shipAddressCount;
        set
        {
            if (_shipAddressCount == value) return;
            _shipAddressCount = value;
            NotifyPropertyChanged();
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