using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LIS.Logger;
using System.IO.Compression;
using System.Net;
using ZorUpdater.Model;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ZorUpdater
{
    public partial class MainForm : Form
    {
        private Applications[] applications;
        public MainForm()
        {
            InitializeComponent();
            txtRepo.Text = Properties.Settings.Default.Repo;
            txtDestination.Text = Properties.Settings.Default.AppDirectory;

            //Test

            //Applications[] applications = new Applications[2]{
            //    new Applications(){
            //        Name = "LIS Server",
            //        Versions = new Model.Version[2]
            //        {
            //            new Model.Version(){ Name = "LIS_Server05_07_2021",Path="LIS_Server05_07_2021.zip"},
            //            new Model.Version(){ Name = "LIS_Server05_07_2021",Path="LIS_Server08_07_2021.zip"}
            //        }
            //    },
            //    new Applications(){
            //        Name = "LIS Console",
            //        Versions = new Model.Version[2]
            //        {
            //            new Model.Version(){ Name = "LIS_Server05_07_2021",Path="LIS_Server05_07_2021.zip"},
            //            new Model.Version(){ Name = "LIS_Server05_07_2021",Path="LIS_Server08_07_2021.zip"}
            //        }
            //    },
            //};

            //string output = JsonConvert.SerializeObject(applications);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            string url = $"{txtRepo.Text}/repo.json";
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                wc.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

                var json = wc.DownloadString(url);

                applications = JsonConvert.DeserializeObject<Applications[]>(json);
                ddlApp.Enabled = true;
                ddlVersion.Enabled = true;
                txtDestination.Enabled = true;
                btnFolder.Enabled = true;
                btnUpdate.Enabled = true;

                BindApplication();
                Logger.LogInstance.LogInfo("Repository initiated");
            }
        }

        private void BindApplication()
        {
            var bindingSource1 = new BindingSource
            {
                DataSource = applications
            };

            ddlApp.DataSource = bindingSource1.DataSource;

            ddlApp.DisplayMember = "Name";
            ddlApp.ValueMember = "Name";
        }

        private void BtnFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtDestination.Text = fbd.SelectedPath;
                    Logger.LogInstance.LogDebug($"Destination Folder :{txtDestination.Text}");
                }
            }
        }

        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            btnUpdate.Enabled = false;
            await DownloadZip();
            if (chkBackup.Checked)
            {
                GetBackup();
            }

            PreDeploymentCleanup();
            ExtractZip();
            Deploy();

            Properties.Settings.Default.Repo = txtRepo.Text;
            Properties.Settings.Default.AppDirectory = txtDestination.Text;
            Properties.Settings.Default.Save();
        }

        private void PreDeploymentCleanup()
        {
            // Use ProcessStartInfo class

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,

                //Give the name as PreRun
                FileName = "PreRun.bat",

                //make the window Hidden
                WindowStyle = ProcessWindowStyle.Hidden
            };

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Logger.LogInstance.LogException(ex);
            }
        }

        private void Deploy()
        {
            string SolutionDirectory = $"{Environment.CurrentDirectory}\\temp";
            string TargetDirectory = txtDestination.Text;
            string config = chkConfig.Checked ? "n" : "y";
            // Use ProcessStartInfo class

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,

                //Give the name as Xcopy
                FileName = "PostRun.bat",

                //make the window Hidden
                WindowStyle = ProcessWindowStyle.Hidden,

                //Send the Source and destination as Arguments to the process
                Arguments = "\"" + TargetDirectory + "\"" + " " + "\"" + SolutionDirectory + "\"" + " " + "\"" + config + "\"" + " " + "\"" + ddlVersion.SelectedValue + "\""
            };

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Logger.LogInstance.LogException(ex);
            }
        }

        private void ExtractZip()
        {
            string tempPath = $"{Environment.CurrentDirectory}\\temp";
            string ZipFileName = $"{Environment.CurrentDirectory}\\{ddlVersion.SelectedValue}";
            try
            {
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                ZipFile.ExtractToDirectory(ZipFileName,tempPath);
            }
            catch (Exception ex)
            {
                Logger.LogInstance.LogException(ex);
            }

            Logger.LogInstance.LogInfo($"Deployment Success");
        }

        private async Task DownloadZip()
        {
            try
            {
                string url = $"{txtRepo.Text}/{ddlVersion.SelectedValue}";
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                    wc.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                    await wc.DownloadFileTaskAsync(new Uri(url), $"{ddlVersion.SelectedValue}");
                    Logger.LogInstance.LogInfo("Application version downloaded");
                }
            }
            catch (Exception ex)
            {
                Logger.LogInstance.LogException(ex);
            }
        }

        private void GetBackup()
        {
            string application = ddlApp.Text;
            string backPath = $"{Environment.CurrentDirectory}\\backup\\{application}";
            string ZipFileName = $"{backPath}\\{DateTime.Now.ToString("ddMMyyyy-HHmmssfffff")}.zip";
            try
            {
                if (!Directory.Exists(backPath)) 
                {
                    Directory.CreateDirectory(backPath);
                }
                ZipFile.CreateFromDirectory(txtDestination.Text, ZipFileName);
            }
            catch (Exception ex)
            {
                Logger.LogInstance.LogException(ex);
            }

            Logger.LogInstance.LogInfo($"Backup Success {ZipFileName}");
        }

        private void DdlApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindVersions();
        }

        private void BindVersions()
        {
            var bindingSource1 = new BindingSource();
            var selectedApp = ddlApp.SelectedItem as Applications;
            bindingSource1.DataSource = selectedApp.Versions;

            
            ddlVersion.DataSource = bindingSource1.DataSource;

            ddlVersion.DisplayMember = "Name";
            ddlVersion.ValueMember = "Path";
        }
    }
}
