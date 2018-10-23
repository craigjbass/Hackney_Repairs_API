using System;
using HackneyRepairs.Models;
using Microsoft.AspNetCore.Mvc;

namespace HackneyRepairs.Builders
{
    public static class ResponseBuilder
    {
        public static JsonResult Error(int errorCode, string userMessage, string developerMessage)
        {
            var error = new ApiErrorMessage
            {
                DeveloperMessage = developerMessage,
                UserMessage = userMessage
            };
            var jsonResponse = new JsonResult(error)
            {
                StatusCode = errorCode
            };

            return jsonResponse;
        }

        public static JsonResult Ok(object responseContent)
        {
            var jsonResponse = new JsonResult(responseContent)
            {
                StatusCode = 200,
                ContentType = "application/json"
            };

            return jsonResponse;
        }
    }
}
