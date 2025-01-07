namespace ERP_API.Moduls
{
    public class PmLabelGenerationReadOnlyDto
    {
        public int PmLabelId { get; set; } // Primary key, typically non-nullable
        public int? ItemId { get; set; } // Nullable integer
        public int? NoOfBags { get; set; } // Nullable integer
        public string? BatchNo { get; set; } // Nullable string
        public double? PackingQty { get; set; } // Nullable double
        public int? NextBagNo { get; set; } // Nullable integer
        public string? CompanyName { get; set; }
        public DateTime? DateOfMfg { get; set; } // Nullable DateTime


    }
}
