using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Models;

public class ProductVariant : INotifyPropertyChanged, IDisposable
{

    private int _variantPropertyReferenceId = default;
    private string _variantPropertyCode = string.Empty;
    private string _variantPropertyName = string.Empty;

    private int _variantPropertyValueReferenceId = default;
    private string _variantPropertyValueCode = string.Empty;
    private string _variantPropertyValueName = string.Empty;

    private int _variantReferenceId = default;
    private string _variantName = string.Empty;
    private string _variantCode = string.Empty;

    private int _itemReferenceId = default;
    private string _itemName = string.Empty;
    private string _itemCode = string.Empty;

   
    public int VariantPropertyReferenceId
        {
        get => _variantPropertyReferenceId;
        set
        {
            if (_variantPropertyReferenceId == value) return;
            _variantPropertyReferenceId = value;
            NotifyPropertyChanged();
        }
    }
    public string VariantPropertyCode
        {
        get => _variantPropertyCode;
        set
        {
            if (_variantPropertyCode == value) return;
            _variantPropertyCode = value;
            NotifyPropertyChanged();
        }
    }
    public string VariantPropertyName
        {
        get => _variantPropertyName;
        set
        {
            if (_variantPropertyName == value) return;
            _variantPropertyName = value;
            NotifyPropertyChanged();
        }
    }



    public int VariantPropertyValueReferenceId
        {
        get => _variantPropertyValueReferenceId;
        set
        {
            if (_variantPropertyValueReferenceId == value) return;
            _variantPropertyValueReferenceId = value;
            NotifyPropertyChanged();
        }
    }
    public string VariantPropertyValueCode
        {
        get => _variantPropertyValueCode;
        set
        {
            if (_variantPropertyValueCode == value) return;
            _variantPropertyValueCode = value;
            NotifyPropertyChanged();
        }
    }
    public string VariantPropertyValueName
        {
        get => _variantPropertyValueName;
        set
        {
            if (_variantPropertyValueName == value) return;
            _variantPropertyValueName = value;
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

    public int ItemReferenceId
        {
        get => _itemReferenceId;
        set
        {
            if (_itemReferenceId == value) return;
            _itemReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public string ItemName
        {
        get => _itemName;
        set
        {
            if (_itemName == value) return;
            _itemName = value;
            NotifyPropertyChanged();
        }
    }

    public string ItemCode
    {
        get => _itemCode;
        set
        {
            if (_itemCode == value) return;
            _itemCode = value;
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

