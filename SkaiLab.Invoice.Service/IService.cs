using System;
using SkaiLab.Invoice.Models;

namespace SkaiLab.Invoice.Service
{
    public interface IService
    {
        Option Option { get; }
        string UserId { get; }
        IAppResource AppResource { get; }
        string OrganisationId { get; }
        string UploadFile(string baseString);
        string UploadFile(string baseString,string fileName);
        string FormatCurrency(decimal price);
        int GetOrganisationBaseCurrencyId(string organisationId);
        Currency GetTaxCurrency(string organisationId);
        bool CheckPermission(string userId, string organisationId, int menuFeatureId);
        bool CheckPermission(string userId, string organisationId, int[] menuFeatureIds);
        string Language { get; }
        bool IsKhmer { get; }
        bool IsValidLicense(string organisationId);
    }
}
