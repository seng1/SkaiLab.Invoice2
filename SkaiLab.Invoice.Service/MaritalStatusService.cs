using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class MaritalStatusService:Service,IMaritalStatusService
    {
        public MaritalStatusService(IDataContext context) : base(context)
        {

        }

        public List<MaritalStatus> GetMaritalStatuses()
        {
            using var context = Context();
            var khmer = IsKhmer;
            var maritalStatuses = context.MaritalStatus.Select(u => new MaritalStatus
            {
                Id = u.Id,
                Name = IsKhmer ? u.NameKh : u.Name
            }).ToList();
            return maritalStatuses;
        }
    }
}
