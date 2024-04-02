using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services
{
    public class TSVLoadService
    {
        public List<T> LoadCsv<T>(string filePath, int numberOfLines)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "\t" }))
            {
                // Limit the number of lines by using Take(numberOfLines)
                return csv.GetRecords<T>().Take(numberOfLines).ToList();
            }
        }
    }
}
