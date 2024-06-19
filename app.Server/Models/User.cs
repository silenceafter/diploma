using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace app.Server.Models;

public partial class User
{
    public int Id { get; set; }

    public string Encrypt { get; set; } = null!;

    public string EmailHash { get; set; } = null!;

    public int Bonuses { get; set; }

    public int RoleId { get; set; }

    [JsonIgnore]
    public virtual Role Role { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
