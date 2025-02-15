using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{

    public class Customer
    {
        public string KeyCloakId { get; set; }

        public int? CustomerId { get; set; }

        public EntrepreneurialStatus? EntrepreneurialStatus { get; set; }

        #region PersonalInfo

        public string PersonalNo { get; set; }

        public string FirstNameGe { get; set; }

        public string LastNameGe { get; set; }

        public string PatronymicNameGe { get; set; }

        public string FirstNameEn { get; set; }

        public string LastNameEn { get; set; }

        public string PatronymicNameEn { get; set; }

        public DateTime? BirthDate { get; set; }

        public string BirthCountry { get; set; }

        public string BirthCity { get; set; }

        public Sex? Sex { get; set; }

        public string Citizenship { get; set; }

        public string DoubleCitizenship { get; set; }

        #endregion

        #region ContactInfo

        public string FinancialNumber { get; set; }

        public string AdditionalMobileNumber { get; set; }

        public bool? MarketingSms { get; set; }

        public string Email { get; set; }

        #region Legal
        public string LegalAddressCountry { get; set; }

        public string LegalAddressCityGe { get; set; }

        public string LegalAddressCityEn { get; set; }

        public int? LegalAddressRegionId { get; set; }

        public int? LegalAddressDistrictId { get; set; }

        public string LegalAddressGe { get; set; }

        public string LegalAddressEn { get; set; }

        #endregion

        #region Actual
        public string ActualAddressCountry { get; set; }

        public string ActualAddressCityGe { get; set; }

        public string ActualAddressCityEn { get; set; }

        public int? ActualAddressRegionId { get; set; }

        public int? ActualAddressDistrictId { get; set; }

        public string ActualAddressGe { get; set; }

        public string ActualAddressEn { get; set; }

        #endregion

        #endregion

        #region Documents

        public DocumentType? DocumentType { get; set; }

        public string DocumentNo { get; set; }

        public DateTime? DocumentIssueDate { get; set; }

        public DateTime? DocumentExpireDate { get; set; }

        public string DocumentIssuer { get; set; }

        public string DocumentIssuerCountry { get; set; }

        public string DocumentPhotoFrontBase64 { get; set; }

        public string DocumentPhotoBackBase64 { get; set; }

        #endregion

        #region WorkInfo

        public string OrganizationName { get; set; }

        public string JobPosition { get; set; }

        public string FieldOfActivityCode { get; set; }

        #endregion

        public bool? IsIdentifiedOnline { get; set; }

        public string Channel { get; set; }

        public bool? isResident { get;set;}

        public bool? isPep { get; set; }

    }

    public enum Sex
    {
        MALE,
        FEMALE
    }

    public enum DocumentType
    {
        TEMPORARY_RESIDENCE_PERMIT,
        PERMANENT_RESIDENCE_PERMIT,
        BIRTH_CERTIFICATE,
        TEMPORARY_ID,
        COMPATRIOT_CERTIFICATE,
        DRIVING_LICENSE,
        NEUTRAL_ID,
        NEUTRAL_TRAVEL_DOC,
        ID,
        BORDER_CROSSING_DOC,
        TRAVEL_PASSPORT,
        PASSPORT_LOCAL,
        PASSPORT_USSR,
        NO_PASSPORT,
        PASSPORT_FOREIGN
    }

    public enum EntrepreneurialStatus
    {
        IE,
        PHYSICAL_PERSON,
        PAYER_PHYSICAL_PERSON
    }
}
