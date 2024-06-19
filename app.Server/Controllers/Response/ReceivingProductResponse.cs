namespace app.server.Controllers.Response
{
    public class ReceivingProductResponse
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public int ProductQuantity { get; set; }

        public int ProductBonus { get; set; }

        public int Cost { get; set; }

        public DateTime Date { get; set; }
    }
}
