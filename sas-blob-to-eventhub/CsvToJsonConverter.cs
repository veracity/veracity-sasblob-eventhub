using System;
using System.IO;
using ChoETL;

namespace sas_blob_to_eventhub
{
    /// <summary>
    /// Class responsible for converting csv data to json structured string.
    /// In code there is simplest case, whole csv is converted to one json.
    /// ChoETL nuget package used for fast and simple conversion from csv to json.
    /// https://github.com/Cinchoo/ChoETL
    /// </summary>
    public class CsvToJsonConverter
    {
        /// <summary>
        /// Coverts csv string to json string
        /// </summary>
        /// <param name="csvContent">string content of csv data</param>
        /// <param name="delimeter">delimenter used in csv data</param>
        /// <returns></returns>
        public static string Convert(string csvContent, string delimeter)
        {
            try
            {
                string jsonResult;
                using (var p = new ChoCSVReader(new StringReader(csvContent)).WithFirstLineHeader().WithDelimiter(delimeter))
                {
                    using (var stringWriter = new StringWriter())
                    {
                        using (var w = new ChoJSONWriter(stringWriter))
                        {
                            w.Write(p);
                        }
                        jsonResult = stringWriter.ToString();
                    }
                }
                return jsonResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}