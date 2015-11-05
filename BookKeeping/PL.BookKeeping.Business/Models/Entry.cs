using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Business.Models
{
    public class Entry : PL.BookKeeping.Entities.Entry
    {
        public void Total()
        {
            foreach (var childEntry in ChildEntries)
            {

            }
        }
    }
}
