using Microsoft.AspNetCore.Http;

namespace AzulSchoolProject.Settings
{
    public class CookieSettings
    {
        // Auth (JWT) cookie
        public string AuthCookieName { get; set; } = "AuthToken";
        public bool AuthHttpOnly { get; set; } = true;
        public bool AuthSecure { get; set; } = true; // HTTPS required when SameSite=None
        public SameSiteMode AuthSameSite { get; set; } = SameSiteMode.None; // allow cross-site requests
        public int AuthExpiresMinutes { get; set; } = 60; // match token lifetime

        // Refresh token cookie
        public string RefreshCookieName { get; set; } = "RefreshToken";
        public int RefreshExpiresDays { get; set; } = 45;

        // Antiforgery (readable) cookie
        public string XsrfCookieName { get; set; } = "XSRF-TOKEN";
        public bool XsrfHttpOnly { get; set; } = false;
        public bool XsrfSecure { get; set; } = true;
        public SameSiteMode XsrfSameSite { get; set; } = SameSiteMode.None;

        // Frontend origin used by CORS and CSRF checks (centralized)
        public string AllowedFrontendOrigin { get; set; } = "https://your-frontend.example.com"; // Once I have Frontend URL, replace this with the actual URL of your frontend application
    }
}