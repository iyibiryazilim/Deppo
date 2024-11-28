using Deppo.Sys.Module.BusinessObjects;
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

namespace Deppo.Sys.Module.BaseBusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem(false)]
    [DefaultProperty("FullName")]
    [Appearance("Current Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
    [Appearance("Current Edit", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
    [Appearance("Current New", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
    public class Current : BaseObject
    {
        string _code;
        string _title;
        string _taxOffice;
        string _taxNumber;
        BusinessObjects.Country _country;
        City _city;
        County _county;
        string _postCode;
        Currency _currency;
        bool _eInvoice = false;
        string _telephone;
        string _otherTelephone;
        string _fax;
        string _eMail;
        string _webAddress;
        string _address;
        decimal _creditLimit;
        bool _creditHold = false;
        int _referenceId;
        bool _isActive = false;
        bool _isPersonal = false;
        string _firstName;
        string _lastName;
        double _customerDiscountRate;
        string _tckn;
        bool _isForeign = false;
        CustomerType _customerType;
        IntegrationResult _result;
        int _firmNumber;

       

        public Current(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();

        }
        [ModelDefault("AllowEdit", "False")]
        public string Code { get => _code; set => SetPropertyValue(nameof(Code), ref _code, value); }

        [ModelDefault("AllowEdit", "False")]
        public string Title { get => _title; set => SetPropertyValue(nameof(Title), ref _title, value); }

        [ModelDefault("AllowEdit", "False")]
        public string TaxOffice { get => _taxOffice; set => SetPropertyValue(nameof(TaxOffice), ref _taxOffice, value); }

        [ModelDefault("AllowEdit", "False")]
        public string TaxNumber { get => _taxNumber; set => SetPropertyValue(nameof(TaxNumber), ref _taxNumber, value); }

        [ModelDefault("AllowEdit", "False")]
        public BusinessObjects.Country Country { get => _country; set => SetPropertyValue(nameof(Country), ref _country, value); }

        [ModelDefault("AllowEdit", "False")]
        public City City { get => _city; set => SetPropertyValue(nameof(City), ref _city, value); }

        [ModelDefault("AllowEdit", "False")]
        public County County { get => _county; set => SetPropertyValue(nameof(County), ref _county, value); }

        [ModelDefault("AllowEdit", "False")]
        public string PostCode { get => _postCode; set => SetPropertyValue(nameof(PostCode), ref _postCode, value); }

        [ModelDefault("AllowEdit", "False")]
        public Currency Currency { get => _currency; set => SetPropertyValue(nameof(Currency), ref _currency, value); }

        [ModelDefault("AllowEdit", "False")]
        public bool EInvoice { get => _eInvoice; set => SetPropertyValue(nameof(EInvoice), ref _eInvoice, value); }

        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("EditMaskType", "Simple")]
        [ModelDefault("EditMask", "(999) 000-0000")]
        public string Telephone { get => _telephone; set => SetPropertyValue(nameof(Telephone), ref _telephone, value); }

        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("EditMaskType", "Simple")]
        [ModelDefault("EditMask", "(999) 000-0000")]
        public string OtherTelephone { get => _otherTelephone; set => SetPropertyValue(nameof(OtherTelephone), ref _otherTelephone, value); }

        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("EditMaskType", "Simple")]
        [ModelDefault("EditMask", "(999) 000-0000")]
        public string Fax { get => _fax; set => SetPropertyValue(nameof(Fax), ref _fax, value); }

        [RuleRegularExpression("RuleRegularExpression Customer.BillingEMail", DefaultContexts.Save, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|" + @"(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")]
        [ModelDefault("AllowEdit", "False")]
        public string EMail { get => _eMail; set => SetPropertyValue(nameof(EMail), ref _eMail, value); }

        [ModelDefault("AllowEdit", "False")]
        public string WebAddress { get => _webAddress; set => SetPropertyValue(nameof(WebAddress), ref _webAddress, value); }

        [ModelDefault("AllowEdit", "False")]
        public string Address { get => _address; set => SetPropertyValue(nameof(Address), ref _address, value); }

        [ModelDefault("AllowEdit", "False")]
        public decimal CreditLimit { get => _creditLimit; set => SetPropertyValue(nameof(CreditLimit), ref _creditLimit, value); }

        [ModelDefault("AllowEdit", "False")]
        public bool CreditHold { get => _creditHold; set => SetPropertyValue(nameof(CreditHold), ref _creditHold, value); }

        [Browsable(false)]
        public int ReferenceId { get => _referenceId; set => SetPropertyValue(nameof(ReferenceId), ref _referenceId, value); }

        [ModelDefault("AllowEdit", "False")]
        public bool IsActive { get => _isActive; set => SetPropertyValue(nameof(IsActive), ref _isActive, value); }

        [ModelDefault("AllowEdit", "False")]
        public bool IsPersonal { get => _isPersonal; set => SetPropertyValue(nameof(IsPersonal), ref _isPersonal, value); }

        [ModelDefault("AllowEdit", "False")]
        public string FirstName
        {
            get => _firstName; set => SetPropertyValue(nameof(FirstName), ref _firstName, value);
        }

        [ModelDefault("AllowEdit", "False")]
        public string LastName
        {
            get => _lastName; set => SetPropertyValue(nameof(LastName), ref _lastName, value);
        }

        [ModelDefault("AllowEdit", "False")]
        public double CustomerDiscountRate { get => _customerDiscountRate; set => SetPropertyValue(nameof(CustomerDiscountRate), ref _customerDiscountRate, value); }

        [ModelDefault("AllowEdit", "False")]
        public string Tckn
        {
            get => _tckn; set => SetPropertyValue(nameof(Tckn), ref _tckn, value);
        }

        [ModelDefault("AllowEdit", "False")]
        public bool IsForeign { get => _isForeign; set => SetPropertyValue(nameof(IsForeign), ref _isForeign, value); }

        [ModelDefault("AllowEdit", "False")]
        public CustomerType CustomerType { get => _customerType; set => SetPropertyValue(nameof(CustomerType), ref _customerType, value); }

        [ModelDefault("AllowEdit", "False")]
        public IntegrationResult Result { get => _result; set => SetPropertyValue(nameof(Result), ref _result, value); }

        [ModelDefault("AllowEdit", "False")]
        public int FirmNumber { get => _firmNumber; set => SetPropertyValue(nameof(FirmNumber), ref _firmNumber, value); }

        public string FullName
        {
            get
            {
                if (IsPersonal)
                {
                    return $"{FirstName} {LastName}";
                }
                else
                {
                    return Title;
                }
            }
        }

    }
}