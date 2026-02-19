using System;
using System.Collections.Generic;
using System.Text;
using BykStudio.data.Interfaces;
using BykStudio.data.Models;
using Microsoft.EntityFrameworkCore;

namespace BykStudio.data.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly ApplicationDbContext _context;
        private const int PointsPerRub = 1; // 1 point per 100 rubles => adjust logic accordingly
        private const int RubPerHundred = 100;

        public LoyaltyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task EarnPointsAsync(string userId, decimal paymentAmount, Guid paymentId, CancellationToken cancellationToken = default)
        {
            // Calculate points: 1 point per 100 rubles (integer division)
            int pointsEarned = (int)(paymentAmount / 100);

            if (pointsEarned <= 0) return;

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Create transaction record
                var trans = new LoyaltyTransaction
                {
                    UserId = userId,
                    Points = pointsEarned,
                    Type = "Earn",
                    PaymentId = paymentId,
                    Description = $"Earned {pointsEarned} points from payment {paymentAmount:C}"
                };
                _context.LoyaltyTransactions.Add(trans);

                // Update user's balance
                var loyaltyPoints = await _context.LoyaltyPoints
                    .FirstOrDefaultAsync(lp => lp.UserId == userId, cancellationToken);
                if (loyaltyPoints == null)
                {
                    loyaltyPoints = new LoyaltyPoints { UserId = userId, Balance = pointsEarned };
                    _context.LoyaltyPoints.Add(loyaltyPoints);
                }
                else
                {
                    loyaltyPoints.Balance += pointsEarned;
                    loyaltyPoints.LastUpdated = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task RedeemPointsAsync(string userId, int pointsToRedeem, Guid bookingId, CancellationToken cancellationToken = default)
        {
            if (pointsToRedeem <= 0)
                throw new ArgumentException("Points to redeem must be positive");

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var loyaltyPoints = await _context.LoyaltyPoints
                .FirstOrDefaultAsync(lp => lp.UserId == userId, cancellationToken);
            if (loyaltyPoints == null || loyaltyPoints.Balance < pointsToRedeem)
                throw new InvalidOperationException("Insufficient loyalty points");

            // Create transaction record
            var trans = new LoyaltyTransaction
            {
                UserId = userId,
                Points = -pointsToRedeem, // negative for redemption
                Type = "Redeem",
                BookingId = bookingId,
                Description = $"Redeemed {pointsToRedeem} points for booking discount"
            };
            _context.LoyaltyTransactions.Add(trans);

            loyaltyPoints.Balance -= pointsToRedeem;
            loyaltyPoints.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        public async Task RefundPointsAsync(Guid paymentId, decimal refundAmount, CancellationToken cancellationToken = default)
        {
            // Find the original earning transaction(s) for this payment
            var earnTransactions = await _context.LoyaltyTransactions
                .Where(t => t.PaymentId == paymentId && t.Type == "Earn")
                .ToListAsync(cancellationToken);

            if (!earnTransactions.Any())
                return; // No points earned for this payment

            int totalEarned = earnTransactions.Sum(t => t.Points);
            int pointsToRefund = (int)(refundAmount / 100); // Recalculate based on refund amount

            // If refund is partial, deduct proportional points (or full? We'll deduct proportional)
            pointsToRefund = Math.Min(pointsToRefund, totalEarned);

            if (pointsToRefund <= 0) return;

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            // Create refund transaction
            var refundTrans = new LoyaltyTransaction
            {
                UserId = earnTransactions.First().UserId,
                Points = -pointsToRefund,
                Type = "Refund",
                PaymentId = paymentId,
                Description = $"Refunded {pointsToRefund} points due to refund of {refundAmount:C}"
            };
            _context.LoyaltyTransactions.Add(refundTrans);

            // Update user's balance
            var loyaltyPoints = await _context.LoyaltyPoints
                .FirstOrDefaultAsync(lp => lp.UserId == refundTrans.UserId, cancellationToken);
            if (loyaltyPoints != null)
            {
                loyaltyPoints.Balance -= pointsToRefund;
                loyaltyPoints.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        public async Task<int> GetBalanceAsync(string userId, CancellationToken cancellationToken = default)
        {
            var lp = await _context.LoyaltyPoints
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
            return lp?.Balance ?? 0;
        }
    }
}
