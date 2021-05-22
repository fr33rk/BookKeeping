using System.Collections.Generic;

namespace BookKeeping.Client.Models
{
	public class OptionsVm
	{
		public string ServerName { get; set; }
		public string DatabaseName { get; set; }
		public string UserId { get; set; }
		public string Password { get; set; }
		public IEnumerable<string> AdministeredAccounts { get; set; } = new List<string>();
		public string Separator { get; set; }
	}
}