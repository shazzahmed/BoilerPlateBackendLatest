public class ItemStock
{
    public int Id { get; set; }
    public int? ItemId { get; set; }
    public int? SupplierId { get; set; }
    public string Symbol { get; set; }
    public int? StoreId { get; set; }
    public int? Quantity { get; set; }
    public string PurchasePrice { get; set; }
    public DateTime Date { get; set; }
    public string Attachment { get; set; }
    public string Description { get; set; }
    public string IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
