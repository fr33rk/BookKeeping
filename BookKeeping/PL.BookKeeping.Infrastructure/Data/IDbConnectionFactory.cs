using Microsoft.EntityFrameworkCore;

namespace PL.BookKeeping.Infrastructure.Data
{
	public interface IDbConnectionFactory
	{
		DbContextOptions Create();
	}
}