using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Text;
using System.Threading.Tasks;
//using ReadMetadata;
using System.Drawing;
using System.Drawing.Imaging;

namespace WinScpSync
{
    class cFilesMan
    {
        List<FileInfo> listDetFilesSource = new List<FileInfo>();
        List<Tuple<FileInfo, BitmapMetadata>> listDataPictures = new List<Tuple<FileInfo, BitmapMetadata>>();
        //Tuple<FileInfo, BitmapMetadata> TempElem;
        int numfilesource;
        bool verbose = false;

        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

        public cFilesMan(string path, bool verboseInput)
        {
            verbose = verboseInput;
            if (Directory.Exists(path))
            {
                ProcessDirectory(path);
            }
            else
            {
                Console.WriteLine(" unable to access {0}", path);
            }
            
        }

        public List<FileInfo> GetFileList()
        {
            return listDetFilesSource;
        }

        public List<Tuple<FileInfo, BitmapMetadata>> GetFileImgList()
        {
            return listDataPictures;
        }


        bool dirperm(string path)
        {

            bool outcond = true;

            dynamic writeAllow = false;
            dynamic writeDeny = false;
            dynamic accessControlList = Directory.GetAccessControl(path);
            if (accessControlList == null)
            {
                outcond = false;
            }
            dynamic accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null)
            {
                outcond = false;
            }

            return outcond;

        }

        void ProcessDirectory(string targetDirectory)
        {
            bool permission = false;

            try
            {
                Console.WriteLine("processing directory {0}", targetDirectory);
                permission = dirperm(targetDirectory);


                if (permission == true)
                {

                    //if (!targetDirectory.Contains("$") & !targetDirectory.Contains("."))
                    if (!targetDirectory.Contains("$") )
                    {
                        string[] fileEntries = Directory.GetFiles(targetDirectory);

                        DirectoryInfo dirSource = new DirectoryInfo(targetDirectory);
                        FileInfo[] filesListSource = dirSource.GetFiles();

                        for (int scidx = 0; scidx < filesListSource.Count(); scidx++)
                        {
                            ProcessFile(filesListSource[scidx].FullName, filesListSource[scidx]);
                        }

                        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                        // Recurse into subdirectories of this directory.
                        string subdirectory = null;
                        foreach (string subdirectory_loopVariable in subdirectoryEntries)
                        {
                            subdirectory = subdirectory_loopVariable;
                            ProcessDirectory(subdirectory);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" exception {0}", ex.Message);
            }

        }

        BitmapMetadata ReadMetaData(FileInfo filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BitmapSource img = BitmapFrame.Create(fs);
                    BitmapMetadata md = (BitmapMetadata)img.Metadata;
                    if ((md!=null) && (md.DateTaken !=null))
                    {
                        string date = md.DateTaken;
                        Console.WriteLine(date);
                    }
         
                    return md;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            
        }


      

        void ProcessFile(string filename, FileInfo infofile)
        {

            if (infofile.FullName.Contains("~") == false)
            {
                string nomef = Path.GetFileNameWithoutExtension(infofile.FullName);

                if (verbose == true)
                {
                    Console.WriteLine("Processato File {0}", infofile.FullName);
                }

                string ext = Path.GetExtension(infofile.FullName);

                if (ext != ".attr")
                {
                    listDetFilesSource.Add(infofile);

                        

                    numfilesource += 1;
                }

            }

          


        }
    }
}
