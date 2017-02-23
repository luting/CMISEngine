using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CMISEngine.CMIS.Configuration
{
    public class CMISConnection
    {
        public static string BaseUrl { get; set; }
        public static string User { get; set; }
        public static string Password { get; set; }
        public static string DocumentRepository { get; set; }
        public static string ExternalApplicationProperty { get; set; }
        public static string SiteName { get; set; }
        public static CMISConnectionSection _Config = ConfigurationManager.GetSection("CMISConnection") as CMISConnectionSection;

        public static void GetParameters()
        {
            try { 
            Dictionary<string, string> pamameterDictionary = new Dictionary<string, string>();
            foreach (ParameterElement parameterElement in _Config.Parameters)
            {
                if (!pamameterDictionary.ContainsKey(parameterElement.Name))
                {
                    pamameterDictionary[parameterElement.Name] = parameterElement.Value;
                }
            }
            BaseUrl = pamameterDictionary["BaseUrl"];
            User = pamameterDictionary["User"];
            Password = pamameterDictionary["Password"];
            DocumentRepository = pamameterDictionary["DocumentRepository"];
            ExternalApplicationProperty = pamameterDictionary["ExternalApplicationProperty"];
            SiteName = pamameterDictionary["SiteName"];
            }
            catch
            {
                throw new Exception("Merci de vérifier les parametres de CMIS connection dans Web.Config");
            }
            }
    }
}