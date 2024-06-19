using System;
using System.Collections.Generic;

namespace app.Server.TempModels;

public partial class ReceivingProduct
{
    public int Id { get; set; }

    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    public DateTime Date { get; set; }
}
