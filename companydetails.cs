using System;
using System.Collections.Generic;

namespace ERP_API.Data;

public partial class companydetails
{
    public int CompanyId { get; set; }

    public string? PrintHead { get; set; }

    public string? PrintShortAddr { get; set; }

    public bool? RoffPurch { get; set; }

    public bool? RoffSales { get; set; }

    public bool? RoffLabour { get; set; }

    public bool? RoffSubCon { get; set; }

    public bool? ReTrkRejPurch { get; set; }

    public bool? ReTrkRejMfgSale { get; set; }

    public bool? ReTrkRejLabourSale { get; set; }

    public bool? ReTrkRejSubCon { get; set; }

    public int? BookTypeDc { get; set; }

    public int? BookTypeInv { get; set; }

    public bool? RedStkM { get; set; }

    public string? MyLstno { get; set; }

    public string? MyCstno { get; set; }

    public string? MyVatregNo { get; set; }

    public string? MyServRegNo { get; set; }

    public string? MyPanno { get; set; }

    public string? Rng { get; set; }

    public string? Divi { get; set; }

    public string? Commission { get; set; }

    public string? AcctCircle { get; set; }

    public string? PrefixQuote { get; set; }

    public string? PrefixPo { get; set; }

    public string? PrefixWo { get; set; }

    public string? PrefixMfgInvoice { get; set; }
}
