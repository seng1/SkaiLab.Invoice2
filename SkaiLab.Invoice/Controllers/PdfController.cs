using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Service;
using System.IO;

namespace SkaiLab.Invoice.Controllers
{
    public class PdfController : Controller
    {
        public IWebHostEnvironment Environment { get; }
        private readonly IHtmlToPdfConverterService htmlToPdfConverter;
        private readonly IPrintService printService;
        public PdfController(IHtmlToPdfConverterService htmlToPdfConverter, IPrintService printService, IWebHostEnvironment environment)
        {
            this.htmlToPdfConverter = htmlToPdfConverter;
            this.printService = printService;
            this.Environment = environment;
        }
        public IActionResult Index(long id)
        {
            var pdfContentType = "application/pdf";
            var url = Request.Scheme + "://" + Request.Host + "/pdf/Payslip?id=" + id;
            
            return File(htmlToPdfConverter.htmlToPdfConverter.GeneratePdfFromFile(url, null), pdfContentType);
        }
        public IActionResult Quote(long id)
        {
            var quote = printService.GetQuote(id);
            ViewBag.path = Path.Combine(Environment.WebRootPath, "invoice.pfx");
            return View(quote);
        }
        public IActionResult PurchaseOrder(long id)
        {
            var quote = printService.GetPurchase(id);
            return View(quote);
        }
        public IActionResult Bill(long id)
        {
            var quote = printService.GetBill(id);
            return View(quote);
        }
        public IActionResult Expense(long id)
        {
            var quote = printService.GetExpense(id);
            return View(quote);
        }
        public IActionResult Invoice(long id)
        {
            var invoice = printService.GetInvoice(id);
            return View(invoice);
        }
        public IActionResult Receipt(long id,string purpose)
        {
            var receipt = printService.GetReceipt(id, purpose);
            return View(receipt);
        }
        public IActionResult Payslip(long id)
        {
            if (printService.IsTaxPayslip(id))
            {
                return Redirect("/pdf/PayslipTax?id=" + id);
            }
            return Redirect("/pdf/PayslipNoneTax?id=" + id);
        }
        public IActionResult PayslipTax(long id)
        {
            return View(printService.GetPayrollTax(id));
        }
        public IActionResult PayslipNoneTax(long id)
        {
            return View(printService.GetPayrollNoneTax(id));
        }
    }
}
