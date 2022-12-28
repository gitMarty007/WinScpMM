using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace WinScpSync
{
    class WinScpInt
    {
        SessionOptions sessionOptions;

        public List<string> ResultFileList = new List<string>();
        public List<RemoteFileInfo> remoteListFiles = new List<RemoteFileInfo>();

        public string commandResult;
        public WinScpInt()
        {

            // Setup session options
            sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = "raspberrypi",
                UserName = "pi",
                Password = "Pegaso75",
                SshHostKeyFingerprint = "ssh-rsa 2048 38:62:45:77:63:63:8f:4a:7a:5e:9e:49:b6:71:75:b7"

            };

           

        }

        public bool DeleteRemoteFile(string remoteFileName)
        {
            bool ret = false;

            commandResult = "";
            try
            {
                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    RemovalOperationResult removalResult;
                    removalResult =
                        session.RemoveFiles(remoteFileName);

                    // Throw on any error
                    if (removalResult.IsSuccess)
                    {
                        ret = true;
                    }
                }

              
            }
            catch (Exception ex)
            {
                //MessageBox.Show(string.Format("{0}", ex.Message));
                ret = false;
            }

            return ret;
        }

        public bool UploadFile(string resultFile, string outputPathRemote)
        {
            bool ret = false;

            try
            {
                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    transferResult =
                        session.PutFiles(resultFile, outputPathRemote, false, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                    }
                }

                ret = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(string.Format("{0}", ex.Message));
                ret = false;
            }

            return ret;

        }

        public bool DownloadDirfiles(string outputPathRemote, string localfolder)
        {
            bool esito = false;
            try
            {
                using (Session session = new Session())
                {
                    ResultFileList.Clear();
                    // Connect
                    session.Open(sessionOptions);

                    // Download files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    transferResult =
                        session.GetFiles(outputPathRemote + "/* ", localfolder, false, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        //UpdateTextOut(string.Format("Download of {0} succeeded", transfer.FileName));
                        ResultFileList.Add(transfer.FileName);
                    }

                    esito = true;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(string.Format("Exception {0}", ex));
                esito = false;
            }

            return esito;
        }

        public bool CercaFileDirectory(string path,string mode)
        {
            int index = 0;
            bool found = false;
            while (index < remoteListFiles.Count())
            {
                if (mode=="dir")
                {
                    if (remoteListFiles[index].IsDirectory == true)
                    {
                        if (remoteListFiles[index].FullName.Contains(path) == true)
                        {
                            found = true;
                        }
                    }
                }
                else
                {
                    if (mode=="file")
                    {
                        if (remoteListFiles[index].IsDirectory == false)
                        {
                            if (remoteListFiles[index].FullName.Contains(path) == true)
                            {
                                found = true;
                            }
                        }
                    }
                }
        

                index++;
            }

            return (found);
        }

        //public bool UploadFile(string remote_path,string LocalFile)
        //{
        //    bool ret_value = false;
        //    //using (Session session = new Session())
        //    //{
        //    //    // Connect
        //    //    session.Open(sessionOptions);
        //    //    RemoteDirectoryInfo targetServerDir = session.ListDirectory(remote_path);
        //    //    //Console.WriteLine(targetServerDir.ToString());
        //    //    // Upload files
        //    //    TransferOptions transferOptions = new TransferOptions();
        //    //    transferOptions.TransferMode = TransferMode.Binary;

        //    //    TransferOperationResult transferResult;
        //    //    //transferResult = session.PutFiles(@"d:\toupload\*", "/home/user/", false, transferOptions);
        //    //    transferResult = session.PutFiles(folderToUpload, "/opt/mean/public/VolumeControlUtility/CurrentVersion/",
        //    //        false, transferOptions);

        //    //    // Throw on any error
        //    //    transferResult.Check();

        //    //    // Print results
        //    //    foreach (TransferEventArgs transfer in transferResult.Transfers)
        //    //    {
        //    //        Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
        //    //    }
        //    //}
        //    try
        //    {



        //        using (Session session = new Session())
        //        {
        //            // Connect
        //            session.Open(sessionOptions);

        //            // Upload files
        //            TransferOptions transferOptions = new TransferOptions();
        //            transferOptions.TransferMode = TransferMode.Binary;

        //            TransferOperationResult transferResult;
        //            transferResult = session.PutFiles(LocalFile, remote_path, false, transferOptions);

        //            // Throw on any error
        //            transferResult.Check();

        //            // Print results
        //            foreach (TransferEventArgs transfer in transferResult.Transfers)
        //            {
        //                Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
        //            }

        //            ret_value = true;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        ret_value = false;
        //    }

        //    return ret_value;
        //}

        public bool DownloadFile(string remoteFileName,string localpath)
        {
            bool result = false;
            try
            {
                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Download files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    transferResult =
                        session.GetFiles(remoteFileName, localpath, false, transferOptions);



                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        Console.WriteLine("Download of {0} succeeded", transfer.FileName);
                    }

                    result = true;
                }
            }
            catch(Exception ex)
            {
                result = false;
            }

            return result;
        }

        public bool DownloadMoreRecentFile(string remotePath, string localPath)
        {
            bool retcond = false;
            try
            {
                

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    //const string remotePath = "/home/user/";
                    //const string localPath = @"C:\downloaded\";

                    // Get list of files in the directory
                    RemoteDirectoryInfo directoryInfo = session.ListDirectory(remotePath);

                    // Select the most recent file
                    RemoteFileInfo latest =
                        directoryInfo.Files
                            .Where(file => !file.IsDirectory)
                            .OrderByDescending(file => file.LastWriteTime)
                            .FirstOrDefault();

                    // Any file at all?
                    if (latest == null)
                    {
                        throw new Exception("No file found");
                    }

                    // Download the selected file
                    session.GetFiles(
                        RemotePath.EscapeFileMask(latest.FullName), localPath).Check();

                    retcond = true;
                }

            }
            catch(Exception ex)
            {
                retcond = false;
            }

            return retcond;
       }

        public bool WriteListRemoteToFile(string filename)
        {
            bool result;

            result = true;

            using (StreamWriter outputFile = new StreamWriter(filename))
            {
     
                int index = 0;

                while (index < remoteListFiles.Count())
                {
                    outputFile.WriteLine(remoteListFiles[index].FullName);

                    index++;
                }
            }

            return result;
        }

        public bool ListRemoteFolder(string remoteFolder)
        {
            bool connection = false;
            remoteListFiles.Clear();

            {
                try
                {


                    // list dir
                    using (Session session = new Session())
                    {
                        // Connect
                        session.Open(sessionOptions);

                        RemoteDirectoryInfo directory =
                            session.ListDirectory(remoteFolder); //

                        foreach (RemoteFileInfo fileInfo in directory.Files)
                        {

                            remoteListFiles.Add(fileInfo);
                        }

                       // DataGridFill();

                        connection = true;
                    }





                }
                catch (Exception ex)
                {
                  
                    connection = false;
                }
            }

            return connection;
        }

        internal bool CreteDirRemote(string myname)
        {
            bool cond = false;
            try
            {


                // list dir
                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    session.CreateDirectory(myname);

                    cond = true;
                }
            }
            catch (Exception ex)
            {
                cond = false;
            }

            return cond;
        }
    }
}
