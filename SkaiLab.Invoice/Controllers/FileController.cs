using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IService service;
        public FileController(IService service)
        {
            this.service = service;
        }
        [HttpPost("[action]")]
        public IActionResult Upload([FromBody]Attachment file)
        {
            try
            {
                return Ok(new File {Url= service.UploadFile(file.FileUrl,file.FileName) });
            }
            catch (Exception ex)
            {
                return BadRequest(new Error(ex.Message));
            }
        }
    }
}
