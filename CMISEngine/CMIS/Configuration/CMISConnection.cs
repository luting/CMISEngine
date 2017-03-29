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
        public static string FolderRepository { get; set; }
        public static string ExternalApplicationProperty { get; set; }
        public static string SiteName { get; set; }
        public static string PublishInEvolveProperty { get; set; }
        public static string CustomAspect { get; set; }
        public static CMISConnectionSection _Config = ConfigurationManager.GetSection("CMISConnection") as CMISConnectionSection;

        public static void GetParameters()
        {
            try
            {
                Dictionary<string, string> parameterDictionary = new Dictionary<string, string>();
                foreach (ParameterElement parameterElement in _Config.Parameters)
                {
                    if (!parameterDictionary.ContainsKey(parameterElement.Name))
                    {
                        parameterDictionary[parameterElement.Name] = parameterElement.Value;
                    }
                }
                BaseUrl = parameterDictionary["BaseUrl"];
                User = parameterDictionary["User"];
                Password = parameterDictionary["Password"];
                DocumentRepository = parameterDictionary["DocumentRepository"];
                FolderRepository = parameterDictionary["FolderRepository"];
                CustomAspect = parameterDictionary["CustomAspect"];
                ExternalApplicationProperty = parameterDictionary["ExternalApplicationProperty"];//parameter used in an older version, think to remove it later on
                PublishInEvolveProperty = parameterDictionary["PublishInEvolveProperty"];
                SiteName = parameterDictionary["SiteName"];
            }
            catch
            {
                throw new Exception("Merci de vérifier les parametres de CMIS connection dans Web.Config");
            }
        }
    }
}