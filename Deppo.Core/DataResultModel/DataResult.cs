using System;

namespace Deppo.Core.DataResultModel;

public class DataResult<T> where T : class
{
	public T? Data { get; set; }
	public bool IsSuccess { get; set; } = false;
	public string Message { get; set; } = string.Empty;
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 100;
	public int TotalRecord { get; set; }
}
