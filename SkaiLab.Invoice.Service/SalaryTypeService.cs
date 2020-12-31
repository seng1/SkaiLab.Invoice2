using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class SalaryTypeService:Service,ISalaryTypeService
    {
        public SalaryTypeService(IDataContext context) : base(context)
        {

        }

        public List<SalaryType> GetSalaryTypes(string organisationId)
        {
            using var context = Context();
            var khmer = IsKhmer;
            var organisation = context.Organisation.FirstOrDefault(u => u.Id == organisationId);
            if (!organisation.DeclareTax)
            {
                return context.SalaryType.Where(u=>u.Id==(int)SalaryTypeEnum.Net).Select(u => new SalaryType
                {
                    Id = u.Id,
                    Name =IsKhmer?u.NameKh: u.Name
                }).ToList();
            }
            var salaryTypes = context.SalaryType.Select(u => new SalaryType
            {
                Id=u.Id,
                Name = IsKhmer ? u.NameKh : u.Name
            }).ToList();
            return salaryTypes;
        }
    }
}
