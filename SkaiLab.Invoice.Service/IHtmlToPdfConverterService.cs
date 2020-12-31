using NReco.PdfGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IHtmlToPdfConverterService
    {
        HtmlToPdfConverter htmlToPdfConverter { get; }
    }
}
