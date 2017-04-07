using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace CMISEngine.Models
{
    public class DocumentList
    {
        private List<DocumentInfo> document = new List<DocumentInfo>();
        public Dictionary<string, DocumentPropertyMetaData> DocumentPropertyMetaData { get; set; }
        public List<DocumentInfo> Documents { get { return this.document; } set { this.document = value; } }
    }

    public class DocumentInfo
    {
        private string path;
        public string Name { get; set; }
        public string ObjectId { get; set; }
        public string Path
        {
            get
            {
                return this.path; 
            }
            set
            {
               this.path = XmlConvert.DecodeName(value);
            }
        }
        public string FolderName { get; set; }

        //  public string Label { get; set; }
        //  public string ObjectTypeScriptName { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        
        //
        public DocumentInfo(string id, string name)
        {
            this.Name = name;
            this.ObjectId = id;
        }
    }
    public class DocumentPropertyMetaData
    {
        public string Id { get; set; }
        public string LocalName { get; set; }
        public string DisplayName { get; set; }
        public DocumentPropertyMetaData() { }
    }
}