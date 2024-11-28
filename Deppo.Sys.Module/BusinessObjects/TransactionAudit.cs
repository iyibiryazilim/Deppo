using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deppo.Sys.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("Audit")]
[ImageName("Action_Chart_Options")]
public class TransactionAudit : BaseObject
{
    private Guid _applicationUserOid;
    private ApplicationUser _applicationUser;
    private DateTime _createdOn;
    private int _transactionReferenceId;
    private string _transactionNumber;
    private DateTime _transactionDate;
    private string _documentNumber;

    private int _currentReferenceId;
    private string _currentName;
    private string _currentCode;
    private int _shipAddressReferenceId;
    private string _shipAddressName;
    private string _shipAddressCode;

    private int _warehouseNumber;
    private string _warehouseName;

    private int _productReferenceCount;
    private int _firmNumber;
    private int _periodNumber;
    private int _ioType;
    private int _transactionType;

    public TransactionAudit(Session session)
        : base(session)
    {
    }

    public override void AfterConstruction()
    {
        base.AfterConstruction();
    }

    protected override void OnChanged(string propertyName, object oldValue, object newValue)
    {
        base.OnChanged(propertyName, oldValue, newValue);
        if (!Session.IsObjectMarkedDeleted(this))
        {
            switch (propertyName)
            {
                case nameof(ApplicationUserOid):
                    ApplicationUser = Session.GetObjectByKey<ApplicationUser>(ApplicationUserOid);
                    this.RaisePropertyChangedEvent(nameof(ApplicationUser));
                    break;

                default:
                    break;
            }
        }
    }

    [VisibleInListView(false), VisibleInDetailView(false), ModelDefault("AllowEdit", "False"),Browsable(false)]
    public Guid ApplicationUserOid { get => _applicationUserOid; set => SetPropertyValue(nameof(ApplicationUserOid), ref _applicationUserOid, value); }

    public ApplicationUser ApplicationUser { get => _applicationUser; set => SetPropertyValue(nameof(ApplicationUser), ref _applicationUser, value); }

    [ModelDefault("AllowEdit", "False")]
    public DateTime CreatedOn { get => _createdOn; set => SetPropertyValue(nameof(CreatedOn), ref _createdOn, value); }

    [ModelDefault("AllowEdit", "False"), Browsable(false)]
    public int TransactionReferenceId { get => _transactionReferenceId; set => SetPropertyValue(nameof(TransactionReferenceId), ref _transactionReferenceId, value); }

    [ModelDefault("AllowEdit", "False")]
    public string TransactionNumber { get => _transactionNumber; set => SetPropertyValue(nameof(TransactionNumber), ref _transactionNumber, value); }

    [ModelDefault("AllowEdit", "False")]
    public DateTime TransactionDate { get => _transactionDate; set => SetPropertyValue(nameof(TransactionDate), ref _transactionDate, value); }

    [ModelDefault("AllowEdit", "False")]
    public string DocumentNumber { get => _documentNumber; set => SetPropertyValue(nameof(DocumentNumber), ref _documentNumber, value); }

    [ModelDefault("AllowEdit", "False"), Browsable(false)]
    public int CurrentReferenceId { get => _currentReferenceId; set => SetPropertyValue(nameof(CurrentReferenceId), ref _currentReferenceId, value); }

    [ModelDefault("AllowEdit", "False")]
    public string CurrentName { get => _currentName; set => SetPropertyValue(nameof(CurrentName), ref _currentName, value); }

    [ModelDefault("AllowEdit", "False")]
    public string CurrentCode { get => _currentCode; set => SetPropertyValue(nameof(CurrentCode), ref _currentCode, value); }

    [ModelDefault("AllowEdit", "False"), Browsable(false)]
    public int ShipAddressReferenceId { get => _shipAddressReferenceId; set => SetPropertyValue(nameof(ShipAddressReferenceId), ref _shipAddressReferenceId, value); }

    [ModelDefault("AllowEdit", "False")]
    public string ShipAddressName { get => _shipAddressName; set => SetPropertyValue(nameof(ShipAddressName), ref _shipAddressName, value); }

    [ModelDefault("AllowEdit", "False")]
    public string ShipAddressCode { get => _shipAddressCode; set => SetPropertyValue(nameof(ShipAddressCode), ref _shipAddressCode, value); }

    [ModelDefault("AllowEdit", "False")]
    public int WarehouseNumber { get => _warehouseNumber; set => SetPropertyValue(nameof(WarehouseNumber), ref _warehouseNumber, value); }

    [ModelDefault("AllowEdit", "False")]
    public string WarehouseName { get => _warehouseName; set => SetPropertyValue(nameof(WarehouseName), ref _warehouseName, value); }

    [ModelDefault("AllowEdit", "False")]
    public int ProductReferenceCount { get => _productReferenceCount; set => SetPropertyValue(nameof(ProductReferenceCount), ref _productReferenceCount, value); }

    [ModelDefault("AllowEdit", "False")]
    public int FirmNumber { get => _firmNumber; set => SetPropertyValue(nameof(FirmNumber), ref _firmNumber, value); }

    [ModelDefault("AllowEdit", "False")]
    public int PeriodNumber { get => _periodNumber; set => SetPropertyValue(nameof(PeriodNumber), ref _periodNumber, value); }

    [ModelDefault("AllowEdit", "False")]
    public int IOType { get => _ioType; set => SetPropertyValue(nameof(IOType), ref _ioType, value); }

    [ModelDefault("AllowEdit", "False")]
    public int TransactionType { get => _transactionType; set => SetPropertyValue(nameof(TransactionType), ref _transactionType, value); }


}