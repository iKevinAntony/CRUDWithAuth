using CRUDWithAuth.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDWithAuth.Helpers
{
    public class CaptureExceptions : ExceptionFilterAttribute
    {
        private ILogger<CaptureExceptions> _Logger;
        public CaptureExceptions(ILogger<CaptureExceptions> logger)
        {
            _Logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            ResponseDTO apiError = new ResponseDTO();
            if (context.Exception is ApiException)
            {
                // handle explicit 'known' API errors
                var ex = context.Exception as ApiException;
                //context.Exception = null;
                apiError.IsSuccess = false;
                apiError.Message = ex.Message;
                apiError.ResponseCode = ex.StatusCode;
                context.HttpContext.Response.StatusCode = ex.StatusCode;
                _Logger.LogError("Controller : " + context.RouteData.Values["controller"] + ", Action : " + context.RouteData.Values["action"] + ". Error Message : " + ex.Message);
            }
            else
            {
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;
                apiError.IsSuccess = false;
                apiError.ResponseCode = 500;
                apiError.Message = msg == null ? "There is some problem now. Please try after some time" : msg;
                context.HttpContext.Response.StatusCode = 500;
                _Logger.LogError("Controller : " + context.RouteData.Values["controller"] + ", Action : " + context.RouteData.Values["action"] + ". Error Message : " + msg);
                // _Logger.LogError(msg);
            }
            // always return a JSON result
            context.Result = new JsonResult(apiError);
            base.OnException(context);

        }

    }
}
