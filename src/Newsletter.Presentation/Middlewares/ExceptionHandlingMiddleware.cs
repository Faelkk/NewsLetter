using System.Net;
using System.Text.Json;

namespace Newsletter.Presentation.Middlewares;

public class ExceptionHandlingMiddleware
{
 
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;

  public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
  {
      _next = next;
      _logger = logger;
  }

  public async Task Invoke(HttpContext context)
  {
      try
      {
          await _next(context);
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Erro inesperado aconteceu");
            
          context.Response.ContentType = "application/json";
          context.Response.StatusCode = ex switch
          {
              InvalidOperationException => (int)HttpStatusCode.Conflict,
              KeyNotFoundException => (int)HttpStatusCode.NotFound,
              ArgumentException => (int)HttpStatusCode.BadRequest,
              _ => (int)HttpStatusCode.InternalServerError
          };
          
          var response = new
          {
              error = ex.Message,
              statusCode = context.Response.StatusCode
          };

          await context.Response.WriteAsync(JsonSerializer.Serialize(response));
      }
  }
  
}