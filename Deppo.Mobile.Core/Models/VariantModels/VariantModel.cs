using System;
using System.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.VariantModels;

public class VariantModel : Variant
{
    private bool _isSelected;
    private string? _image;
    

    public VariantModel()
    {

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

  
}
