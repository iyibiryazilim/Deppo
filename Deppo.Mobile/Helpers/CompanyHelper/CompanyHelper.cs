using System.Diagnostics;

namespace Deppo.Mobile.Helpers.CompanyHelper;

public static class CompanyHelper
{
	/// <summary>
	/// Asynchronously retrieves the company number from secure storage and returns it as an integer.
	/// </summary>
	/// <returns>
	/// The company number as an integer. Returns 0 if the company number is not found.
	/// </returns>
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


	/// <summary>
	/// Asynchronously retrieves the company number from secure storage and formats it as a 3-digit string 
	/// by adding leading zeros. For example, if the stored number is "1", the returned value will be "001".
	/// </summary>
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

	/// <summary>
	/// Asynchronously retrieves the company period from secure storage and formats it as a 2-digit string 
	/// by adding a leading zero if necessary. For example, if the stored period is "2", the returned value will be "02".
	/// </summary>
	public static async Task<string> GetFormattedCompanyPeriodAsync()
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
