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
public class TransactionAudit : BaseObject
{
    private ApplicationUser _applicationUser;
    private DateTime _createdOn;
    private string _transactionNumber;
    private DateTime _transactionDate;
    private string _documentNumber;

    private int _currentReferenceId;
    private string _currentName;
    private string _currentCode;
    private int _shipAddressReferenceId;
    private string _shipAdressName;
    private string _shipAddressCode;

    private string _warehouseNumber;
    private string _warehouseName;

    private int _productReferenceCount;

    public TransactionAudit(Session session)
        : base(session)
    {
    }

    public override void AfterConstruction()
    {
        base.AfterConstruction();
    }

    [ModelDefault("AllowEdit", "False")]
    public ApplicationUser ApplicationUser { get => _applicationUser; set => SetPropertyValue(nameof(ApplicationUser), ref _applicationUser, value); }

    [ModelDefault("AllowEdit", "False")]
    public DateTime CreatedOn { get => _createdOn; set => SetPropertyValue(nameof(CreatedOn), ref _createdOn, value); }

    [ModelDefault("AllowEdit", "False")]
    public string TransactionNumber { get => _transactionNumber; set => SetPropertyValue(nameof(TransactionNumber), ref _transactionNumber, value); }

    [ModelDefault("AllowEdit", "False")]
    public DateTime TransactionDate { get => _transactionDate; set => SetPropertyValue(nameof(TransactionDate), ref _transactionDate, value); }

    [ModelDefault("AllowEdit", "False")]
    public string DocumentNumber { get => _documentNumber; set => SetPropertyValue(nameof(DocumentNumber), ref _documentNumber, value); }

    [ModelDefault("AllowEdit", "False")]
    public int CurrentReferenceId { get => _currentReferenceId; set => SetPropertyValue(nameof(CurrentReferenceId), ref _currentReferenceId, value); }

    [ModelDefault("AllowEdit", "False")]
    public string CurrentName { get => _currentName; set => SetPropertyValue(nameof(CurrentName), ref _currentName, value); }

    [ModelDefault("AllowEdit", "False")]
    public string CurrentCode { get => _currentCode; set => SetPropertyValue(nameof(CurrentCode), ref _currentCode, value); }

    [ModelDefault("AllowEdit", "False")]
    public int ShipAddressReferenceId { get => _shipAddressReferenceId; set => SetPropertyValue(nameof(ShipAddressReferenceId), ref _shipAddressReferenceId, value); }

    [ModelDefault("AllowEdit", "False")]
    public string ShipAdressName { get => _shipAdressName; set => SetPropertyValue(nameof(ShipAdressName), ref _shipAdressName, value); }

    [ModelDefault("AllowEdit", "False")]
    public string ShipAdressCode { get => _shipAddressCode; set => SetPropertyValue(nameof(ShipAdressCode), ref _shipAddressCode, value); }

    [ModelDefault("AllowEdit", "False")]
    public string WarehouseNumber { get => _warehouseNumber; set => SetPropertyValue(nameof(WarehouseNumber), ref _warehouseNumber, value); }

    [ModelDefault("AllowEdit", "False")]
    public string WarehouseName { get => _warehouseName; set => SetPropertyValue(nameof(WarehouseName), ref _warehouseName, value); }

    [ModelDefault("AllowEdit", "False")]
    public int ProductReferenceCount { get => _productReferenceCount; set => SetPropertyValue(nameof(ProductReferenceCount), ref _productReferenceCount, value); }
}