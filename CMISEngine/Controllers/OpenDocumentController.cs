using CMISEngine.CMIS;
using CMISEngine.Models;
using DotCMIS.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace CMISEngine.Controllers
{
   // [EnableCors(origins: "http://localhost/urbi/static/", headers: "*", methods: "*")]
    public class OpenDocumentController : ApiController
    {
        //
        // GET: /OpenDocument/

        // GET api/values/5
        public HttpResponseMessage Get(string id)
        {
            if (String.IsNullOrEmpty(id))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            CMISQuery query = new CMIS.CMISQuery();

            IContentStream documentContentStream = query.GetDocumentById(id);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            if (documentContentStream.Stream != null)
            {
                response.Content = new StreamContent(documentContentStream.Stream);
                response.Headers.CacheControl = new CacheControlHeaderValue(); 
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = documentContentStream.FileName;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(documentContentStream.MimeType);
              //  response.AppendHeader("Access-Control-Allow-Origin", "*");
            }
            return response;
        }
    }
}