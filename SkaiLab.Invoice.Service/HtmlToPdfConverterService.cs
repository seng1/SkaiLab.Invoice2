using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using NReco.PdfGenerator;
using SkaiLab.Invoice.Models;
using System.IO;
using System.Runtime.InteropServices;

namespace SkaiLab.Invoice.Service
{
    public class HtmlToPdfConverterService : IHtmlToPdfConverterService
    {
        private readonly HtmlToPdfConverter _htmlToPdfConverter;
        public HtmlToPdfConverterService(IOptions<Option> option, IWebHostEnvironment webHostEnvironment)
        {
            _htmlToPdfConverter = new HtmlToPdfConverter();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _htmlToPdfConverter.WkHtmlToPdfExeName = "wkhtmltopdf";
                _htmlToPdfConverter.PdfToolPath = Path.Combine(webHostEnvironment.WebRootPath, "wkhtmltox", "linux");
            }
            else
            {
                _htmlToPdfConverter.WkHtmlToPdfExeName = "wkhtmltopdf.exe";
                _htmlToPdfConverter.PdfToolPath = Path.Combine(webHostEnvironment.WebRootPath, "wkhtmltox", "window");
            }
            _htmlToPdfConverter.Size = PageSize.A4;
            _htmlToPdfConverter.PageHeaderHtml = "<div style='height:10px'></div>";
            _htmlToPdfConverter.PageFooterHtml = "<div style='text-align:center'>page <span class='page'></span> of &nbsp;<span class='topage'></span></div>";
            _htmlToPdfConverter.License.SetLicenseKey(
                    option.Value.NReco.Owner,
                    option.Value.NReco.Key
                );
        }

        HtmlToPdfConverter IHtmlToPdfConverterService.htmlToPdfConverter => _htmlToPdfConverter;
    }
}
