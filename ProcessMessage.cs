using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using MS.model;
using MS.Utils;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MS.Function
{
    public class ProcessMessage
    {
        private readonly IConfiguration config;
        public ProcessMessage(IConfiguration config)
        {

            this.config = config;

        }
        [FunctionName("ProcessMessage")]
        [return: Table("MessageTable")]
        public async Task<MessageTable> RunAsync(
            [QueueTrigger("outqueue", Connection = "AzureWebJobsStorage")] string queItem)
        {
            Message message = JsonConvert.DeserializeObject<Message>(queItem);
            Util util = new Util();
            MessageString mstring = util.convertMessage(message);
            InsertSQL(message);
            InsertTableStorage(mstring);
            SendMailAsync(mstring);
            return new MessageTable { PartitionKey = mstring.Key, RowKey = mstring.Email, Atributes = mstring.Attributes };
        }

        private async Task SendMailAsync(MessageString mstring)
        {
            var apiKey = config.GetConnectionString("ApiKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "Message Sender");
            var subject = "Congratulations, you got mail";
            var to = new EmailAddress(mstring.Email, "Receiver");
            var plainTextContent = mstring.Attributes;
            var htmlContent = mstring.Attributes;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        private void InsertTableStorage(MessageString mstring)
        {
            var tableConn = config.GetConnectionString("TableStorage");
            BlobContainerClient container = new BlobContainerClient(tableConn, ("message-container"));
            container.CreateIfNotExists();
            DateTime now = DateTime.UtcNow;
            string fileName = "log_" + now.Date.Year.ToString() + now.Date.Month.ToString("00") + now.Date.Day.ToString("00") + ".csv";
            AppendBlobClient blob = container.GetAppendBlobClient(fileName);
            blob.CreateIfNotExists();
            string hashemail = GetStringSha256Hash(mstring.Email);
            string blobMessage = mstring.Key + ";" + hashemail + ";" + mstring.Attributes + Environment.NewLine;
            using (var stream = GenerateStreamFromString(blobMessage))
            {
                blob.AppendBlock(stream);
                stream.Close();
            }
        }

        private void InsertSQL(Message message)
        {
            var sqlConn = config.GetConnectionString("mycs");
            MS.repositories.MessageRepository messageRepository = new MS.repositories.MessageRepository(sqlConn);
            messageRepository.InsertMessage(message);
        }

        internal static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        internal static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}