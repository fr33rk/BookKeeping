using PL.BookKeeping.Infrastructure.Data;

namespace PL.BookKeeping.Infrastructure
{
	public class Settings
	{
		public string Sever { get; set; } = "localhost";
		public string Name { get; set; } = "Bookkeeping";
		public string UserId { get; set; } = "Bookkeeper";
		public string Password { get; set; } = "books";
	}
}