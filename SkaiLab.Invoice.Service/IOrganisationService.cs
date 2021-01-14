using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IOrganisationService:IService
    {
       void Create(string userId, string companyName);
        Organsation Get(string id);
        string GetOrganisationIdByUserId(string userId);
        void Update(Organsation org);
        Currency GetBaseCurrency(string organisationId);
        List<Organsation> GetOrganisations(string userId);
        Organsation GetWorkingOrganisation(string userId);
        int Create(Organsation organsation, string userId);
        void ChangeWorkingOrganisation(string organisationId, string userId);
        List<Organsation> GetOrganisationsWithSameBaseCurrency(string organisationId,string userId);
        int GetMaximumCreateOrganisationByUser(string userId);
    }
}
