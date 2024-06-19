using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace app.Server.Models;

public partial class ReceivingProduct
{
    public int Id { get; set; }

    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    public DateTime Date { get; set; }

    public int Quantity { get; set; }

    [JsonIgnore]
    public virtual Product Product { get; set; } = null!;

    [JsonIgnore]
    public virtual Transaction Transaction { get; set; } = null!;
}
