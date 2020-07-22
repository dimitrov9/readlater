using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using System.Diagnostics;

namespace API.Filters
{
    public class LoggingFilter : ActionFilterAttribute, IActionFilter
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnActionExecuting(HttpActionContext actionExecutingContext)
        {
            var filterContext = actionExecutingContext;

            var logLine = new LogLine
            {
                Controller = filterContext.ActionDescriptor != null && filterContext.ActionDescriptor.ControllerDescriptor != null ? filterContext.ActionDescriptor.ControllerDescriptor.ControllerName : "N/A",
                Action = filterContext.ActionDescriptor != null ? filterContext.ActionDescriptor.ActionName : "N/A",
                TimeStamp = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.Date != null ? filterContext.Request.Headers.Date.ToString() : "N/A",
                IPAddress = filterContext.Request != null && filterContext.Request.Headers.Host != null ? filterContext.Request.Headers.Host : "N/A",
                UserId = ((ApiController)filterContext.ControllerContext.Controller).User.Identity.Name,
                Method = filterContext.Request != null && filterContext.Request.Method != null ? filterContext.Request.Method.Method : "N/A",
                Parameters = filterContext.Request != null && filterContext.Request.RequestUri != null ? filterContext.Request.RequestUri.Query : "N/A",
                RawUrl = filterContext.Request != null && filterContext.Request.RequestUri != null ? filterContext.Request.RequestUri.AbsolutePath : "N/A",
                UrlReferer = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.Referrer != null ? filterContext.Request.Headers.Referrer.ToString() : "N/A",
                Platform = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.Referrer != null ? filterContext.Request.Headers.Referrer.ToString() : "N/A",
                UserAgent = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.UserAgent != null ? filterContext.Request.Headers.UserAgent.ToString() : "N/A",
                QueryStringParameters = string.Empty,
                TotalMilliseconds = "0"
            };

            if (filterContext.ActionArguments != null && filterContext.ActionArguments.Count() > 0)
            {
                foreach (var argument in filterContext.ActionArguments)
                {
                    logLine.QueryStringParameters += argument.Key + ": " + JsonConvert.SerializeObject(argument.Value) + " .....";
                }
            }

            var timer = GetTimer(actionExecutingContext.Request);
            timer.Start();

            try
            {
                if (log.IsDebugEnabled)
                    log.DebugFormat(@"OnActionExecuting >>> Controller: {0} Action: {1} TimeStamp: {2} IPAddress: {3} UserID: {4} Method: {5} IsAjax: {6} Parameters: {7} FormData: {8} RawUrl: {9} Platform: {10} UserAgent: {11} UrlReferer: {12} ActionArguments: {13} ",
                    logLine.Controller, logLine.Action, logLine.TimeStamp, logLine.IPAddress, logLine.UserId, logLine.Method, "true", logLine.QueryStringParameters, "null", logLine.RawUrl, logLine.Platform, logLine.UserAgent, logLine.UrlReferer, logLine.QueryStringParameters);
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) { log.ErrorFormat("Message: {0} Exception: {1}", ex.Message, ex); }
            }

            base.OnActionExecuting(actionExecutingContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var filterContext = actionExecutedContext.ActionContext;

            var timer = GetTimer(actionExecutedContext.Request);
            timer.Stop();
            var totalMilliseconds = timer.ElapsedMilliseconds;

            var logLine = new LogLine
            {
                Controller = filterContext.ActionDescriptor != null && filterContext.ActionDescriptor.ControllerDescriptor != null ? filterContext.ActionDescriptor.ControllerDescriptor.ControllerName : "N/A",
                Action = filterContext.ActionDescriptor != null ? filterContext.ActionDescriptor.ActionName : "N/A",
                TimeStamp = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.Date != null ? filterContext.Request.Headers.Date.ToString() : "N/A",
                IPAddress = filterContext.Request != null && filterContext.Request.Headers.Host != null ? filterContext.Request.Headers.Host : "N/A",
                UserId = ((ApiController)filterContext.ControllerContext.Controller).User.Identity.Name,
                Method = filterContext.Request != null && filterContext.Request.Method != null ? filterContext.Request.Method.Method : "N/A",
                Parameters = filterContext.Request != null && filterContext.Request.RequestUri != null ? filterContext.Request.RequestUri.Query : "N/A",
                RawUrl = filterContext.Request != null && filterContext.Request.RequestUri != null ? filterContext.Request.RequestUri.AbsolutePath : "N/A",
                UrlReferer = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.Referrer != null ? filterContext.Request.Headers.Referrer.ToString() : "N/A",
                Platform = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.Referrer != null ? filterContext.Request.Headers.Referrer.ToString() : "N/A",
                UserAgent = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.UserAgent != null ? filterContext.Request.Headers.UserAgent.ToString() : "N/A",
                QueryStringParameters = string.Empty,
                TotalMilliseconds = totalMilliseconds.ToString()
            };
            
            try
            {
                int totalMillisecondsThreshold = 5000;
                if (log.IsDebugEnabled)
                    log.DebugFormat(@"OnActionExecuted >>> Controller: {0} Action: {1} TimeStamp: {2} IPAddress: {3} UserID: {4} Method: {5} IsAjax: {6} Parameters: {7} FormData: {8} RawUrl: {9} Platform: {10} UserAgent: {11} UrlReferer: {12} TotalMilliseconds: {13}",
                        logLine.Controller, logLine.Action, logLine.TimeStamp, logLine.IPAddress, logLine.UserId, logLine.Method, "true", logLine.Parameters, "null", logLine.RawUrl, logLine.Platform, logLine.UserAgent, logLine.UrlReferer, logLine.TotalMilliseconds);
                if (log.IsWarnEnabled && totalMilliseconds > totalMillisecondsThreshold)
                    log.WarnFormat(@"LongRunningActionDetected >>> Controller: {0} Action: {1} TimeStamp: {2} IPAddress: {3} UserID: {4} Method: {5} IsAjax: {6} Parameters: {7} FormData: {8} RawUrl: {9} Platform: {10} UserAgent: {11} UrlReferer: {12} TotalMilliseconds: {13}",
                        logLine.Controller, logLine.Action, logLine.TimeStamp, logLine.IPAddress, logLine.UserId, logLine.Method, "true", logLine.Parameters, "null", logLine.RawUrl, logLine.Platform, logLine.UserAgent, logLine.UrlReferer, logLine.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) { log.ErrorFormat("Message: {0} Exception: {1}", ex.Message, ex); }
            }

            base.OnActionExecuted(actionExecutedContext);
        }

