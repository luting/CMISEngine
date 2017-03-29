using CMISEngine.CMIS;
using CMISEngine.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml;

namespace CMISEngine.Controllers
{
    // [EnableCors(origins: "http://localhost/urbi/static/", headers: "*", methods: "*")]
    public class FoldersController : ApiController
    {
        // [JsonErrorHandler]
        public CMISFolder Get(string id)
        {
            CMISQuery query = new CMIS.CMISQuery();
            string cmisPath = id;
            CMISFolder folder = null;// = query.GetDocumentsByPath("path");
            return folder;
        }
        /// <summary>
        /// get document by path and site Name
        /// </summary>
        /// <param name="path"></param>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public List<CMISFolder> Get(string path, string checkPublishInEvolveFilter)
        {
            CMISQuery query = new CMIS.CMISQuery();
            // uri to string

            //cmis query path
            //  pathString = this.TranformToCMISPath(pathString);
            //get documents by path
            List<CMISFolder> list = query.GetFoldersByPath(path, checkPublishInEvolveFilter);
            return list;
        }
    }
}