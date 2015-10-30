using FirebirdSql.Data.FirebirdClient;
using System.Data.Entity;
using PL.BookKeeping.Entities;

namespace PL.BookKeeping.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base(new FbConnection(@"database=localhost:D:\Projects\TSF\ReceiptProcessor\Trunk\Database\BookKeeping.fdb;user=sysdba;password=masterkey"), true)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}