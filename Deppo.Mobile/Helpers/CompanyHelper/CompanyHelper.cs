using System.Diagnostics;

namespace Deppo.Mobile.Helpers.CompanyHelper;

public static class CompanyHelper
{
	public static async Task<int> GetCompanyNumberAsync()
	{
		try
		{
			var result = await SecureStorage.GetAsync("CompanyNumber");

			if (result is null)
				return 0;

			return int.Parse(result);
		}
		catch(Exception ex)
		{
			Debug.WriteLine(ex.Message);
			throw;
		}
	}

	public static async Task<string> GetFormattedCompanyNumber()
	{
		try
		{
			var result = await SecureStorage.GetAsync("CompanyNumber");

			if (result is null)
				return string.Empty;

			return result.PadLeft(3, '0');
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			throw;
		}
	}

	public static async Task<string> GetCompanyPeriodAsync()
	{
		try
		{
			var result = await SecureStorage.GetAsync("CompanyPeriod");

			if (result is null)
				return string.Empty;

			return result.PadLeft(2, '0');
		}
		catch(Exception ex)
		{
			Debug.WriteLine(ex.Message);
			throw;
		}
	}
}
