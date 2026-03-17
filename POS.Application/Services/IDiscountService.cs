using POS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.Services
{
    public interface IDiscountService
    {
        Task<Promotion> GetActivePromotionForProductAsync(int productId);
        decimal CalculateDiscountedPrice(Promotion promotion, decimal originalPrice);
    }
}
