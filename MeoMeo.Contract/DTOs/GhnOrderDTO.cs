using System.Text.Json.Serialization;

namespace MeoMeo.Contract.DTOs
{
    public class GhnCreateOrderRequestDTO
    {
        [JsonPropertyName("payment_type_id")]
        public int PaymentTypeId { get; set; } = 2; // 1: Shop/Seller, 2: Buyer/Consignee

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonPropertyName("required_note")]
        public string RequiredNote { get; set; } = "KHONGCHOXEMHANG"; // CHOTHUHANG, CHOXEMHANGKHONGTHU, KHONGCHOXEMHANG

        [JsonPropertyName("from_name")]
        public string FromName { get; set; } = "MeoMeo Shop";

        [JsonPropertyName("from_phone")]
        public string FromPhone { get; set; } = "0987654321";

        [JsonPropertyName("from_address")]
        public string FromAddress { get; set; } = "72 Thành Thái, Phường 14, Quận 10, Hồ Chí Minh, Vietnam";

        [JsonPropertyName("from_ward_name")]
        public string FromWardName { get; set; } = "Phường 14";

        [JsonPropertyName("from_district_name")]
        public string FromDistrictName { get; set; } = "Quận 10";

        [JsonPropertyName("from_province_name")]
        public string FromProvinceName { get; set; } = "HCM";

        [JsonPropertyName("return_phone")]
        public string? ReturnPhone { get; set; }

        [JsonPropertyName("return_address")]
        public string? ReturnAddress { get; set; }

        [JsonPropertyName("return_district_id")]
        public int? ReturnDistrictId { get; set; }

        [JsonPropertyName("return_ward_code")]
        public string? ReturnWardCode { get; set; }

        [JsonPropertyName("client_order_code")]
        public string? ClientOrderCode { get; set; }

        [JsonPropertyName("to_name")]
        public string ToName { get; set; } = "";

        [JsonPropertyName("to_phone")]
        public string ToPhone { get; set; } = "";

        [JsonPropertyName("to_address")]
        public string ToAddress { get; set; } = "";

        [JsonPropertyName("to_ward_code")]
        public string ToWardCode { get; set; } = "";

        [JsonPropertyName("to_district_id")]
        public int ToDistrictId { get; set; }

        [JsonPropertyName("cod_amount")]
        public int CodAmount { get; set; } = 0;

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("weight")]
        public int Weight { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("pick_station_id")]
        public int? PickStationId { get; set; }

        [JsonPropertyName("deliver_station_id")]
        public int? DeliverStationId { get; set; }

        [JsonPropertyName("insurance_value")]
        public int InsuranceValue { get; set; } = 0;

        [JsonPropertyName("service_id")]
        public int ServiceId { get; set; } = 0;

        [JsonPropertyName("service_type_id")]
        public int ServiceTypeId { get; set; } = 2; // 2: E-commerce Delivery, 5: Traditional Delivery

        [JsonPropertyName("coupon")]
        public string? Coupon { get; set; }

        [JsonPropertyName("pick_shift")]
        public int[]? PickShift { get; set; }

        [JsonPropertyName("items")]
        public List<GhnOrderItemDTO> Items { get; set; } = new();
    }

    public class GhnOrderItemDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("weight")]
        public int Weight { get; set; }

        [JsonPropertyName("category")]
        public GhnItemCategoryDTO? Category { get; set; }
    }

    public class GhnItemCategoryDTO
    {
        [JsonPropertyName("level1")]
        public string? Level1 { get; set; }

        [JsonPropertyName("level2")]
        public string? Level2 { get; set; }

        [JsonPropertyName("level3")]
        public string? Level3 { get; set; }
    }

    public class GhnCreateOrderResponseDTO
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = "";

        [JsonPropertyName("data")]
        public GhnOrderDataDTO? Data { get; set; }

        [JsonPropertyName("code_message")]
        public string? CodeMessage { get; set; }
    }

    public class GhnOrderDataDTO
    {
        [JsonPropertyName("district_encode")]
        public string? DistrictEncode { get; set; }

        [JsonPropertyName("expected_delivery_time")]
        public string? ExpectedDeliveryTime { get; set; }

        [JsonPropertyName("fee")]
        public GhnOrderFeeDTO? Fee { get; set; }

        [JsonPropertyName("order_code")]
        public string? OrderCode { get; set; }

        [JsonPropertyName("sort_code")]
        public string? SortCode { get; set; }

        [JsonPropertyName("total_fee")]
        public string? TotalFee { get; set; }

        [JsonPropertyName("trans_type")]
        public string? TransType { get; set; }

        [JsonPropertyName("ward_encode")]
        public string? WardEncode { get; set; }
    }

    public class GhnOrderFeeDTO
    {
        [JsonPropertyName("coupon")]
        public int Coupon { get; set; }

        [JsonPropertyName("insurance")]
        public int Insurance { get; set; }

        [JsonPropertyName("main_service")]
        public int MainService { get; set; }

        [JsonPropertyName("r2s")]
        public int R2s { get; set; }

        [JsonPropertyName("return")]
        public int Return { get; set; }

        [JsonPropertyName("station_do")]
        public int StationDo { get; set; }

        [JsonPropertyName("station_pu")]
        public int StationPu { get; set; }
    }
}
