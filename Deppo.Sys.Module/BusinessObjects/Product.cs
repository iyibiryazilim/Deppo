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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Deppo.Sys.Module.BusinessObjects
{
    [DefaultClassOptions]
    [Appearance("Product Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
    [Appearance("Product Edit", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
    [Appearance("Product New", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
    [NavigationItem("ProductManagement")]
    [ImageName("BO_Product")]
    public class Product : BaseObject
    {
        int _referenceId;
        string _code;
        string _name;
        Unitset _unitset;
        int _unitsetReferenceId;
        double _vat;
        string _group;
        string _speCode;
        string _speCode2;
        string _speCode3;
        string _speCode4;
        string _speCode5;
        int _firmNumber;
        IntegrationResult _result;
        string _producerCode;

        public Product(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            
        }

        [ModelDefault("AllowEdit", "False"),Browsable(false),ForeignKey("ReferenceId")]
        public int ReferenceId { get => _referenceId; set => SetPropertyValue(nameof(ReferenceId), ref _referenceId, value); }

        [ModelDefault("AllowEdit", "False")]
        public string Code { get => _code; set => SetPropertyValue(nameof(Code), ref _code, value); }

        [ModelDefault("AllowEdit", "False")]
        public string Name { get => _name; set => SetPropertyValue(nameof(Name), ref _name, value); }

        [ModelDefault("AllowEdit", "False")]
        public Unitset Unitset { get => _unitset; set => SetPropertyValue(nameof(Unitset), ref _unitset, value); }

        [ModelDefault("AllowEdit", "False")]
        public double Vat { get => _vat; set => SetPropertyValue(nameof(Vat), ref _vat, value); }

        [ModelDefault("AllowEdit", "False")]
        public string Group { get => _group; set => SetPropertyValue(nameof(Group), ref _group, value); }

        [ModelDefault("AllowEdit", "False")]
        public string SpeCode { get => _speCode; set => SetPropertyValue(nameof(SpeCode), ref _speCode, value); }

        [ModelDefault("AllowEdit", "False")]
        public string SpeCode2 { get => _speCode2; set => SetPropertyValue(nameof(SpeCode2), ref _speCode2, value); }

        [ModelDefault("AllowEdit", "False")]
        public string SpeCode3 { get => _speCode3; set => SetPropertyValue(nameof(SpeCode3), ref _speCode3, value); }

        [ModelDefault("AllowEdit", "False")]
        public string SpeCode4 { get => _speCode4; set => SetPropertyValue(nameof(SpeCode4), ref _speCode4, value); }

        [ModelDefault("AllowEdit", "False")]
        public string SpeCode5 { get => _speCode5; set => SetPropertyValue(nameof(SpeCode5), ref _speCode5, value); }


        [ModelDefault("AllowEdit", "False")]
        public int FirmNumber { get => _firmNumber; set => SetPropertyValue(nameof(FirmNumber), ref _firmNumber, value); }

        [ModelDefault("AllowEdit", "False")]
        public IntegrationResult Result { get => _result; set => SetPropertyValue(nameof(Result), ref _result, value); }

        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public int UnitsetReferenceId { get => _unitsetReferenceId; set => SetPropertyValue(nameof(UnitsetReferenceId), ref _unitsetReferenceId, value); }

        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public string ProducerCode { get => _producerCode; set => SetPropertyValue(nameof(ProducerCode), ref _producerCode, value); }

    }
}