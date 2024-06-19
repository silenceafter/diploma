using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace app.Server.Models;

public partial class Acceptance
{
    public int Id { get; set; }

    public int TransactionId { get; set; }

    public int HazardousWasteId { get; set; }

    public int Quantity { get; set; }

    public DateTime Date { get; set; }

    public int PointId { get; set; }

    [JsonIgnore]
    public virtual HazardousWaste HazardousWaste { get; set; } = null!;

    [JsonIgnore]
    public virtual Point Point { get; set; } = null!;

    [JsonIgnore]
    public virtual Transaction Transaction { get; set; } = null!;
}
