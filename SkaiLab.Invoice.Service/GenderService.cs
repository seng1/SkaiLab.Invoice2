using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class GenderService:Service,IGenderService
    {
        public GenderService(IDataContext context) : base(context)
        {

        }

        public List<Gender> GetGenders()
        {
            var khmer = IsKhmer;
            using var context = Context();
            return context.Gender.Select(u => new Gender
            {
                Id=u.Id,
                Name=IsKhmer?u.NameKh: u.Name
            }).ToList();
        }
    }
}
