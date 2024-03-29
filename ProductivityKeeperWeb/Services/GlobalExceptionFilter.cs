﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Net.Http;
using IExceptionFilter = Microsoft.AspNetCore.Mvc.Filters.IExceptionFilter;

namespace ProductivityKeeperWeb.Services
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch(context.Exception)
            {
                case UnauthorizedAccessException:
                    context.HttpContext.Response.StatusCode = 401;
                    break;
                default:
                    context.HttpContext.Response.StatusCode = 500;
                    break;
            }

            var error = context.Exception.InnerException?.Message ?? context.Exception.Message;

            context.Result = new JsonResult(new { message = error, Status = context.HttpContext.Response.StatusCode });
        }
    }
}
