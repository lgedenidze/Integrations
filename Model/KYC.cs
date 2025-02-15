using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public class KYC
    {
        public int Id { get; set; }

        public int customerNo { get; set; }

        public List<string> employmentStatusCodes { get; set; }

        public List<string> incomeSourceCodes { get; set; }

        public List<EmployerInfo> employerInfoList { get; set; }

        public List<ActivityInfo> activityInfoList { get; set; }

        public string monthlyIncomeCode { get; set; }

        public List<string> bankRelationPurposeCodes { get; set; }

        public List<string> bankRelationProductCodes { get; set; }

        public string expectedTransactionCountCode { get; set; }

        public string expectedTransactionAmountCode { get; set; }

        public string serviceReceiveChannelCode { get; set; }

        public bool? hasInternationalTransactions { get; set; }

        public List<string> internationTransactionCountries { get; set; }

        public string accountOpenReasonInGeorgia { get; set; }

        public bool? hasGreenCard { get; set; }

        public bool? isForeignCountryPayer { get; set; }

        public List<ForeignCountryTaxPayerInfo> foreignCountryTaxPayerInfoList { get; set; }

        public bool? isPEP { get; set; }

        public PEPDetails pepDetails { get; set; }

        public bool? hasAccountInOtherBanks { get; set; }

        public List<OtherBankAccountInfo> otherBankAccountInfoList { get; set; }

        public bool? actsForOtherPerson { get; set; }

        public List<OtherPersonInfo> otherPersonInfoList { get; set; }

        public List<PropertyInfo> PropertyInfoList { get; set; }
    }

    public class PEPDetails
    {
        public bool? isPepCivilServant { get; set; }

        public string pepFamilyMemberTypeCode { get; set; }

        public string pepBusinesRelationCode { get; set; }

        public string relatedPepFullName { get; set; }

        public string pepPositionCode { get; set; }

        public string pepCountry { get; set; }

        public DateTime? pepEndDate { get; set; }

        public string pepCategoryCode { get; set; }
    }

    public class ForeignCountryTaxPayerInfo
    {
        public string id { get; set; }

        public string country { get; set; }

        public string taxNumber { get; set; }
    }

    public class OtherBankAccountInfo
    {
        public string id { get; set; }

        public string bankName { get; set; }

        public string country { get; set; }
    }

    public class OtherPersonInfo
    {
        public string id { get; set; }

        public string personName { get; set; }

        public string identNumber { get; set; }
    }

    public class EmployerInfo
    {
        public int? id { get; set; }

        public string name { get; set; }

        public string identNumber { get; set; }

        public string address { get; set; }

        public string webSite { get; set; }

        public string fieldOfActivityCode { get; set; }

        public string activityCountry { get; set; }

        public string position { get; set; }
    }

    public class ActivityInfo
    {
        public int? id { get; set; }

        public string name { get; set; }

        public string identNumber { get; set; }

        public string address { get; set; }

        public string webSite { get; set; }

        public string fieldOfActivityCode { get; set; }

        public string activityCountry { get; set; }

    }

    public class PropertyInfo
    {
        public int? id { get; set; }

        public string propertyTypeCode { get; set; }

        public double? price { get; set; }

        public string currency { get; set; }

        public List<string> propertyOriginCodes { get; set; }
    }



}
