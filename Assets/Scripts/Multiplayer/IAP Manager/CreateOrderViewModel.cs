using System;

[System.Serializable]
public class CreateOrderViewModel
{
    public Guid PlayerId { get; set; }
    public ItemForPurchaseNumber PurchaseType { get; set; }
    public string Currency { get; set; }
    public decimal Price { get; set; }
}
