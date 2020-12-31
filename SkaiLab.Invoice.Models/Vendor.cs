using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
	public class Vendor
	{
		public long Id { get; set; }
		public string OrganisationId { get; set; }
		public string LegalName { get; set; }
		public string DisplayName { get; set; }
		public int CurrencyId { get; set; }
		public Currency Currency { get; set; }
		public string TaxNumber { get; set; }
		public string BusinessRegistrationNumber { get; set; }
		public string ContactId { get; set; }
		public Contact Contact { get; set; }
		public string LocalLegalName { get; set; }
	}
}
