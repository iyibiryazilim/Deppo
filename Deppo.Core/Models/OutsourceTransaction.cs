using System;
using System.ComponentModel;
using Deppo.Core.BaseModels;

namespace Deppo.Core.Models;

public class OutsourceTransaction : BaseTransaction
{
    private int _outsourceReferenceId;
    private string _outsourceCode = string.Empty;
    private string _outsourceName = string.Empty;

    public OutsourceTransaction()
    {

    }

    [Browsable(false)]
    public int OutsourceReferenceId
    {
        get => _outsourceReferenceId;
        set
        {
            if (_outsourceReferenceId == value) return;
            _outsourceReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public string OutsourceCode
    {
        get => _outsourceCode;
        set
        {
            if (_outsourceCode == value) return;
            _outsourceCode = value;
            NotifyPropertyChanged();
        }
    }

    public string OutsourceName
    {
        get => _outsourceName;
        set
        {
            if (_outsourceName == value) return;
            _outsourceName = value;
            NotifyPropertyChanged();
        }
    }
}
