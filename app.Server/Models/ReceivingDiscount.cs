using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace app.Server.Models;

public partial class ReceivingDiscount
{
    public int Id { get; set; }

    public int TransactionId { get; set; }

    public int DiscountId { get; set; }

    public DateTime Date { get; set; }

    [JsonIgnore]
    public virtual Discount Discount { get; set; } = null!;

    [JsonIgnore]
    public virtual Transaction Transaction { get; set; } = null!;
}
