﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Diagnostics;

namespace Condenser
{
    public partial class Main : Form
    {
        public string source = @"C:\Condenser\Source";
        public static string logpath = (@"C:\Condenser" + @"\logs\");
        public string output = @"C:\Condenser\Output";
        public string cache = @"\appcache\httpcache\";
        public string config = @"\config\";
        public string csvdir = @"\files_csv\";
        public string csvname = "file_list";
        public string carveoutputdir = @"\carve_results\";
        public LogWrite LW = new LogWrite(logpath, "log.txt");

        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }

        public string Output
        {
            get
            {
                return output;
            }
            set
            {
                output = value;
            }
        }

        public Main()
        {
            InitializeComponent();
            RefreshManager();

        }

        private void Main_Shown(object sender, EventArgs e)
        {
            LogWrite.WriteLineNoDate("=== Condenser: A Steam Web Artefact and Metadata Tool ===");
            NewSession();
        }

        private void Form1_Load(object sender, EventArgs e) 
        {
            


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public void RefreshManager()
        {
            System.Timers.Timer updateinterval = new System.Timers.Timer(50);
            updateinterval.Elapsed += new ElapsedEventHandler(FixedUpdate);
        }

        public void Update()
        {
            this.Refresh();
        }

        public void FixedUpdate(object sender, ElapsedEventArgs e)
        {
            this.Refresh();
        }

        private void NewSession()
        {
            steamDirBrowser.ShowDialog();
            Source = steamDirBrowser.SelectedPath;
            LogWrite.WriteLine("Session source: " + Source);
            
            outputBrowser.SelectedPath = System.Environment.CurrentDirectory;
            outputBrowser.ShowDialog();
            Output = outputBrowser.SelectedPath;
            LogWrite.WriteLine("Session output: " + Output);
        }
        
        private void outputToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusOutputLabel.Text = "CSV output starting. This may take some time...";
            Stopwatch time = new Stopwatch();            
            
            Thread csvthread = new Thread(CreateCSV);            

            time.Start();
            csvthread.Start();

            //Run while loop in another thread?
            while (csvthread.ThreadState == System.Threading.ThreadState.Running)
            {
                if (csvthread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    time.Stop();

                    break;
                }
            }

            long timetaken = time.ElapsedMilliseconds;
            statusOutputLabel.Text = "Output to csv complete! Took " + timetaken.ToString() + " milliseconds";
            
        }

        public void CreateCSV()
        {
            string csvpath = Output + csvdir;            


            FileOperations FO = new FileOperations(source, output, config, cache);            
            List<string> files = FO.GetAllFiles();
            CSVHelper csvh = new CSVHelper(files);
            
            List<string[]> fileinfo = csvh.GetFileListData();
            CSVWriter csv = new CSVWriter(fileinfo, csvpath, csvname);
            Debug.WriteLine("Created objects, about to write.");
            
            csv.Write();

            
            Debug.WriteLine("Written.");
        }

        private void newSession_Click_1(object sender, EventArgs e)
        {
            NewSession();  
        }

        private void discoverWebBrowserDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ListViewItem> items = PopulateList();
            for (int i = 0; i < items.Count; i++)
            {
                CompleteFileListView.Items.Add(items[i]);
            }
            for (int i = 0; i < CompleteFileListView.Columns.Count; i++)
            {
            CompleteFileListView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            //PopulateList();
        }

        public List<ListViewItem> PopulateList()
        { 
            FileOperations FO = new FileOperations(source, output, cache, config);
            List<string> steamFiles = FO.GetAllFiles();
            
            int total = steamFiles.Count;
            List<ListViewItem> items = new List<ListViewItem>();

            for (int i = 0; i < steamFiles.Count; i++)
            {
                SteamFileInfo FI = new SteamFileInfo(steamFiles[i]);

                string name = FI.GetFileName();
                string path = FI.GetFilePath();
                string size = (FI.GetFileSize() + " bytes");
                string accessdate = FI.GetAccessDate();
                string creationdate = FI.GetCreationDate();
                string modifieddate = FI.GetModifiedDate();

                string md5 = FI.GetMD5Hash();
                string sha1 = FI.GetSHA1Hash();

                ListViewItem item = new ListViewItem(new[] { name, path, size, accessdate, creationdate, modifieddate, md5, sha1 });
                items.Add(item);
                Debug.WriteLine("Added file number " + i + " of " + steamFiles.Count); 
            }

            return items;
        }



