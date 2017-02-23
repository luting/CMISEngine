using CMISEngine.CMIS.Configuration;
using CMISEngine.Errors;
using CMISEngine.Models;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using DotCMIS.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace CMISEngine.CMIS
{
    public class CMISQuery
    {
        private static ISession Session;
        //private string _documentRepository;
        //private string _externalApplicationProperty;
        public CMISQuery()
        {
            this.Connection();
        }

        public void Connection()
        {
            try
            {
                CMISConnection.GetParameters();
                String baseUrl = CMISConnection.BaseUrl as String; //"http://127.0.0.1:8080/alfresco";

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters[SessionParameter.BindingType] = BindingType.AtomPub;
                //  parameters[SessionParameter.AtomPubUrl] = baseUrl + "/cmisatom";
                parameters[SessionParameter.AtomPubUrl] = baseUrl + "/api/-default-/public/cmis/versions/1.0/atom";
                parameters[SessionParameter.User] = CMISConnection.User;//"admin";
                parameters[SessionParameter.Password] = CMISConnection.Password;//"admin";
                //this._documentRepository = CMISConnection.DocumentRepository;
                //this._externalApplicationProperty = CMISConnection.ExternalApplicationProperty;

                SessionFactory factory = SessionFactory.NewInstance();
                // Connexion à Alfresco (on se connecte toujours au 1er Repository : il n'y en a qu'un seul)
                Session = factory.GetRepositories(parameters)[0].CreateSession();
            }
            catch
            {

                throw new Exception("Veuillez vérifier la connection aux entrepôts Alfresco: " + CMISConnection.BaseUrl + ". Impossible de se connecter à Alfresco.");
            }
        }


        /// <summary>
        /// Select Document By Key
        /// </summary>
        /// <param name="key">a key can store information to link a document to an application</param>
        /// <returns></returns>
        private static IItemEnumerable<IQueryResult> SelectDocumentsByKey(string key)
        {
            // List<DocumentInfo> documents = new List<DocumentInfo>();
            //by document name
            //string query = "SELECT * FROM cmis:document d WHERE CONTAINS(d, 'cmis:name:(" + key + ")')";

            //by tag
            //string query = " SELECT * FROM cmis:document d WHERE CONTAINS(d, 'TAG:(" + key + ")')";


            IItemEnumerable<IQueryResult> qr;

            string query = null;

            if (!String.IsNullOrEmpty(key))
            {
                 //query = "SELECT * FROM cmis:folder WHERE CONTAINS('PATH:\"/app:company_home/st:sites/cm:spw/cm:documentLibrary//*\"')";
//                query = " SELECT * FROM " + CMISConnection.DocumentRepository + " d WHERE CONTAINS(d, 'objectId:(337ae234-5ccf-4c66-8276-ae72c3363212)')";
                 query = " SELECT * FROM " + CMISConnection.DocumentRepository + " d WHERE CONTAINS(d, '" + CMISConnection.ExternalApplicationProperty + "(*" + key + "*)')";

                //query = " SELECT * FROM " + CMISConnection.DocumentRepository + " d WHERE " + CMISConnection.ExternalApplicationProperty + " LIKE '%" + key + "%'";
                //query = " SELECT * FROM cmis:document WHERE TAG LIKE '%aad-1%'";
            }
            else
            {
                query = "SELECT * FROM " + CMISConnection.DocumentRepository + " d";
            }
            try
            {
                qr = Session.Query(query, false);
            }
            catch
            {
                throw new Exception("Impossible de sélectionner les documents liés à " + key);
            }
            return qr;
        }




        private static IItemEnumerable<IQueryResult> SelectFoldersByPath(string path)
        {
            IItemEnumerable<IQueryResult> qr;
            string query = null;


            // query = " SELECT * FROM " + CMISConnection.DocumentRepository + " d WHERE CONTAINS(d, '" + CMISConnection.ExternalApplicationProperty + "(*" + key + "*)')";
            query = "SELECT * FROM cmis:folder WHERE CONTAINS('PATH:\"/app:company_home/st:sites/cm:spw/cm:documentLibrary//*\"')";

            qr = Session.Query(query, false);
            return qr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IItemEnumerable<IQueryResult> SelectDocumentsByPath(string path)
        {
            IItemEnumerable<IQueryResult> qr;
            string query = null;

            //query = "SELECT * FROM cmis:document WHERE CONTAINS('PATH:\"/app:company_home/st:sites/cm:spw/cm:documentLibrary/cm:Applicatif03/*\"')";
            //query = "SELECT * FROM cmis:document WHERE CONTAINS('PATH:\"/app:company_home/st:sites/cm:spw/cm:documentLibrary//*\"')";
            // query = "select * from " + CMISConnection.DocumentRepository + " where contains('PATH:\"/app:company_home/st:sites/cm:spw/cm:documentLibrary/cm:" + fileName1 + "/cm:Applications/cm:" + fileName2 + "//*\"')";
            query = "select * from " + CMISConnection.DocumentRepository + " where contains('PATH:\"/app:company_home/st:sites/cm:" + CMISConnection.SiteName + "/cm:documentLibrary" + path + "//*\"')";
            qr = Session.Query(query, false);
            return qr;
        }

        public DocumentList GetDocumentsByKey(string key)
        {
            List<DocumentInfo> documents = new List<DocumentInfo>();

            Dictionary<string, string> propertiesByKey = new Dictionary<string, string>();

            IItemEnumerable<IQueryResult> qr = CMISQuery.SelectDocumentsByKey(key);
            Dictionary<string, DocumentPropertyMetaData> propertyMetaData = null;

            foreach (IQueryResult hit in qr)
            {
                DocumentInfo doc = new DocumentInfo(hit["d.cmis:objectId"].FirstValue.ToString(), hit["d.cmis:name"].FirstValue.ToString());
                doc.Properties = this.GetProperties(hit);

                documents.Add(doc);
                if (propertyMetaData == null)
                {
                    propertyMetaData = this.GetDocumentPropertyMetaData(hit);
                }
            }

            return new DocumentList
             {
                 Documents = documents,
                 DocumentPropertyMetaData = propertyMetaData
             };
        }


        public CMISFolder GetFoldersByPath(string path)
        {
            CMISFolder folder = new CMISFolder(path);
            IItemEnumerable<IQueryResult> qr = CMISQuery.SelectFoldersByPath(path);
            foreach (IQueryResult hit in qr)
            {
                Dictionary<string, string> FolderProperties = this.GetProperties(hit);

                string pathOfSubFolder = FolderProperties["path"];
                CMISFolder subFolder = new CMISFolder(pathOfSubFolder);
                folder.SubFolders.Add(subFolder);
                //hit.Properties
            }

            return folder;
        }

        public DocumentList GetDocumentsByPath(string path)
        {
            List<DocumentInfo> documents = new List<DocumentInfo>();

            Dictionary<string, string> propertiesByKey = new Dictionary<string, string>();


            Dictionary<string, DocumentPropertyMetaData> propertyMetaData = null;


            IItemEnumerable<IQueryResult> qr = CMISQuery.SelectDocumentsByPath(path);

            //foreach (IQueryResult hit in qr2)
            //{
            //    Dictionary<string, string> FolderProperties = this.GetProperties(hit);

            //    DocumentInfo doc = new DocumentInfo(hit["cmis:objectId"].FirstValue.ToString(), hit["cmis:name"].FirstValue.ToString());
            //    doc.Properties = this.GetProperties(hit);
            //    folder.ContainedDocuments.Add(doc);
            //}

            foreach (IQueryResult hit in qr)
            {
                DocumentInfo doc = new DocumentInfo(hit["cmis:objectId"].FirstValue.ToString(), hit["cmis:name"].FirstValue.ToString());
                doc.Properties = this.GetProperties(hit);

                documents.Add(doc);
                if (propertyMetaData == null)
                {
                    propertyMetaData = this.GetDocumentPropertyMetaData(hit);
                }
            }

            return new DocumentList
            {
                Documents = documents,
                DocumentPropertyMetaData = propertyMetaData
            };
        }

        private Dictionary<string, string> GetProperties(IQueryResult hit)
        {
            Dictionary<string, string> propertiesByKey = new Dictionary<string, string>();
            foreach (var prop in hit.Properties)
            {
                string value = null;

                if (prop.FirstValue != null)
                {
                    value = prop.FirstValue.ToString();
                }
                propertiesByKey[prop.LocalName] = value;
            }
            return propertiesByKey;
        }

        private Dictionary<string, DocumentPropertyMetaData> GetDocumentPropertyMetaData(IQueryResult hit)
        {
            Dictionary<string, DocumentPropertyMetaData> metaDataByKey = new Dictionary<string, DocumentPropertyMetaData>();
            foreach (var prop in hit.Properties)
            {
                string value = null;

                if (prop.FirstValue != null)
                {
                    value = prop.FirstValue.ToString();
                }
                DocumentPropertyMetaData metaData = new DocumentPropertyMetaData
                {
                    DisplayName = prop.DisplayName,
                    Id = prop.Id.ToString(),
                    LocalName = prop.LocalName
                };

                metaDataByKey[prop.LocalName] = metaData;
            }
            return metaDataByKey;
        }


        public Dictionary<string, DocumentPropertyMetaData> GetDocumentPropertyMetaData()
        {
            Dictionary<string, DocumentPropertyMetaData> propertyMetaData = null;
            IItemEnumerable<IQueryResult> qr = CMISQuery.SelectDocumentsByKey("");
            foreach (IQueryResult hit in qr)
            {
                if (propertyMetaData == null)
                {
                    propertyMetaData = this.GetDocumentPropertyMetaData(hit);
                }
                else
                {
                    break;
                }
            }
            return propertyMetaData;
        }

        /// <summary>
        /// getDocumentContentStream
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IContentStream GetDocumentById(string id)
        {
            IObjectId objectId = Session.CreateObjectId(id);
            IDocument doc = Session.GetObject(id) as IDocument;
            IContentStream contentStream = doc.GetContentStream();
            return contentStream;
        }
    }
}