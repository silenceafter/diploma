using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace app.Server.Models;

public partial class Point
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Acceptance> Acceptances { get; set; } = new List<Acceptance>();
}
