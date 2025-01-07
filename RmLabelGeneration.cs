namespace ERP_API.Data
{
    public class RmLabelGeneration
    {
        public int LabelId { get; set; }
        public string? ItemName { get; set; }

        public int ? ItemId { get; set; }
        public int ? VendorId { get; set; }
        public int NoOfBags { get; set; }
        public int NextBagNo { get; set; }
        public string? BatchNo { get; set; }
        public string? InvNo { get; set; }
        public DateTime? InvDate { get; set; }
        public string? CompanyName { get; set; }




    }
}
