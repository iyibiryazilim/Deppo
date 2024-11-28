using System.ComponentModel;
using Deppo.Core.BaseModels;

namespace Deppo.Core.Models;

public class WarehouseTransaction : BaseTransaction
{
    private int _warehouseReferenceId;
    private int _currentReferenceId;
    private string _currentCode = string.Empty;
    private string _currentName = string.Empty;
    private string? _image;

    public WarehouseTransaction()
    {
    }

    [Browsable(false)]
    public int WarehouseReferenceId
    {
        get => _warehouseReferenceId;
        set
        {
            if (_warehouseReferenceId == value) return;
            _warehouseReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    [Browsable(false)]
    public int CurrentReferenceId
    {
        get => _currentReferenceId;
        set
        {
            if (_currentReferenceId == value)
                return;
            _currentReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public string CurrentCode
    {
        get => _currentCode;
        set
        {
            if (_currentCode == value)
                return;
            _currentCode = value;
            NotifyPropertyChanged();
        }
    }

    public string CurrentName
    {
        get => _currentName;
        set
        {
            if (_currentName == value)
                return;
            _currentName = value;
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