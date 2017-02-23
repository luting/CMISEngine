using CMISEngine.CMIS;
using CMISEngine.Models;
using System.Web.Http;
using System.Web.Mvc;


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
            CMISFolder folder=null;// = query.GetDocumentsByPath("path");


           return folder;
        }



        public string Afficher(int jour, int mois, int annee)
        {
            return "Il fait soleil le " + jour + "/" + mois + "/" + annee;
        }


        // [JsonErrorHandler]
        public ActionResult path(string path)
        {

            string cmisPath = path;
            return null;
          //  return cmisPath;
           // var input = Microsoft.SqlServer.Server.HtmlEncode(name);
         //   return Content(input);


        }


        //public DocumentList Get()
        //{
        //    return this.Get("");
        //    //CMISQuery query = new CMIS.CMISQuery();

        //    //DocumentList list = query.GetDocumentsByKey("");
        //    //return list;
        //}
    }
}