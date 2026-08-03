using System;

namespace AzulSchoolProject.Settings
{
    public class ClientSettings
    {
        // Frontend origin used by CORS and CSRF checks (centralized)
        public string AllowedFrontendOrigin { get; set; } = "https://your-frontend.example.com";
    }
}