namespace ERP_API.Data
{
    public class FgLabelGeneration
    {
        public int FgLabelId { get; set; } // Primary Key (cannot be null)
        public int? ItemId { get; set; } // Nullable integer
        public int ? VendorId { get; set; }
        public string? CompanyName { get; set; }

        public double? PackQty { get; set; } // Nullable decimal
        public int? NextBagNo { get; set; } // Nullable string
        public DateTime? DateOfMfg { get; set; } // Nullable DateTime
        public string? BatchNo { get; set; } // Nullable string
        public int? NoOfBags { get; set; }

        public int?  SerialNumber { get;  set; }

    }
}
