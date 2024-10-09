using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.ProcessBottomSheetModels;

public class ProcessBottomSheetModel : INotifyPropertyChanged, IDisposable
{
    public string _ınfoTitle;
    public string _description;
    private int? _icon;  // Nullable integer for the Icon (0 or 1)
    private string _iconColor;  // IconColor can be set dynamically
    private string _iconText;  // IconText can be set dynamically

    public ProcessBottomSheetModel()
    {
    }

    public string InfoTitle
    {
        get => _ınfoTitle;
        set
        {
            if (_ınfoTitle == value) return;
            _ınfoTitle = value;
            NotifyPropertyChanged();
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (_description == value) return;
            _description = value;
            NotifyPropertyChanged();
        }
    }

    // The Icon property can be used to trigger changes, but doesn't directly return IconText or IconColor
    public int? Icon
    {
        get => _icon;
        set
        {
            if (_icon == value) return;
            _icon = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(IconText));  // Notify when IconText changes
            NotifyPropertyChanged(nameof(IconColor)); // Notify when IconColor changes
        }
    }

    // IconText can be set independently from the Icon
    public string IconText
    {
        get => _iconText;  // Return the value of _iconText
        set
        {
            if (_iconText == value) return;
            _iconText = value;
            NotifyPropertyChanged();
        }
    }

    // IconColor can be set independently from the Icon
    public string IconColor
    {
        get => _iconColor;  // Return the value of _iconColor
        set
        {
            if (_iconColor == value) return;
            _iconColor = value;
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