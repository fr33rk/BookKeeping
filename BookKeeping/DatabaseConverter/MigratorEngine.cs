using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PL.BookKeeping.Entities;

namespace DatabaseConverter
{
    public class MigratorEngine
    {
	    public void ExportToFile()
	    {
		    var output = new StringBuilder();




		    var testUser = new User() {CreationDT = DateTime.Now, Key = 1, Name = "Freerk"};

		    var testOutput = JsonConvert.SerializeObject(testUser);
	    }
    }
}
