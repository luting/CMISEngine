using CMISEngine.CMIS;
using CMISEngine.Models;
using System;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml;

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
        public DocumentList GetFilter(string path, string siteName)
        {
            CMISQuery query = new CMIS.CMISQuery();

            // uri to string
            string pathString = path.ToString();
            //cmis query path
            pathString = this.TranformToCMISPath(pathString);
            //get documents by path
            DocumentList list = query.GetDocumentsByPath(pathString);
            return list;
        }

        /// <summary>
        /// the filter for path to be used in CMIS query should XML encoded string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string TranformToCMISPath(string path)
        {
            string formattedCMISPath = null;
            string[] foldersName = Regex.Split(path, "/");

            foreach (string folderName in foldersName)
            {
                if (folderName != "")
                {
                    formattedCMISPath = formattedCMISPath + "/cm:" + XmlConvert.EncodeName(folderName);
                }
            }
            return formattedCMISPath;
        }

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