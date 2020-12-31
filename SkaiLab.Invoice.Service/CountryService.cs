using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class CountryService : Service, ICountryService
    {
        public CountryService(IDataContext context) : base(context)
        {

        }

        public List<Country> GetCountries()
        {
            using var context = Context();
            var khmer = IsKhmer;
            var countries = context.Country.Select(u => new Country
            {
                Id=u.Id,
                Alpha2Code=u.Alpha2Code,
                Alpha3Code=u.Alpha3Code,
                Name=IsKhmer?u.NameKh: u.Name,
                Nationality=khmer?u.NationalityKh: u.Nationality
            }).ToList();
            return countries;
        }
    }
    
}
