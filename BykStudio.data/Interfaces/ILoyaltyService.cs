namespace BykStudio.data.Interfaces
{
    public interface ILoyaltyService
    {
        Task EarnPointsAsync(string userId, decimal paymentAmount, Guid paymentId, CancellationToken cancellationToken = default);
        Task RedeemPointsAsync(string userId, int pointsToRedeem, Guid bookingId, CancellationToken cancellationToken = default);
        Task RefundPointsAsync(Guid paymentId, decimal refundAmount, CancellationToken cancellationToken = default);
        Task<int> GetBalanceAsync(string userId, CancellationToken cancellationToken = default);
    }
}
