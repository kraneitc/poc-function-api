using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace poc_function_api
{
    public static class Customers
    {
        [FunctionName("Customers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = null)] HttpRequest req,
            ILogger log)
        {

            string result;
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            int.TryParse(req.Query["CustomerID"], out var id);

            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var command = new SqlCommand("[SalesLT].[GetCustomer]", conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@CustomerID", id));

                var rows = await command.ExecuteReaderAsync();

                var r = Serializer.Serialize(rows); 
                result = JsonConvert.SerializeObject(r, Formatting.Indented);
            }

            return new OkObjectResult(result);
        }
    }
}
