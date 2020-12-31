using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;
using System;
namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CurrencyController : ParentController
    {
        private readonly ICurrencyService currencyService;
        public CurrencyController(ICurrencyService currencyService) : base(currencyService)
        {
            this.currencyService = currencyService;
        }
        [HttpGet("[action]")]
        public IActionResult GetOrganisationCurrencies()
        {
            return Ok(currencyService.GetCurrencies(currencyService.OrganisationId));
        }
        [HttpGet("[action]")]
        public IActionResult GetCurrenciesWithoutNote()
        {
            return Ok(currencyService.GetCurrenciesWithoutNote(currencyService.OrganisationId));
        }
        [HttpGet("[action]")]
        public IActionResult GetCurrenciesForCreate()
        {
            return Ok(currencyService.GetCurrenciesForCreate(currencyService.OrganisationId));
        }
        [HttpPost("[action]")]
        public IActionResult Create([FromBody] Currency currency)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                currencyService.CreateCurrency(currencyService.OrganisationId,currency);
                return Ok();
            }
            catch(Exception ex)
            {
                return  BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpGet("[action]/{id}")]
        public IActionResult GetCurrencyWithExchangeRate(int id)
        {
            try
            {
                return Ok(currencyService.GetCurrencyWithExchangeRate(id, currencyService.OrganisationId));
            }
            catch(Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpPost("[action]")]
        public IActionResult UpdateExchangeRate([FromBody] Currency currency)
        {
            try
            {
                EnsureHasPermission((int)MenuFeatureEnum.ManageOrganisactionSetting);
                currencyService.UpdateExchangeRate(currencyService.OrganisationId, currency);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new Error { ErrorText = ex.Message });
            }
        }
        [HttpGet("[action]")]
        public IActionResult GetCurrenciesWithExchangeRate()
        {
            return Ok(currencyService.GetCurrenciesWithExchangeRate(currencyService.OrganisationId));
        }
        [HttpGet("[action]")]
        public IActionResult GetCurrenciesWithoutOrganisation()
        {
            return Ok(currencyService.GetCurrencies());
        }
    }
}
