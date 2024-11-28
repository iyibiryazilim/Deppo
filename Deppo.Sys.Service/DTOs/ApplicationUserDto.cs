using Deppo.Sys.Service.Models;

namespace Deppo.Sys.Service.DTOs
{
	public class ApplicationUserDto
	{
		public bool ChangePasswordOnFirstLogon { get; set; } = true;
		public string UserName { get; set; } = string.Empty;
		public bool IsActive { get; set; }
		public string Code { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string TCKN { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string Telephone { get; set; } = string.Empty;
		public string OtherTelephone { get; set; } = string.Empty;
		public string WebAddress { get; set; } = string.Empty;
		public string EMail { get; set; } = string.Empty;
		public string ClientMachineName { get; set; } = string.Empty;
		public string LogoUserName { get; set; } = string.Empty;
		public Guid Image { get; set; }
	}
}

