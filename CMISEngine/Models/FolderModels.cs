using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMISEngine.Models
{
    //public class DocumentList
    //{
    //    public Dictionary<string, DocumentPropertyMetaData> DocumentPropertyMetaData { get; set; }
    //    public List<DocumentInfo> Documents { get; set; }
    //}

    public class CMISFolder
    {
        //public string Name { get; set; }
        //public string ObjectId { get; set; }
        public string Path { get; set; }
        //  public string Label { get; set; }
        //  public string ObjectTypeScriptName { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public List<CMISFolder> SubFolders = new List<CMISFolder>();
        public List<DocumentInfo> ContainedDocuments = new List<DocumentInfo>();

        public CMISFolder(string path)
        {
            this.Path = path;
            //this.Name = path;
            //this.ObjectId = id;
        }
    }

    //public class DocumentPropertyMetaData
    //{
    //    public string Id { get; set; }
    //    public string LocalName { get; set; }
    //    public string DisplayName { get; set; }
    //    public DocumentPropertyMetaData() { }
    //}
}