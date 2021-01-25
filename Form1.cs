using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Easy2FA
{
    public partial class MainForm : Form
    {
        string twoFactorsId = string.Empty;
        short currentStep = 0;
        public MainForm()
        {
            InitializeComponent();
            buttonRequest.Text = "Request token";
            statusLabel.Text = "";
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Filling data from configuration file
            textUser.Text = ConfigurationManager.AppSettings["user"];
            textPassword.Text = ConfigurationManager.AppSettings["password"];
            checkBoxRemember.Checked = !string.IsNullOrWhiteSpace(textUser.Text) || !string.IsNullOrWhiteSpace(textPassword.Text);
        }

        private void requestToken(object sender, EventArgs e)
        {
            // Cleaning
            statusLabel.Text = string.Empty;            

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["uriToken"]);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("X-Forwarded-Host", "ci-portal-tiempodev.azurewebsites.net");
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"username\":\"" + textUser.Text + "\"," +
                              "\"password\":\"" + textPassword.Text + "\"}";

                streamWriter.Write(json);
            }
            try
            {
                statusLabel.Text = "Requesting for token, please wait...";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var response = streamReader.ReadToEnd();
                    var objResponse = JsonConvert.DeserializeObject<StepOneResponse>(response);
                    twoFactorsId = objResponse.TwoFactorAuthTokenId.ToString();
                    statusLabel.Text = "Token requested with Id: " + twoFactorsId + Environment.NewLine + "Please provide the msm code in Mobile Token";
                }
            }
            catch(Exception ex)
            {
                statusLabel.Text = ex.Message;
                buttonRequest.Enabled = false;
            }
        }

        private void validateToken(object sender, EventArgs e)
        {
            try
            {
                if(!string.IsNullOrEmpty(textMobileToken.Text) && !string.IsNullOrEmpty(twoFactorsId)) 
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["uriValidate"]);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Headers.Add("X-Forwarded-Host", "ci-portal-tiempodev.azurewebsites.net");
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = "{\"twoFactorTokenId\":\"" + twoFactorsId + "\"," +
                                      "\"code\":\"" + textMobileToken.Text.Trim() + "\"}";

                        streamWriter.Write(json);
                    }
                    statusLabel.Text = "Requestin verification, please wait...";
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var response = streamReader.ReadToEnd();
                        var objResponse = JsonConvert.DeserializeObject<StepTwoResponse>(response);                        
                        this.BeginInvoke(new Action(() => Clipboard.SetText(objResponse.AccessToken)));
                    }
                    statusLabel.Text = "Token copied to memory";
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = ex.Message;
            }

        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            if(currentStep == 0)
            {
                statusLabel.Text = "Requesting token, please wait...";
                buttonRequest.Enabled = false;
                worker.RunWorkerAsync(e);
            }
            else
            {
                if(string.IsNullOrEmpty(textMobileToken.Text) || string.IsNullOrWhiteSpace(textMobileToken.Text))
                {
                    return;
                }
                statusLabel.Text = "Validating token, please wait...";
                worker.RunWorkerAsync(e);               
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (currentStep == 0)
            {
                requestToken(sender, e); 
            }
            else
            {                
                validateToken(sender, e);                
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonRequest.Enabled = true;
            if (currentStep == 0)
            {
                currentStep = 1;
                buttonRequest.Text = "Validate Token";
            }
            else
            {
                buttonRequest.Text = "Request Token";
                currentStep = 0;
                textMobileToken.Text = string.Empty;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (checkBoxRemember.Checked)
            {
                SetSetting("user", textUser.Text);
                SetSetting("password",textPassword.Text);
            }
            else
            {
                SetSetting("user", "");
                SetSetting("password", "");
            }
        }

        private static void SetSetting(string key, string value)
        {
            Configuration configuration =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
