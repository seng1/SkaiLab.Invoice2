using System;
using Azure.Storage.Blobs;
using SkaiLab.Invoice.Dal.Models;
using SkaiLab.Invoice.Models;

namespace SkaiLab.Invoice.Service
{
    public interface IDataContext
    {
        InvoiceContext Context();
        Models.Option Option { get; }
        string UserId { get; }
        string OrganisationId { get; }
        string Language { get; }
        BlobContainerClient BlobContainerClient();
        IAppResource AppResource { get; }
    }
}
