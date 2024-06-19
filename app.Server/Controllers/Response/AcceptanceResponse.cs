using app.Server.Models;

namespace app.server.Controllers.Response
{
    public class AcceptanceResponse
    {
        public int Id { get; set; }

        public string HazardousWasteName { get; set; }

        public int Quantity { get; set; }
        
        public string PointName { get; set; }

        public string PointAddress { get; set; }

        public DateTime Date { get; set; }
    }
}