        public Stopwatch GetTimer(HttpRequestMessage request)
        {
            const string key = "__timer__";
            if (request.Properties.ContainsKey(key))
            {
                return (Stopwatch)request.Properties[key];
            }

            var result = new Stopwatch();
            request.Properties[key] = result;
            return result;
        }
    }

    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(HttpActionExecutedContext filterContext)
        {
            string exceptionType = "";

            var logLine = new LogLine
            {
                Controller = filterContext.ActionContext != null && filterContext.ActionContext.ControllerContext != null && filterContext.ActionContext.ControllerContext.ControllerDescriptor != null ? filterContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName : "N/A",
                Action = filterContext.ActionContext.ActionDescriptor != null ? filterContext.ActionContext.ActionDescriptor.ActionName : "N/A",
                TimeStamp = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.Date != null ? filterContext.Request.Headers.Date.ToString() : "N/A",
                IPAddress = filterContext.Request != null && filterContext.Request.Headers.Host != null ? filterContext.Request.Headers.Host : "N/A",
                UserId = ((ApiController)filterContext.ActionContext.ControllerContext.Controller).User.Identity.Name,
                Method = filterContext.Request != null && filterContext.Request.Method != null ? filterContext.Request.Method.Method : "N/A",
                Parameters = filterContext.Request != null && filterContext.Request.RequestUri != null ? filterContext.Request.RequestUri.Query : "N/A",
                RawUrl = filterContext.Request != null && filterContext.Request.RequestUri != null ? filterContext.Request.RequestUri.AbsolutePath : "N/A",
                UrlReferer = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.Referrer != null ? filterContext.Request.Headers.Referrer.ToString() : "N/A",
                Platform = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.Referrer != null ? filterContext.Request.Headers.Referrer.ToString() : "N/A",
                UserAgent = filterContext.Request != null && filterContext.Request.Headers != null && filterContext.Request.Headers.UserAgent != null ? filterContext.Request.Headers.UserAgent.ToString() : "N/A",
                QueryStringParameters = string.Empty,
                TotalMilliseconds = "0"
            };

            if (filterContext.ActionContext.ActionArguments != null && filterContext.ActionContext.ActionArguments.Count() > 0)
            {
                foreach (var argument in filterContext.ActionContext.ActionArguments)
                {
                    logLine.QueryStringParameters += argument.Key + ": " + JsonConvert.SerializeObject(argument.Value) + " .....";
                }
            }

            if (!String.IsNullOrWhiteSpace(logLine.Action))
                exceptionType += logLine.Action;
            if (filterContext.Exception.GetType() != null)
                exceptionType += " : " + filterContext.Exception.GetType().ToString();
            if (filterContext.Exception.InnerException != null)
                exceptionType += " : " + filterContext.Exception.InnerException.GetType().ToString();
            if (exceptionType == "")
                exceptionType = "UserAccountsException";

            log4net.GlobalContext.Properties["Subject"] = exceptionType;
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                if (log.IsErrorEnabled)
                    log.ErrorFormat(@"OnExcepition >>> Controller: {0} Action: {1} UserID: {2} Method: {3} IsAjax: {4} Parameters: {5} FormData: {6} RawUrl: {7} Platform: {8} UserAgent: {9} UrlReferer: {10} ActionArguments: {11} Message: {12} Exception: {13} ",
                    logLine.Controller, logLine.Action, logLine.UserId, logLine.Method, "true", logLine.Parameters, "null", logLine.RawUrl, logLine.Platform, logLine.UserAgent, logLine.UrlReferer, logLine.QueryStringParameters, filterContext.Exception.Message, filterContext.Exception);
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled) { log.ErrorFormat("Message: {0} Exception: {1}", ex.Message, ex); }
            }

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("An error occurred, please try again or contact the administrator."),
                ReasonPhrase = "Critical Exception"
            });
        }
    }

    public class LogLine
    {
        public string LogLineId { get; set; }
        public string LogLineRev { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string TimeStamp { get; set; }
        public string IPAddress { get; set; }
        public string UserId { get; set; }
        public string Method { get; set; }
        public string Parameters { get; set; }
        public string RawUrl { get; set; }
        public string UrlReferer { get; set; }
        public string Platform { get; set; }
        public string UserAgent { get; set; }
        public string QueryStringParameters { get; set; }
        public string TotalMilliseconds { get; set; }
    }
}