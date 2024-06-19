using app.server.Controllers.Requests;
using System.ComponentModel.DataAnnotations;

namespace app.Server.Controllers.Requests
{
    public class AcceptanceRequest
    {
        public string Email { get; set; }
        public int PointId { get; set; }
        public List<WasteItem>? WasteItems { get; set; }        
    }
}
