using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace CMISEngine.Models
{
    public class CMISFolder
    {
        //public string Name { get; set; }
        public string Path { get; set; }
        public DocumentList ContainedDocuments { get; set; }

        public CMISFolder(string path)
        {
            this.Path = XmlConvert.DecodeName(path);
        }
    }
}