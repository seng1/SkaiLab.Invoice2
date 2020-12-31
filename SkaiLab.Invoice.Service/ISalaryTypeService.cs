using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface ISalaryTypeService:IService
    {
        List<SalaryType> GetSalaryTypes(string organisationId);
    }
}
