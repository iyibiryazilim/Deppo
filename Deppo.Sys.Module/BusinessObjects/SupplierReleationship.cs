using Deppo.Sys.Module.LogoBusinessObjects;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Deppo.Sys.Module.BusinessObjects;

[DefaultClassOptions]
[Appearance("SupplierReleationship Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
[Appearance("SupplierReleationship Edit", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
[Appearance("SupplierReleationship New", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
[ImageName("ProductQuickShippments")]
[NavigationItem("ProductManagement")]
[Persistent("SupplierReleationship")]
public class SupplierReleationship : XPLiteObject
{
   
    int _firmNumber;
    int referenceId;
    LOGO_Product _product;
    LOGO_Supplier _supplier;
    string _supplierProductCode;
    string _supplierProductName;
    string _supplierProductBarcode;


    public SupplierReleationship(Session session)
        : base(session)
    {
    }
  


    [ModelDefault("AllowEdit", "False")]
    public int FirmNumber { get => _firmNumber; set => SetPropertyValue(nameof(FirmNumber), ref _firmNumber, value); }

    [Key,Browsable(false)]
    public int ReferenceId { get => referenceId; set => SetPropertyValue(nameof(ReferenceId), ref referenceId, value); }

    [ModelDefault("AllowEdit", "False"),NoForeignKey]
    public LOGO_Product Product { get => _product; set => SetPropertyValue(nameof(Product), ref _product, value); }

    [ModelDefault("AllowEdit", "False"), NoForeignKey]
    public LOGO_Supplier Supplier { get => _supplier; set => SetPropertyValue(nameof(Supplier), ref _supplier, value); }

    [ModelDefault("AllowEdit", "False")]
    public string SupplierProductCode { get => _supplierProductCode; set => SetPropertyValue(nameof(SupplierProductCode), ref _supplierProductCode, value); }

    [ModelDefault("AllowEdit", "False")]
    public string SupplierProductName { get => _supplierProductName; set => SetPropertyValue(nameof(SupplierProductName), ref _supplierProductName, value); }
    [ModelDefault("AllowEdit", "False")]
    public string SupplierProductBarcode { get => _supplierProductBarcode; set => SetPropertyValue(nameof(SupplierProductBarcode), ref _supplierProductBarcode, value); }





}