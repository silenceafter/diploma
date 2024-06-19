namespace app.server.Controllers.Response
{
    public class ReceivingDiscountsResponse
    {
        public int Id { get; set; }

        public string PartnerName { get; set; }

        public string DiscountTerms { get; set; }

        public DateTime DiscountDateStart { get; set; }

        public DateTime DiscountDateEnd { get; set; }

        public int DiscountBonuses { get; set; }

        public DateTime Date { get; set; }
    }
}