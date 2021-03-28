using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace MS.Function
{
    public class GetDailyRequests
    {
        [FunctionName("GetDailyRequests")]
        public async void Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            string savelocation = req.Query["savelocation"];
            System.Net.WebClient client = new WebClient();
            DateTime now = DateTime.UtcNow;
            string receivelocation = "containerurl" + "log_" + now.Date.Year.ToString() + now.Date.Month.ToString("00") + now.Date.Day.ToString("00") + ".csv";
            client.DownloadFile(receivelocation, savelocation);
        }
    }
}