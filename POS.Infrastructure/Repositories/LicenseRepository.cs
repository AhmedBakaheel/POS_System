using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Interfaces;
using POS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Infrastructure.Repositories
{
    public class LicenseRepository : GenericRepository<SystemLicense>, ILicenseRepository
    {
        public LicenseRepository(ApplicationDbContext context) : base(context) { }

        public async Task<SystemLicense> GetByMachineCodeAsync(string machineCode)
        {
            return await _context.Licenses.FirstOrDefaultAsync(l => l.MachineCode == machineCode);
        }
    }
}
