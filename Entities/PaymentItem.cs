namespace Entities;

public class PaymentItem
{
    public int PaymentItemId { get; set; }
    public int PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public decimal Price { get; set; }
}