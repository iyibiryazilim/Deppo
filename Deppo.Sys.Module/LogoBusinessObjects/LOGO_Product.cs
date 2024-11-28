using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Deppo.Sys.Module.LogoBusinessObjects;

[DefaultClassOptions]
[Appearance("LOGO_Product Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
[Appearance("LOGO_Product Edit", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
[Appearance("LOGO_Product New", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
[Persistent("LOGO_Product")]
[NavigationItem(false)]
public class LOGO_Product : XPLiteObject
{
    public LOGO_Product(Session session) : base(session) { }

    [Key, Browsable(false)]
    public int ReferenceId { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public int CardType { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public string Code { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public string Name { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public double Vat { get; set; }

    [ModelDefault("AllowEdit", "False"), Browsable(false)]
    public int UnitsetReferenceId { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public string Specode { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public string Specode2 { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public string Specode3 { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public string Specode4 { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public string Specode5 { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public string ProductGroup { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public bool IsActive { get; set; }

    [ModelDefault("AllowEdit", "False")]
    public int FirmNumber { get; set; }
}
