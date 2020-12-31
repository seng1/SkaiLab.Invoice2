using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PrintController : ControllerBase
    {
        private readonly IHtmlToPdfConverterService htmlToPdfConverter;
        private readonly IWebHostEnvironment webHostEnvironment;
        public PrintController(IHtmlToPdfConverterService htmlToPdfConverter, IWebHostEnvironment webHostEnvironment)
        {
            this.htmlToPdfConverter = htmlToPdfConverter;
            this.webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("[action]/{reportId}/{reportTypeId}")]
        public IActionResult Print(long reportId,int reportTypeId,string fileName)
        {
            var url = Request.Scheme + "://" + Request.Host;
            var temPath = Path.Combine(webHostEnvironment.WebRootPath, "Temp");

            if (!Directory.Exists(temPath))
            {
                Directory.CreateDirectory(temPath);
            }
            var files = new DirectoryInfo(temPath).GetFiles().Where(u => u.CreationTime < DateTime.Now.AddHours(-4)).Select(u => u);
            foreach(var file in files)
            {
                file.Delete();
            }
            if (fileName == null)
            {
                fileName = "";
            }
            fileName += Guid.NewGuid().ToString() + ".pdf";
            if (System.IO.File.Exists(Path.Combine(temPath, fileName)))
            {
                System.IO.File.Delete(Path.Combine(temPath, fileName));
            }
            htmlToPdfConverter.htmlToPdfConverter.GeneratePdfFromFile(generateReportUrl(reportId, reportTypeId),null, Path.Combine(temPath, fileName));
            return Ok(new { result = url+ "/Temp/"+ fileName });
        }
        private string generateReportUrl(long reportid, int reportTypeId)
        {
            var url = Request.Scheme + "://" + Request.Host + "/pdf";
            switch (reportTypeId)
            {
                case (int)PrintDocumentTypeEnum.Quote:
                    url += "/Quote";
                    break;
                case (int)PrintDocumentTypeEnum.PurchaseOrder:
                    url += "/PurchaseOrder";
                    break;
                case (int)PrintDocumentTypeEnum.Bill:
                    url += "/Bill";
                    break;
                case (int)PrintDocumentTypeEnum.Expense:
                    url += "/Expense";
                    break;
                case (int)PrintDocumentTypeEnum.Invoice:
                    url += "/Invoice";
                    break;
            }
            url += "?id=" + reportid;
            return url;
        }
        [HttpGet("[action]/{reportId}")]
        public IActionResult PrintReceipt(long reportId, string fileName,string purpose)
        {
            var url = Request.Scheme + "://" + Request.Host;
            var temPath = Path.Combine(webHostEnvironment.WebRootPath, "Temp");

            if (!Directory.Exists(temPath))
            {
                Directory.CreateDirectory(temPath);
            }
            var files = new DirectoryInfo(temPath).GetFiles().Where(u => u.CreationTime < DateTime.Now.AddHours(-4)).Select(u => u);
            foreach (var file in files)
            {
                file.Delete();
            }
            if (fileName == null)
            {
                fileName = "";
            }
            fileName += Guid.NewGuid().ToString() + ".pdf";
            if (System.IO.File.Exists(Path.Combine(temPath, fileName)))
            {
                System.IO.File.Delete(Path.Combine(temPath, fileName));
            }
            var reportUrl = Request.Scheme + "://" + Request.Host + "/pdf/Receipt?id="+reportId+ "&purpose="+ purpose; 
            htmlToPdfConverter.htmlToPdfConverter.GeneratePdfFromFile(reportUrl, null, Path.Combine(temPath, fileName));
            return Ok(new { result = url + "/Temp/" + fileName });
        }
        [HttpGet("[action]/{id}")]
        public IActionResult PrintPayslip(long id, string fileName)
        {
            var url = Request.Scheme + "://" + Request.Host;
            var temPath = Path.Combine(webHostEnvironment.WebRootPath, "Temp");

            if (!Directory.Exists(temPath))
            {
                Directory.CreateDirectory(temPath);
            }
            var files = new DirectoryInfo(temPath).GetFiles().Where(u => u.CreationTime < DateTime.Now.AddHours(-4)).Select(u => u);
            foreach (var file in files)
            {
                file.Delete();
            }
            if (fileName == null)
            {
                fileName = "";
            }
            fileName += Guid.NewGuid().ToString() + ".pdf";
            if (System.IO.File.Exists(Path.Combine(temPath, fileName)))
            {
                System.IO.File.Delete(Path.Combine(temPath, fileName));
            }
            var reportUrl = Request.Scheme + "://" + Request.Host + "/pdf/Payslip?id=" + id;
            htmlToPdfConverter.htmlToPdfConverter.GeneratePdfFromFile(reportUrl, null, Path.Combine(temPath, fileName));
            return Ok(new { result = url + "/Temp/" + fileName });
        }
        [HttpPost("[action]/{month}")]
        public IActionResult PrintPayslips(string month,[FromBody] List<int> ids)
        {
            var url = Request.Scheme + "://" + Request.Host;
            var temPath = Path.Combine(webHostEnvironment.WebRootPath, "Temp");

            if (!Directory.Exists(temPath))
            {
                Directory.CreateDirectory(temPath);
            }
            var files = new DirectoryInfo(temPath).GetFiles().Where(u => u.CreationTime < DateTime.Now.AddHours(-4)).Select(u => u);
            foreach (var file in files)
            {
                file.Delete();
            }
            string fileName = "Payroll" + month+Guid.NewGuid().ToString() + ".pdf";
            if (System.IO.File.Exists(Path.Combine(temPath, fileName)))
            {
                System.IO.File.Delete(Path.Combine(temPath, fileName));
            }
            var reportUrl = Request.Scheme + "://" + Request.Host + "/pdf/Payslip?id=";

            htmlToPdfConverter.htmlToPdfConverter.GeneratePdfFromFiles(ids.Select(u=> reportUrl+u).ToArray(), null, Path.Combine(temPath, fileName));
            return Ok(new { result = url + "/Temp/" + fileName });
        }

       

    }
}
