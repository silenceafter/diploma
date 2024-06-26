﻿using System;
using System.Collections.Generic;

namespace app.Server.Models;

public partial class Discount
{
    public int Id { get; set; }

    public int PartnerId { get; set; }

    public string Terms { get; set; } = null!;

    public DateTime DateStart { get; set; }

    public DateTime DateEnd { get; set; }

    public int Bonuses { get; set; }

    public virtual Partner Partner { get; set; } = null!;

    public virtual ICollection<ReceivingDiscount> ReceivingDiscounts { get; set; } = new List<ReceivingDiscount>();
}
