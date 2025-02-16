using System.Drawing.Printing;
 

namespace Integrations.Utils
{
    public static class AppResourceHelper
    {
        public static string Get(string key)
        {
             
            return AppResource.ResourceManager.GetString(key) ?? $"Missing resource: {key}";
        }
    }
}
