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

    private int _charCodeReferenceId = default;
    private string _charCodeName = string.Empty;
    private int _charValReferenceId;
    private string _charValName = string.Empty;



    public int CharCodeReferenceId
    {
        get => _charCodeReferenceId;
        set
        {
            if (_charCodeReferenceId == value) return;
            _charCodeReferenceId = value;
            NotifyPropertyChanged();
        }
    }
    

    public string CharCodeName
        {
        get => _charCodeName;
        set
        {
            if (_charCodeName == value) return;
            _charCodeName = value;
            NotifyPropertyChanged();
        }
    }


    public int CharValReferenceId
    {
        get => _charValReferenceId;
        set
        {
            if (_charValReferenceId == value) return;
            _charValReferenceId = value;
            NotifyPropertyChanged();
        }
    }


    public string CharValName
    {
        get => _charValName;
        set
        {
            if (_charValName == value) return;
            _charValName = value;
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

