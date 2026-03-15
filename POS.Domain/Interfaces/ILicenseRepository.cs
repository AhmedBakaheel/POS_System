using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Interfaces
{
    public interface ILicenseRepository : IGenericRepository<Entities.SystemLicense>
    {
        Task<Entities.SystemLicense> GetByMachineCodeAsync(string machineCode);
    }
}
