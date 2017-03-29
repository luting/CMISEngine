using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace CMISEngine.Utilities
{
    public static class CMISUtilities
    {
        public static string AddCMISNamespaceToPath(string path)
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

        public static string sliceDocumentLibraryPath(string path)
        {
            int x = path.IndexOf("documentLibrary") + "documentLibrary".Length;
            string slicedPath = path.Substring(x, path.Length - x);
            return slicedPath;
        }
    }
}