        private void CompleteFileListView_SelectedIndexChanged(object sender, EventArgs e)
        { 
        }



        public Util SetupUtils(string source, string output)
        {
            Util util = new Util();
            util.SteamDirectory = source;
            util.OutputDirectory = output;            

            return util;
        }
        

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {            
            MessageBox.Show(e.Context.ToString());
            e.Cancel = true;
            
        }

        private void loadCookiesTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SQLiteReader reader = new SQLiteReader(@"C:\Program Files (x86)\Steam\config\htmlcache\Cookies;");

            DataTable dbout = reader.GetConnection();

            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = dbout;
            dataGridView1.ReadOnly = true;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {   //We don't want to output any byte arrays to the data grid.
                if (dataGridView1.Columns[i].ValueType.ToString() == "System.Byte[]")
                {
                    dataGridView1.Columns[i].Visible = false;
                }
            }
        }

        private void findSteamDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        
           
        }

        private void copyWebBrowserDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //FileOperations FO = new FileOperations(@"C:\Condenser\Source\", @"C:\Condenser\");
            //Run File Operations on a new thread to allow the user to use the UI during the process.



            
            
            //Thread carveThread = new Thread(new ThreadStart(FO.CarveIdentify));

            //carveThread.Start();


        }

        private void fileCopyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Debug.WriteLine("starting...");
            FileOperations FO = new FileOperations(source, output, cache, config);
            FO.FileCopy();
            //fileCopyWorker.ReportProgress(FO.listprogress);
        }


        private string GetFileSelected()
        {
            
            
            /*if (CompleteFileListView.SelectedIndices. == 0)
            {
                return null;
            }*/
            int selectedindex = CompleteFileListView.SelectedIndices[0];
            string filepath = CompleteFileListView.Items[selectedindex].SubItems[1].Text + @"\" + CompleteFileListView.Items[selectedindex].Text;


            MessageBox.Show(filepath);

            return filepath;

                
          
        }

      

        private void carveSingleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileCarver FC = new FileCarver();
            string file = GetFileSelected();
            FC.Carve(FC.GetBytes(file), 0);         

        }

        private void carveFolderContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            carverFolderBrowser.SelectedPath = Source;
            carverFolderBrowser.ShowDialog();

            IO io = new IO();
            io.Show();

            io.IOtext.AppendText("Getting files...\n");
            string[] files = Directory.GetFiles(carverFolderBrowser.SelectedPath, "*", SearchOption.AllDirectories);
            io.IOtext.AppendText("Got files, now carving them...\n");

            string _carveoutputdir = Output + carveoutputdir;

            FileCarver FC = new FileCarver(files, _carveoutputdir);
            Thread carveThread = new Thread(FC.CarveManager);
            Stopwatch time = new Stopwatch();

            time.Start();
            carveThread.Start();
            while (carveThread.ThreadState == System.Threading.ThreadState.Running)
            {
                if (carveThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    time.Stop();

                    break;
                }
            }

            long timetaken = time.ElapsedMilliseconds;


            io.IOtext.AppendText("Files carved in " + timetaken.ToString() + " milliseconds.\n");

        }

        private void imageWebBrowserDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileOperations FO = new FileOperations(source, output, cache, config);
            Thread copyThread = new Thread(new ThreadStart(FO.FileCopy));
            copyThread.Start();
            while (copyThread.ThreadState == System.Threading.ThreadState.Running)
            {
                statusOutputLabel.Text = "Copying files!";
                Update();
                if (copyThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    statusOutputLabel.Text = "Finished copying all files.!";
                }
            }
            
            
        }
        

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)       {       }

        private void fileSizeLabel_Click(object sender, EventArgs e)       {       }

        

        private void fileModifiedLabel_Click(object sender, EventArgs e)       {       }

        public void FileListBox_SelectedIndexChanged(object sender, EventArgs e)       {       }

        private void fileCarveToolStripMenuItem_Click(object sender, EventArgs e)       {        }

        private void steamDirBrowser_HelpRequest(object sender, EventArgs e)       {       }

        private void ProgressBar_Click(object sender, EventArgs e)       {       }


    }
}
