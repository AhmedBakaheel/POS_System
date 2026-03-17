using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DiscountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Promotion> GetActivePromotionForProductAsync(int productId)
        {
            var now = DateTime.Now;
            return (await _unitOfWork.Promotions.FindAsync(p =>
                p.IsActive &&
                p.StartDate <= now &&
                p.EndDate >= now))
                .FirstOrDefault();
        }

        public decimal CalculateDiscountedPrice(Promotion promotion, decimal originalPrice)
        {
            if (promotion == null) return originalPrice;

            return promotion.Type switch
            {
                PromotionType.Percentage => originalPrice - (originalPrice * (promotion.Value / 100)),
                PromotionType.FixedAmount => originalPrice - promotion.Value,
                _ => originalPrice
            };
        }
    }
}
