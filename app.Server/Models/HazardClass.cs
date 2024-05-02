﻿using System;
using System.Collections.Generic;

namespace app.Server.Models;

public partial class HazardClass
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<HazardousWaste> HazardousWastes { get; set; } = new List<HazardousWaste>();
}
