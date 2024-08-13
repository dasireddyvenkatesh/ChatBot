namespace ChatBot
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (context.Request.Path.StartsWithSegments("/Public", StringComparison.OrdinalIgnoreCase))
                {
                    await _next(context);
                    return;
                }

                var encryptedData = context.Request.Cookies["MUID"];

                if (string.IsNullOrWhiteSpace(encryptedData))
                {
                    var requestedController = context.GetRouteValue("controller")?.ToString();

                    if (requestedController == "Public")
                    {
                        await _next(context);
                        return;
                    }

                    context.Response.Redirect("/Public");
                    return;
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"An error occurred while processing the request: {ex.Message}");
            }
        }
    }
}
