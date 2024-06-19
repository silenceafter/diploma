namespace app.server.Controllers.Response
{
    public class TransactionResponse
    {
        public int Id { get; set; }
        public string TransactionName { get; set; } = null!;

        public DateTime Date { get; set; }

        public int BonusesStart { get; set; }

        public int BonusesEnd { get; set; }
    }
}
