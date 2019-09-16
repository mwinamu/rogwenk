using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class CustomerSurvey
    {
        public int CustomerSurveyID { get; set; }
        public int CustomerID { get; set; }
        public string LastPurchased { get; set; }
        public string PaymentMethod { get; set; }
        public string MpesaNumber { get; set; }
        public int TimesPurchasedLastWeekFromPro { get; set; }
        public string RegularContact { get; set; }
        public string TimesPurchasedLastWeekFromOthers { get; set; }
        public int RegularSuppliers { get; set; }
        public string PrefferedVisitDays { get; set; }
        public string WantsAuthorizedProgasReseller { get; set; }
        public string WantsExclusiveProgasDealer { get; set; }
        public string ContactNumber { get; set; }
        public string SmsNumber { get; set; }
        public string DateAdded { get; set; }
        public string CreatedBy { get; set; }
        public string First { get; set; }
        public string Second { get; set; }
        public string Third { get; set; }
        public string VOC { get; set; }
    }
    public class CustomerSurveyDetails
    {
        public int CustomerSurveyDetailID { get; set; }
        public int CustomerSurveyID { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public string DateAdded { get; set; }
        public string CreatedBy { get; set; }
        public string Type { get; set; }
    }
    public class CustomerSurveyPurchased {
        public int ID { get; set; }
        public int CustomerSurveyID { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string Type { get; set; }
        public string DateAdded { get; set; }
        public string CreatedBycvc { get; set; }

    }
    public class CallCenterPostModel
    {
        public CallCenterModel callCenter { get; set; }
        public List<CallCenterAnySuppierModel> anySupplier { get; set; }
        public List<CallCenterPurchased> purchased { get; set; }
        public List<CallCenterWeeklyPurchaseModel> weekly { get; set; }
    }
    public class CallCenterTop3Performers
    {
        public int Id { get; set; }
        public int CallCenterId { get; set; }
        public string First { get; set; }
        public string Second { get; set; }
        public string Third { get; set; }
        public string AddedAt { get; set; }
        public string AddedBy { get; set; }

    }

    public class CallCenterPurchased
    {
        public int Id { get; set; }
        public int CallCenterId { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string Price { get; set; }
        public string AddedAt { get; set; }
        public string AddedBy { get; set; }
        public string Type { get; set; }
    }
    public class CallCenterModel
    {
        public int Id { get; set; }
        public int OutletId { get; set; }
        public string LastPurchased { get; set; }
        public string PaymentMethod { get; set; }
        public string MpesaNumber { get; set; }
        public string TimesPurchasedLastWeekFromPro { get; set; }
        public string RegularContact { get; set; }
        public string PrefferedVisitDays { get; set; }
        public string TimesPurchasedLastWeekFromOthers { get; set; }
        public string RegularSuppliers { get; set; }
        public string WantsExclusiveProgasDealer { get; set; }
        public string WantsAuthorizedProgasReseller { get; set; }
        public string ContactNumber { get; set; }
        public string SmsNumber { get; set; }
        public string AddedBy { get; set; }
        public string AddedAt { get; set; }
        public string First { get; set; }
        public string Second { get; set; }
        public string Third { get; set; }
        public string VOC { get; set; }
    }
    public class CallCenterAnySuppierModel
    {
        public int Id { get; set; }
        public int CallCenterId { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public string AddedAt { get; set; }
        public string AddedBy { get; set; }
        public string Category { get; set; }
    }
    public class CallCenterWeeklyPurchaseModel
    {
        public int Id { get; set; }
        public int CallCenterId { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public string AddedAt { get; set; }
        public string AddedBy { get; set; }
        public string Category { get; set; }
    }
    public class Outlets
    {
        public int outlet_id { get; set; }
        public int route_id { get; set; }
        public string outletname { get; set; }
        public string latitude { get; set; }
        public string Route { get; set; }
        public string longtitude { get; set; }
        public string contact_person { get; set; }
        public string contact_number { get; set; }
        public string street { get; set; }
        public string area { get; set; }
        public string date_added { get; set; }
        public int added_by_user_id { get; set; }
        public string outlet_class { get; set; }
        public string ReturnsCat { get; set; }
        public string sap_customer_id { get; set; }
        public string photo_id { get; set; }
        public int CustomerCategory { get; set; }
    }
    public class SearchResultModel
    {
        public string SerialNumber { get; set; }
        public string TagQRCode { get; set; }
        public string TagVisible { get; set; }
        public string ProductionOrder { get; set; }
        public string value { get; set; }
        public string label { get; set; }
        public string Id { get; set; }
    }
}