using Dsw2025Tpi.Domain.Enums;

namespace Dsw2025Tpi.Domain.Entities;

public sealed class Payment : EntityBase
{
    public Guid OrderId { get; private set; }
    public string MercadoPagoId { get; private set; }
    public string PreferenceId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod? Method { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? PayerEmail { get; private set; }
    public string? StatusDetail { get; private set; }

    public Order? Order { get; private set; }

    private Payment() { }

    private Payment(
        Guid orderId,
        string preferenceId,
        decimal amount,
        string? externalReference)
    {
        OrderId = orderId;
        PreferenceId = preferenceId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        ExternalReference = externalReference;
        MercadoPagoId = string.Empty;
    }

    public static Payment Create(
        Guid orderId,
        string preferenceId,
        decimal amount,
        string? externalReference = null)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty", nameof(orderId));

        if (string.IsNullOrWhiteSpace(preferenceId))
            throw new ArgumentException("PreferenceId cannot be empty", nameof(preferenceId));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        return new Payment(orderId, preferenceId, amount, externalReference);
    }

    public void UpdateStatus(
        string mercadoPagoId,
        PaymentStatus status,
        PaymentMethod? method = null,
        string? payerEmail = null,
        string? statusDetail = null)
    {
        MercadoPagoId = mercadoPagoId;
        Status = status;
        Method = method;
        PayerEmail = payerEmail;
        StatusDetail = statusDetail;

        if (status == PaymentStatus.Approved || status == PaymentStatus.Rejected)
        {
            ProcessedAt = DateTime.UtcNow;
        }
    }

    public void MarkAsApproved(
        string mercadoPagoId,
        PaymentMethod method,
        string? payerEmail = null)
    {
        UpdateStatus(mercadoPagoId, PaymentStatus.Approved, method, payerEmail);
    }

    public void MarkAsRejected(
        string mercadoPagoId,
        string? statusDetail = null)
    {
        UpdateStatus(mercadoPagoId, PaymentStatus.Rejected, null, null, statusDetail);
    }

    public void MarkAsPending(string mercadoPagoId)
    {
        UpdateStatus(mercadoPagoId, PaymentStatus.Pending);
    }

    public bool IsApproved => Status == PaymentStatus.Approved;
    public bool IsPending => Status == PaymentStatus.Pending;
    public bool IsRejected => Status == PaymentStatus.Rejected;
}
