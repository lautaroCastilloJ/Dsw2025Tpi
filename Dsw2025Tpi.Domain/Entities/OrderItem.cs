using Dsw2025Tpi.Domain.Exceptions.OrderExceptions;


namespace Dsw2025Tpi.Domain.Entities;

public sealed class OrderItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    // Derivado: el backend calcula el Subtotal en base a Quantity y UnitPrice
    public decimal Subtotal => Quantity * UnitPrice;

    private OrderItem() { } // for EF

    private OrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        SetProduct(productId);
        SetQuantity(quantity);
        SetUnitPrice(unitPrice);
    }

    public static OrderItem Create(Guid productId, int quantity, decimal unitPrice)
        => new(productId, quantity, unitPrice);

    public void ChangeQuantity(int newQuantity)
    {
        SetQuantity(newQuantity);
    }

    private void SetProduct(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new InvalidOrderItemProduct();
        ProductId = productId;
    }

    private void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidOrderItemQuantityException();

        Quantity = quantity;
    }

    private void SetUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new InvalidOrderItemPriceException();

        UnitPrice = unitPrice;
    }
}
