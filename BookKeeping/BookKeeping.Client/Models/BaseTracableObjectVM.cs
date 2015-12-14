using PL.BookKeeping.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookKeeping.Client.Models
{
    public class BaseTracableObjectVM
    {
        /// <summary>The unique identifier for this object. Should never be manually set as it is 
        /// manager by the Entity Framework.
        /// </summary>
        public int Key { get; set; }

        /// <summary>Gets or sets the creation date and time. Should never be manually set.
        /// </summary>
        public DateTime CreationDT { get; set; }

        public int? CreatorKey { get; set; }

        /// <summary>Gets or sets the creator. Should never be manually set.
        /// </summary>        
        public virtual User Creator { get; set; }
    }
}
