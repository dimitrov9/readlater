using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using API.Filters;

namespace API.Handlers
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void Handle(ExceptionHandlerContext context)
        {
#if DEBUG
            context.Result = new TextPlainErrorResult
            {
                Request = context.ExceptionContext.Request,
                Content = context.Exception.InnerException != null
                    ? context.Exception.InnerException.Message
                    : context.Exception.Message
            };
#else
            var exceptionType = context.Exception.GetType().ToString();

            var logLine = new LogLine
            {
                Controller = context.ExceptionContext.ActionContext?.ControllerContext?.ControllerDescriptor?.ControllerName ?? "N/A",
                Action = context.ExceptionContext.ActionContext?.ActionDescriptor?.ActionName ?? "N/A",
                TimeStamp = context.ExceptionContext.Request?.Headers?.Date?.ToString() ?? "N/A",
                IPAddress = context.ExceptionContext.Request?.Headers?.Host ?? "N/A",
                UserId = ((ApiController)context.ExceptionContext.ActionContext?.ControllerContext?.Controller)?.User.Identity.Name,
                Method = context.ExceptionContext.Request?.Method?.Method ?? "N/A",
                Parameters = context.ExceptionContext.Request?.RequestUri?.Query ?? "N/A",
                RawUrl = context.ExceptionContext.Request?.RequestUri?.AbsolutePath ?? "N/A",
                UrlReferer = context.ExceptionContext.Request?.Headers?.Referrer?.ToString() ?? "N/A",
                Platform = context.ExceptionContext.Request?.Headers?.Referrer?.ToString() ?? "N/A",
                UserAgent = context.ExceptionContext.Request?.Headers?.UserAgent?.ToString() ?? "N/A",
                QueryStringParameters = string.Empty,
                TotalMilliseconds = "0"
            };

            if (!string.IsNullOrWhiteSpace(logLine.Action))
                exceptionType = logLine.Action + " : " + exceptionType;

            if (context.ExceptionContext.Exception.InnerException != null)
                exceptionType += " : " + context.ExceptionContext.Exception.InnerException.GetType();

            log4net.GlobalContext.Properties["Subject"] = exceptionType;
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                if (log.IsErrorEnabled)
                {
                    log.ErrorFormat(
                        @"OnExcepition >>> Controller: {0} Action: {1} UserID: {2} Method: {3} IsAjax: {4} Parameters: {5} FormData: {6} RawUrl: {7} Platform: {8} UserAgent: {9} UrlReferer: {10} ActionArguments: {11} Message: {12} Exception: {13} ",
                        logLine.Controller, logLine.Action, logLine.UserId, logLine.Method, "true",
                        logLine.Parameters, "null", logLine.RawUrl, logLine.Platform, logLine.UserAgent,
                        logLine.UrlReferer, logLine.QueryStringParameters, context.Exception.Message,
                        context.Exception);
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                {
                    log.ErrorFormat("Message: {0} Exception: {1}", ex.Message, ex);
                }
            }

            context.Result = new TextPlainErrorResult
            {
                Request = context.ExceptionContext.Request,
                Content = "An error occurred, please try again or contact the administrator."
            };
#endif

        }

        private class TextPlainErrorResult : IHttpActionResult
        {
            public HttpRequestMessage Request { get; set; }

            public string Content { get; set; }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(Content),
                    RequestMessage = Request,
                    ReasonPhrase = "Critical Exception"
                };
                return Task.FromResult(response);
            }
        }
    }
}