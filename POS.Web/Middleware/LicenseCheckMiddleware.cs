using POS.Application.Services;

namespace POS.Web.Middleware
{
    public class LicenseCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public LicenseCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILicenseService licenseService)
        {
            var path = context.Request.Path.Value.ToLower();
            if (path.StartsWith("/license") || path.StartsWith("/lib") || path.StartsWith("/css"))
            {
                await _next(context);
                return;
            }

            var isAllowed = await licenseService.IsFeatureAccessAllowedAsync();

            if (!isAllowed)
            {
                context.Response.Redirect("/License/Index");
                return;
            }

            await _next(context);
        }
    }
}

