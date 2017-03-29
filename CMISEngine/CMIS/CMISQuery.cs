using CMISEngine.CMIS.Configuration;
using CMISEngine.Models;
using PortCMIS;
using PortCMIS.Client;
using PortCMIS.Client.Impl;
using PortCMIS.Data;
//using DotCMIS;
//using DotCMIS.Client;
//using DotCMIS.Client.Impl;
//using DotCMIS.Data;
using System;
using System.Collections.Generic;

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
                //parameters[SessionParameter.AtomPubUrl] = baseUrl + "/cmisatom";
                parameters[SessionParameter.AtomPubUrl] = baseUrl + "/api/-default-/public/cmis/versions/1.1/atom";
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
                query = " SELECT * FROM " + CMISConnection.DocumentRepository + " d WHERE CONTAINS(d, '" + CMISConnection.ExternalApplicationProperty + "(*" + key + "*)')";
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IItemEnumerable<IQueryResult> SelectDocumentsByPath(string path, string checkPublishInEvolveFilter)
        {
            //query = "SELECT * FROM cmis:document WHERE CONTAINS('PATH:\"/app:company_home/st:sites/cm:spw/cm:documentLibrary/cm:Applicatif03/*\"')";
            //=> select sub folder documents and sub sub folder documents => query = "select * from " + CMISConnection.DocumentRepository + " d where contains(d, 'PATH:\"/app:company_home/st:sites/cm:" + CMISConnection.SiteName + "/cm:documentLibrary" + path + "//*\"')";

            IItemEnumerable<IQueryResult> qr;
            string query = null, pathFilter = null, checkEvolveFilter = null;

            pathFilter = "PATH:\"/app:company_home/st:sites/cm:" + CMISConnection.SiteName + "/cm:documentLibrary" + path + "/*\"";
            checkEvolveFilter = CMISConnection.PublishInEvolveProperty + "(" + checkPublishInEvolveFilter + ")";

            query = "select * from " + CMISConnection.DocumentRepository + " d join " + CMISConnection.CustomAspect + " s on d.cmis:objectId = s.cmis:objectId where contains(d, '" + pathFilter + "')";

            if (Convert.ToBoolean(checkPublishInEvolveFilter))
            {
                query += "and CONTAINS(s, '" + checkEvolveFilter + "')";
            }
            qr = Session.Query(query, false);
            return qr;
        }
        /// <summary>
        /// select all sub folders according to a path
        /// </summary>
        /// <param name="path">a folder path in alfresco </param>
        /// <returns></returns>
        private static IItemEnumerable<IQueryResult> SelectFoldersByPath(string path)
        {
            // query = "SELECT * FROM cmis:folder WHERE CONTAINS('PATH:\"/app:company_home/st:sites/cm:spw/cm:documentLibrary//*\"')";
            //only the sub folder, not the sub sub folder=> query = "select * from cmis:folder d where contains(d, 'PATH:\"/app:company_home/st:sites/cm:" + CMISConnection.SiteName + "/cm:documentLibrary" + path + "/*\"')";
            IItemEnumerable<IQueryResult> qr;
            string query = null;

            query = "select * from " + CMISConnection.FolderRepository + " d where contains(d, 'PATH:\"/app:company_home/st:sites/cm:" + CMISConnection.SiteName + "/cm:documentLibrary" + path + "//*\"')";
            qr = Session.Query(query, false);
            return qr;
        }

        /// <summary>
        /// the key links a document to a cm instance
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DocumentList GetDocumentsByKey(string key)
        {
            List<DocumentInfo> documents = new List<DocumentInfo>();
            Dictionary<string, string> propertiesByKey = new Dictionary<string, string>();
            Dictionary<string, DocumentPropertyMetaData> propertyMetaData = null;
            IItemEnumerable<IQueryResult> qr = CMISQuery.SelectDocumentsByKey(key);

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

        public DocumentList GetDocumentsByPath(string path, string checkPublishInEvolveFilter)
        {

            string pathString = Utilities.CMISUtilities.AddCMISNamespaceToPath(path);
            List<DocumentInfo> documents = new List<DocumentInfo>();
            Dictionary<string, string> propertiesByKey = new Dictionary<string, string>();
            Dictionary<string, DocumentPropertyMetaData> propertyMetaData = null;
            IItemEnumerable<IQueryResult> qr = CMISQuery.SelectDocumentsByPath(pathString, checkPublishInEvolveFilter);

            foreach (IQueryResult hit in qr)
            {
                DocumentInfo doc = new DocumentInfo(hit["d.cmis:objectId"].FirstValue.ToString(), hit["d.cmis:name"].FirstValue.ToString());
                doc.Properties = this.GetProperties(hit);
                doc.Path = path;


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

        public List<CMISFolder> GetFoldersByPath(string path, string checkPublishInEvolveFilter)
        {
            DocumentList documents = new DocumentList();
            List<CMISFolder> folders = new List<CMISFolder>();
            CMISFolder folder = new CMISFolder(path);

            folders.Add(folder);
            string pathString = Utilities.CMISUtilities.AddCMISNamespaceToPath(path);
            // get documents
            folder.ContainedDocuments = this.GetDocumentsByPath(path, checkPublishInEvolveFilter);
            //get sub folders
            IItemEnumerable<IQueryResult> qr = CMISQuery.SelectFoldersByPath(pathString);

            foreach (IQueryResult hit in qr)
            {
                Dictionary<string, string> FolderProperties = this.GetProperties(hit);

                string subFolderPath = FolderProperties["path"];
                string slicedPath = Utilities.CMISUtilities.sliceDocumentLibraryPath(subFolderPath);

                CMISFolder subFolder = new CMISFolder(slicedPath);

                subFolder.ContainedDocuments = this.GetDocumentsByPath(slicedPath, checkPublishInEvolveFilter);

                folders.Add(subFolder);
            }
            return folders;
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