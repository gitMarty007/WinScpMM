using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinScpSync;

namespace WinScpMM
{
    public partial class FormWinScpSync : Form
    {
        WinScpInt winScpInt;

        public enum sync_state
        {
            IDLE,
            TRY_CONNECT,
            CONNECTED,
            CONNECTED_SCAN_DIR,
            SEARCH_DISK,
            CONNECTED_IDLE,
            DISCONNECTED,
            EXIT
        };

        string mainFolderRemote = "/media/pi";
        bool firstTime = true;
        List<string> ListResult = new List<string>();
        List<string> ListResultFiltr = new List<string>();
        sync_state sync_State = sync_state.IDLE;
        sync_state sync_State_prev = sync_state.IDLE;

        public FormWinScpSync()
        {
            InitializeComponent();

      

            backgroundWorker1.RunWorkerAsync();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            sync_State = sync_state.TRY_CONNECT;
        }


        void UpdateTextOut(string text)
        {
            if (textBoxOut.InvokeRequired == true)
            {
                textBoxOut.Invoke(new MethodInvoker(delegate () { UpdateTextOut(text); }));
                return;
            }

            textBoxOut.AppendText(text + Environment.NewLine);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            bool exit = false;

            bool condConn = false;

            string remotefolder="";

            while(exit==false)
            {
                switch (sync_State)
                {
                    case sync_state.IDLE:

                        break;

                    case sync_state.TRY_CONNECT:

                        winScpInt = new WinScpInt();

                        if (winScpInt!=null)
                        {
                            UpdateTextOut(" Connected");
                        }

                        sync_State = sync_state.CONNECTED;

                        break;

                    case sync_state.CONNECTED:

                        sync_State = sync_state.CONNECTED_SCAN_DIR;


                        break;


                    case sync_state.CONNECTED_IDLE:



                        break;

                    case sync_state.CONNECTED_SCAN_DIR:

                        if (winScpInt != null)
                        {
                            if (winScpInt.ListRemoteFolder(mainFolderRemote) == true)
                            {
                                int nelem = winScpInt.remoteListFiles.Count();

                                int idxelem = 0;
                                remotefolder = "";
                                // scan list files remote
                                while (idxelem < nelem)
                                {
                                    if (winScpInt.remoteListFiles[idxelem].IsDirectory)
                                    {
                                        remotefolder += "[DIR] ";
                                    }
                                    remotefolder += winScpInt.remoteListFiles[idxelem].Name;
                                    remotefolder += Environment.NewLine;

                                    ListResult.Add(winScpInt.remoteListFiles[idxelem].ToString());

                                    idxelem++;


                                }
                            }

                            UpdateTextOut(remotefolder);

                            if (firstTime==true)
                            {
                                sync_State = sync_state.SEARCH_DISK;
                                firstTime = false;
                            }
                            else
                            {
                                sync_State = sync_state.CONNECTED_IDLE;
                            }
                          
                        }

                            break;


                    case sync_state.SEARCH_DISK:

                        if (ListResult.Count()>0)
                        {
                            int scx = 0;

                            while(scx<ListResult.Count())
                            {
                                if (ListResult[scx].Length>2)
                                {
                                    mainFolderRemote += "/" + ListResult[scx];
                                    scx = ListResult.Count();
                                }


                                scx++;
                            }

                            sync_State = sync_state.CONNECTED_SCAN_DIR;
                        }

                        break;

                }

                if (sync_State != sync_State_prev)
                {
                    sync_State_prev = sync_state;
                }

            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {

        }
    }
}
