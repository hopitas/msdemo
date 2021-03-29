using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MS.Function
{
    public class GetDailyRequests
    {
        [FunctionName("GetDailyRequests")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            string savelocation = req.Query["savelocation"];
            System.Net.WebClient client = new WebClient();
            DateTime now = DateTime.UtcNow;
            string receivelocation = "https://storageaccountmsdem9003.blob.core.windows.net/message-container/" + "log_" + now.Date.Year.ToString() + now.Date.Month.ToString("00") + now.Date.Day.ToString("00") + ".csv";
            byte[] filebytes = client.DownloadData(receivelocation);
            return new FileContentResult(filebytes, "application/octet-stream")
            {
                FileDownloadName = "Export.csv"
            };

        }
    }
}