using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CMISEngine.CMIS.Configuration
{
    public class ParameterElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true, DefaultValue = "")]
        //[RegexStringValidator(@"https?\://\S+")]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        [ConfigurationProperty("cache", IsRequired = false, DefaultValue = true)]
        public bool Cache
        {
            get { return (bool)this["cache"]; }
            set { this["cache"] = value; }
        }
    }
}