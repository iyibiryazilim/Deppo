using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Deppo.Core.Models;

/// <summary>
/// Firma - L_CAPIFIRM
/// </summary>
public class Company : INotifyPropertyChanged,IDisposable
{
    private int _referenceId;
    private string _name = string.Empty;
    private int _number;
    private int _periodNumber;

    public Company()
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

    public int Number
    {
        get => _number;
        set
        {
            if (_number == value) return;
            _number = value;
            NotifyPropertyChanged();
        }
    }

    public int PeriodNumber
    {
        get => _periodNumber;
        set
        {
            if (_periodNumber == value) return;
            _periodNumber = value;
            NotifyPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
