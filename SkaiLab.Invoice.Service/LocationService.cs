using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class LocationService:Service,ILocationService
    {
        public LocationService(IDataContext context) : base(context)
        {

        }

        public void Create(string organisationId, Location location)
        {
            using var context = Context();
            if(context.Location.Any(u=>u.Name==location.Name&& u.OrganisationId == organisationId))
            {
                throw new Exception("Localtion name already exist");
            }
            context.Location.Add(new Dal.Models.Location
            {
                OrganisationId = organisationId,
                Name = location.Name
            });
            context.SaveChanges();
        }

        public Location GetLocation(string organisationId, long id)
        {
            using var context = Context();
            var location = context.Location.FirstOrDefault(u => u.Id == id && u.OrganisationId == organisationId);
            if (location == null)
            {
                throw new Exception("Location not found");
            }
            return new Location
            {
                Id = location.Id,
                Name = location.Name
            };
        }

        public List<Location> GetLocations(string organisationId)
        {
            using var context = Context();
            var locations = context.Location.Where(u => u.OrganisationId == organisationId)
                .OrderBy(u => u.Name)
                .Select(u => new Location
                {
                    Name=u.Name,
                    Id=u.Id
                }).ToList();
            return locations;
        }

        public void Update(string organisationId, Location location)
        {
            using var context = Context();
            var updateLocation = context.Location.FirstOrDefault(u => u.Id == location.Id && u.OrganisationId == organisationId);
            if (updateLocation == null)
            {
                throw new Exception("Location not found");
            }
            updateLocation.Name = location.Name;
            context.SaveChanges();


        }
    }
}
