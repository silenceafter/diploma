using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace app.Server.Models;

public partial class HazardousWaste
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int HazardClassId { get; set; }

    public int Bonuses { get; set; }

    [JsonIgnore]
    public virtual ICollection<Acceptance> Acceptances { get; set; } = new List<Acceptance>();

    [JsonIgnore]
    public virtual HazardClass HazardClass { get; set; } = null!;
}
