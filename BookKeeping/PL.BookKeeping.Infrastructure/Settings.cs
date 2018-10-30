﻿namespace PL.BookKeeping.Infrastructure
{
	public class Settings
	{
		public string ServerName { get; set; } = "localhost";
		public string DatabaseName { get; set; } = "Bookkeeping";
		public string UserId { get; set; } = "Bookkeeper";
		public string Password { get; set; } = "books";
	}
}