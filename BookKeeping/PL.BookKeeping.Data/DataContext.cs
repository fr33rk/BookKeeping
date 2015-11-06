using System.Data.Entity;
using FirebirdSql.Data.FirebirdClient;
using PL.BookKeeping.Entities;

namespace PL.BookKeeping.Data
{
    public class DataContext : DbContext
    {
        /// <summary>Gets or sets the current user.</summary>
        /// <value>The current user.</value>
        public User CurrentUser { get; set; }

        public DataContext()                             
            : base(new FbConnection(@"database=localhost:D:\Ontwikkeling\Eigen\BookKeeping\BookKeeping\BookKeeping.Client\bin\Debug\BookKeeping.fdb;user=sysdba;password=masterkey"), true)
            //: base(new FbConnection(@"database=localhost:D:\Projects\TSF\BookKeeping\BookKeeping\Database\BookKeeping.fdb;user=sysdba;password=masterkey"), true)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}