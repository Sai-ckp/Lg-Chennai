namespace ERP_API.Data
{
    public class Bom
    {
        
        public int? Id {  get; set; }

        public int? ItemId { get; set; }

        public  double? RMUtilityqty { get; set; }

        public Item Item { get; set; }


    }
}
