using CMISEngine.CMIS;
using CMISEngine.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace CMISEngine.Controllers
{
    // [EnableCors(origins: "http://localhost/urbi/static/", headers: "*", methods: "*")]
    public class DocumentsController : ApiController
    {
        // [JsonErrorHandler]
        public DocumentList Get(string id)
        {
            CMISQuery query = new CMIS.CMISQuery();
            DocumentList list = query.GetDocumentsByKey(id);
            return list;
        }

        /// <summary>
        /// get document by path and site Name
        /// </summary>
        /// <param name="path"></param>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public DocumentList Get(string path, string checkPublishInEvolveFilter)
        {
            DocumentList allDocuments = new DocumentList();
            CMISQuery query = new CMIS.CMISQuery();
            // string pathString = Utilities.CMISUtilities.AddCMISNamespaceToPath(path.ToString());
            //get documents by path
            //  DocumentList list = query.GetDocumentsByPath(pathString, onlyEvolveAvailable);

            List<CMISFolder> list = query.GetFoldersByPath(path, checkPublishInEvolveFilter);

            foreach (CMISFolder folder in list)
            {
                if (folder.ContainedDocuments == null) { continue; }
                if (allDocuments.DocumentPropertyMetaData == null && folder.ContainedDocuments.DocumentPropertyMetaData != null)
                {
                    allDocuments.DocumentPropertyMetaData = folder.ContainedDocuments.DocumentPropertyMetaData;
                }
                if (folder.ContainedDocuments.Documents.Count != 0)
                {
                    allDocuments.Documents.AddRange(folder.ContainedDocuments.Documents);
                }
            }
            return allDocuments;
        }

        /// <summary>
        /// the filter for path to be used in CMIS query should XML encoded string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        //private string TranformToCMISPath(string path)
        //{
        //    string formattedCMISPath = null;
        //    string[] foldersName = Regex.Split(path, "/");

        //    foreach (string folderName in foldersName)
        //    {
        //        if (folderName != "")
        //        {
        //            formattedCMISPath = formattedCMISPath + "/cm:" + XmlConvert.EncodeName(folderName);
        //        }
        //    }
        //    return formattedCMISPath;
        //}

        /// <summary>
        /// Get All Documents
        /// </summary>
        /// <returns></returns>
        public DocumentList Get()
        {
            return this.Get("");
            //CMISQuery query = new CMIS.CMISQuery();
            //DocumentList list = query.GetDocumentsByKey("");
            //return list;
        }
    }
}