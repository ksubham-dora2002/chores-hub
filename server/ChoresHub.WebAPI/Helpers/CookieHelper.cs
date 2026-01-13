namespace ChoresHub.WebAPI.Helpers
{
    public static class CookieHelper
    {
        public static void SetCookie(HttpResponse response, string key, string value, TimeSpan expirationTime, bool isHttpOnly = true)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = isHttpOnly,
                Expires = DateTime.UtcNow.Add(expirationTime),
                SameSite = SameSiteMode.Lax, 
                Secure = false, 

                // For production
                // SameSite = SameSiteMode.None, 
                // Secure = true, 

            };
            response.Cookies.Append(key, value, cookieOptions);
        }
        public static void SetSessionCookie(HttpResponse response, string key, string value, bool isHttpOnly = true)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = isHttpOnly,
                SameSite = SameSiteMode.None,
                Secure = false,

                // For production
                // Secure = true, 
            };
            response.Cookies.Append(key, value, cookieOptions);
        }
        public static void DeleteCookie(HttpResponse response, string key)
        {
            response.Cookies.Delete(key);
        }
    }
}