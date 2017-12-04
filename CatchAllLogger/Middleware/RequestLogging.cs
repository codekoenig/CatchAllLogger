using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatchAllLogger.Middleware
{
    public class RequestLogging
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public RequestLogging(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            loggerFactory.AddConsole();
            logger = loggerFactory.CreateLogger<RequestLogging>();
        }

        public async Task Invoke(HttpContext context)
        {
            string loggedRequest = await FormatRequest(context.Request);

            context.Items.Add("RequestRawContent", loggedRequest);
            logger.LogInformation(loggedRequest);

            await next(context);

            //code dealing with response
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            StringBuilder logMessageBuilder = new StringBuilder();
            request.EnableRewind();

            StreamReader sr = new StreamReader(request.Body);
            string bodyText = await sr.ReadToEndAsync();

            List<string> headers = request.Headers.Select(h => String.Concat(h.Key.PadRight(25, ' '), h.Value)).ToList();

            logMessageBuilder.AppendLine("\r\n------------------------------------------------------------------------------------------");
            logMessageBuilder.AppendLine($"{request.Method} {request.Scheme}://{request.Host}{request.Path}{request.QueryString}");
            headers.ForEach(h => logMessageBuilder.AppendLine(h));
            logMessageBuilder.AppendLine(bodyText);
            logMessageBuilder.AppendLine("------------------------------------------------------------------------------------------\r\n");

            return logMessageBuilder.ToString();
        }
    }
}
