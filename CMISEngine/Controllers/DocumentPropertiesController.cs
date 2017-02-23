using CMISEngine.CMIS;
using CMISEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CMISEngine.Controllers
{
    public class DocumentPropertiesController : ApiController
    {
        public Dictionary<string, DocumentPropertyMetaData> Get()
        {
            CMISQuery query;

            query = new CMIS.CMISQuery();
            return query.GetDocumentPropertyMetaData();
        }
    }
}
