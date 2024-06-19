using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace app.Server.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Bonus { get; set; }

    [JsonIgnore]
    public virtual ICollection<ReceivingProduct> ReceivingProducts { get; set; } = new List<ReceivingProduct>();
}
