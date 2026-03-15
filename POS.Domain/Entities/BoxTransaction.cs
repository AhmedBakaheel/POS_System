using POS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class BoxTransaction
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int? ShiftId { get; set; }
        public Shift Shift { get; set; }

        public string? UserId { get; set; }
    }
}
