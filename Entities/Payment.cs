using Entities;
using System;
using System.Collections.Generic;
using System.Text;

public class Payment
{
    public int PaymentId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public decimal Amount { get; set; }
    public List<PaymentItem> Items { get; set; } = new();
}

