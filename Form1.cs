using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using FormatVolumeMethod1;
using FormatVolumeMethod2;
using ExtractISO;
using System.Threading;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;


namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Imstall_Click(object sender, EventArgs e)
        {
            using (Process exeProcess = Process.Start(@"rfs.exe")) ;
            System.Threading.Thread.Sleep(10);
            //using (Process exeProcess = Process.Start(@"rufus.exe")) ;
            //using (Process exeProcess = Process.Start(@"iso.exe")) ;
            System.Threading.Thread.Sleep(1);
            this.saving.Visible = false;
            this.label1.Visible = false;
            this.Install.Visible = false;
            this.siso.Visible = false;
            this.killpgrm.Visible = false;
            this.usblabel.Visible = true;
            this.usbpick.Visible = true;
            this.writeusb.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Process exeProcess = Process.Start(@"iso.exe")) ;
            this.saving.Visible = true;
            System.Threading.Thread.Sleep(3);
            this.saving.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            usbpick.DataSource =
                           DriveInfo.GetDrives()
                           .Where(x => x.DriveType == DriveType.Removable)
                           .ToList();
        }

        private void saving_Click(object sender, EventArgs e)
        {

        }

        public int copyfin;

        private void usbpick_SelectedIndexChanged(object sender, EventArgs e)
        {
            string usbdrive = usbpick.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine(usbdrive);


        }

        private void writeusb_Click(object sender, EventArgs e)
        {
            string usbdrive = usbpick.SelectedItem.ToString();
            System.Windows.Forms.MessageBox.Show("Your USB drive will be erased.");
            System.Threading.Thread.Sleep(5);
            string drive = StripString.Stripstring.RemoveSpecialCharacters(usbdrive);
            char chardrive = char.Parse(drive);
            System.Diagnostics.Debug.WriteLine(chardrive);
            this.Formatting.Visible = true;
            FormatVolumeMethod1.FormatLeDiskMethod1.FormatDriveMethod1(chardrive, "PearOS", "EXFAT", true, false);
            File.WriteAllText("drive.txt", drive);
            System.Threading.Thread.Sleep(100);
            this.Formatting.Visible = false;
            this.Formatting.Text = "Saving...";
            savedpercent.Visible = true;
            ThreadStart myThreadStart = new ThreadStart(MyThreadRoutine);
            Thread myThread = new Thread(myThreadStart);
            myThread.Start();
            //
            startDownload();

        }



        public void CopyFileWithProgress(string source, string destination)
        {
            var webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(filecopyfin);
            webClient.DownloadFile(new Uri(source), destination);
        }
        public void filecopyfin(object sender, AsyncCompletedEventArgs e)
        {
            Application.Exit();
        }
        private void writeprogress_Click(object sender, EventArgs e)
        {

        }

        private void savinglbl_Click(object sender, EventArgs e)
        {

        }
        private void startCopy()
        {

            string usbdrive = usbpick.SelectedItem.ToString();
            string drive = StripString.Stripstring.RemoveSpecialCharacters(usbdrive);
            char chardrive = char.Parse(drive);
            CopyFileWithProgress(@"Temp\\PearOS\\", drive + ":\\");
        }
        public void startDownload()
        {

            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadFileAsync(new Uri("https://mirrors.advancedhosters.com/linuxmint/isos/stable/21/linuxmint-21-cinnamon-64bit.iso"), @"Temp\PearOS.iso");
        }
        public void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            int intamountsaved = int.Parse(Math.Truncate(percentage).ToString());
            string stramountsaved = intamountsaved.ToString();
            savedpercent.Text = "Saved " + bytesIn + "  bytes out of " + totalBytes + " total bytes.";
        }
        public void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            copyfin = 1;
            string usbdrive = usbpick.SelectedItem.ToString();
            string drive = StripString.Stripstring.RemoveSpecialCharacters(usbdrive);
            savedpercent.Visible = false;
            Extract_ISO.ExtractISO(@"Temp\PearOS.iso", drive + ":\\");
            //startCopy();
        }
        private void MyThreadRoutine()
        {
            Invoke((MethodInvoker)delegate { ProgressGif.Visible = true; });
            if (copyfin == 1)
            {
                System.Diagnostics.Debug.WriteLine("copyfin");
                Invoke((MethodInvoker)delegate { ProgressGif.Visible = false; });
            }
        }
    }
}
