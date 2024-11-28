using Deppo.Sys.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Module.CustomLogonObjects;

[DomainComponent, Serializable]
[System.ComponentModel.DisplayName("Oturum Aç")]
public class CustomLogonParameters : INotifyPropertyChanged,IDisposable, IServiceProviderConsumer
{
    private int companyNumber;
    private string userName;
    private string password;
    IServiceProvider? serviceProvider;
    readonly List<IDisposable> objToDispose = new List<IDisposable>();

    public CustomLogonParameters()
    {
        companyNumber = 1;
        userName = string.Empty;
        password = string.Empty;
    }

    [RuleRange(1, 999),RuleValueComparison(ValueComparisonType.GreaterThanOrEqual, 1)]
    public int CompanyNumber
    {
        get { return companyNumber; }
        set
        {
            if (companyNumber == value) return;
            companyNumber = value;
            OnPropertyChanged("CompanyNumber");
        }
    }
    

    public string UserName
    {
        get { return userName; }
        set
        {
            if (userName == value) return;
            userName = value;
            OnPropertyChanged("UserName");
        }
    }

    [PasswordPropertyText(true)]
    public string Password
    {
        get { return password; }
        set
        {
            if (password == value) return;
            password = value;
        }
    }

    void IDisposable.Dispose()
    {
        foreach (IDisposable disposable in objToDispose)
        {
            disposable.Dispose();
        }
        serviceProvider = null;
    }

    void IServiceProviderConsumer.SetServiceProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    private void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void RefreshPersistentObjects(IObjectSpace objectSpace)
    {
        //ApplicationUser = (UserName == null) ? null : objectSpace.FirstOrDefault<ApplicationUser>(e => e.UserName == UserName);
    }

    


}
