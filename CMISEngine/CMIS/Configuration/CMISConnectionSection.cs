using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CMISEngine.CMIS.Configuration
{
    public class CMISConnectionSection : ConfigurationSection
    {
        [ConfigurationProperty("parameters", IsDefaultCollection = true)]
        public ParameterCollection Parameters
        {
            get { return (ParameterCollection)this["parameters"]; }
            set { this["parameters"] = value; }
        }
    }
}