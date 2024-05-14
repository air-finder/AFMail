using System.Net;
using Domain.Common;
using Domain.Exceptions;
using Domain.SeedWork.Notification;
using Infra.Utils.Constants;
using Newtonsoft.Json;

namespace API.Middlewares
{
    public class ControllerMiddleware(RequestDelegate next)
    {
        private static readonly bool IsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        public async Task Invoke(HttpContext context, INotification notification)
        {
            try
            {
                await next(context);
            }
            catch (NotificationException)
            {
                await HandleNotificationExceptionAsync(context, notification.Notifications);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleNotificationExceptionAsync(HttpContext context, List<NotificationModel> notifications)
        {
            var result = new GenericResponse<object>(null);
            notifications.ForEach(x => result.AddError(x.Message));
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = new GenericResponse<object>(null);
            result.AddError(IsDevelopment ? exception.Message : RequestErrorResponseConstant.InternalError);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
