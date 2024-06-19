using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace app.Server.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TypeId { get; set; }

    public DateTime Date { get; set; }

    public int BonusesStart { get; set; }

    public int BonusesEnd { get; set; }

    [JsonIgnore]
    public virtual ICollection<Acceptance> Acceptances { get; set; } = new List<Acceptance>();

    [JsonIgnore]
    public virtual ICollection<ReceivingDiscount> ReceivingDiscounts { get; set; } = new List<ReceivingDiscount>();

    [JsonIgnore]
    public virtual ICollection<ReceivingProduct> ReceivingProducts { get; set; } = new List<ReceivingProduct>();

    [JsonIgnore]
    public virtual TransactionType Type { get; set; } = null!;

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
