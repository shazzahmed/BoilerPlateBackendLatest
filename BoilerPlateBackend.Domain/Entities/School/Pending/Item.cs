public class Item
{
    public int Id { get; set; }
    public int? ItemCategoryId { get; set; }
    public string Name { get; set; }
    public string Unit { get; set; }
    public string ItemPhoto { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? ItemStoreId { get; set; }
    public int? ItemSupplierId { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
}
