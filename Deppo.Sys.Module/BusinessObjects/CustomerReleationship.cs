using Deppo.Sys.Module.LogoBusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
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
[Appearance("CustomerReleationship Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
[Appearance("CustomerReleationship Edit", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
[Appearance("CustomerReleationship New", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
[ImageName("ProductQuickShippments")]
[NavigationItem("ProductManagement")]

[Persistent("CustomerReleationship")]

public class CustomerReleationship : XPLiteObject
{
    int _firmNumber;
    int referenceId;
    LOGO_Product _product;
    LOGO_Customer _customer;
    string _customerProductCode;
    string _customerProductName;
    string _customerProductBarcode;

    public CustomerReleationship(Session session)
        : base(session)
    {
    }
    public override void AfterConstruction()
    {
        base.AfterConstruction();

    }

    [ModelDefault("AllowEdit", "False")]
    public int FirmNumber { get => _firmNumber; set => SetPropertyValue(nameof(FirmNumber), ref _firmNumber, value); }

    [Key, Browsable(false)]
    public int ReferenceId { get => referenceId; set => SetPropertyValue(nameof(ReferenceId), ref referenceId, value); }

    [ModelDefault("AllowEdit", "False"), NoForeignKey]
    public LOGO_Product Product { get => _product; set => SetPropertyValue(nameof(Product), ref _product, value); }

    [ModelDefault("AllowEdit", "False"), NoForeignKey]
    public LOGO_Customer Customer { get => _customer; set => SetPropertyValue(nameof(Customer), ref _customer, value); }

    [ModelDefault("AllowEdit", "False")]
    public string CustomerProductCode { get => _customerProductCode; set => SetPropertyValue(nameof(CustomerProductCode), ref _customerProductCode, value); }

    [ModelDefault("AllowEdit", "False")]
    public string CustomerProductName { get => _customerProductName; set => SetPropertyValue(nameof(CustomerProductName), ref _customerProductName, value); }
    [ModelDefault("AllowEdit", "False")]
    public string CustomerProductBarcode { get => _customerProductBarcode; set => SetPropertyValue(nameof(CustomerProductBarcode), ref _customerProductBarcode, value); }

}