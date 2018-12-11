using System.Data.Common;

namespace PL.BookKeeping.Infrastructure.Data
{
	public interface IDbConnectionFactory
	{
		DbConnection Create();
	}
}