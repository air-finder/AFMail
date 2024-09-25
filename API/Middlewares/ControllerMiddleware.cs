using System.Net;
using System.Text;
using Domain.Common;
using Domain.Exceptions;
using Domain.SeedWork.Notification;
using Infra.Utils.Constants;
using Newtonsoft.Json;
using static System.String;

namespace API.Middlewares
{
    public class ControllerMiddleware(RequestDelegate next, ILogger<ControllerMiddleware> logger)
    {
        private static readonly bool IsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        private string _body = Empty;
        public async Task Invoke(HttpContext context, INotification notification)
        {
            try
            {
                _body = await GetBody(context.Request);
                await next(context);
                LogInformation(context);
            }
            catch (NotificationException)
            {
                await HandleNotificationExceptionAsync(context, notification.Notifications);
            }
            catch (NotAllowedException)
            {
                await HandleNotAllowedException(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task<string> GetBody(HttpRequest request)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        #region HANDLERS
        private async Task HandleNotificationExceptionAsync(HttpContext context, List<NotificationModel> notifications)
        {
            var result = new GenericResponse<object>();
            notifications.ForEach(x => result.AddError(x.Message));
            UpdateContext(context, HttpStatusCode.BadRequest);
            LogError(context);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = new GenericResponse<object>();
            result.AddError(IsDevelopment ? exception.Message : RequestErrorResponseConstant.InternalError);
            UpdateContext(context, HttpStatusCode.InternalServerError);
            LogError(context);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        private async Task HandleNotAllowedException(HttpContext context)
        {
            var result = new GenericResponse<object>();
            result.AddError(RequestErrorResponseConstant.NotAllowed);
            UpdateContext(context, HttpStatusCode.MethodNotAllowed);
            LogError(context);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
        private static void UpdateContext(HttpContext context, HttpStatusCode code)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
        }
        #endregion

        #region LOGGING
        private static string LogMessage => "Request {method} {url} returned HttpStatusCode {statusCode}";
        private static string LogMessageQuery => LogMessage + "; QueryString: {queryString}";
        private static string LogMessageBody => LogMessage + "; Body: {body}";

        private void LogInformation(HttpContext context)
        {
            switch (context.Request.Method)
            {
                case "GET" when !IsNullOrEmpty(context.Request.QueryString.ToString()):
                    logger.LogInformation(
                        LogMessageQuery,
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        context.Request.QueryString
                    );
                    break;
                case "POST":
                case "PUT":
                case "PATCH":
                    logger.LogInformation(
                        LogMessageBody,
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        _body
                    );
                    break;
                default:
                    logger.LogInformation(
                        LogMessage,
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode
                    );
                    break;
            }
        }

        private void LogError(HttpContext context)
        {
            switch (context.Request.Method)
            {
                case "GET" when !IsNullOrEmpty(context.Request.QueryString.ToString()):
                    logger.LogError(
                        LogMessageQuery,
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        context.Request.QueryString
                    );
                    break;
                case "POST":
                case "PUT":
                case "PATCH":
                    logger.LogError(
                        LogMessageBody,
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        _body
                    );
                    break;
                default:
                    logger.LogError(
                        LogMessage,
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode
                    );
                    break;
            }
        }
        #endregion
    }
}
