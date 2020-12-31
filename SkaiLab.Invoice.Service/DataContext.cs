using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SkaiLab.Invoice.Dal.Models;
using SkaiLab.Invoice.Models;

namespace SkaiLab.Invoice.Service
{
    public class DataContext : IDataContext
    {
        private readonly IHttpContextAccessor contextAccessor;
        private string _organisationId;
        public DataContext(IOptions<Option> option, IHttpContextAccessor httpContext, IAppResource appResource)
        {
            this.Option = option.Value;
            this.contextAccessor = httpContext;
            this.AppResource = appResource;
        }
        public Option Option { get; }

        public string UserId
        {
            get
            {
                if (contextAccessor.HttpContext.User != null && contextAccessor.HttpContext.User.Identity != null && contextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    return contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                return "";
            }
        }

        public string OrganisationId  {
            get
            {
                if (contextAccessor.HttpContext.User != null && contextAccessor.HttpContext.User.Identity != null && contextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    var organisationId = contextAccessor.HttpContext.Request.Headers["organisationid"].ToString();
                    if (organisationId == _organisationId)
                    {
                        _organisationId = organisationId;
                        return _organisationId;
                    }
                    if (!IsUserHasPermissionToWorkingWithOrganisation(organisationId))
                    {
                        throw new Exception("Access deny");
                    }
                    _organisationId = organisationId;
                    return _organisationId;
                }
                return "";
            }
        }
        public InvoiceContext Context()
        {
            var optionsBuilder = new DbContextOptionsBuilder<InvoiceContext>();
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseSqlServer(Option.ConnectionString);
            return new InvoiceContext(optionsBuilder.Options);
        }
        public BlobContainerClient BlobContainerClient()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(Option.BlobStorage.AccountConnection);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(Option.BlobStorage.ContainerName);
            return containerClient;
        }
        private bool IsUserHasPermissionToWorkingWithOrganisation(string organisationId)
        {
            using var context = Context();
            return context.OrganisationUser.Any(t => t.OrganisationId == organisationId && t.UserId == UserId);
        }
        public string Language
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture.Name;
            }
        }

        public IAppResource AppResource { get; }
    }
}
