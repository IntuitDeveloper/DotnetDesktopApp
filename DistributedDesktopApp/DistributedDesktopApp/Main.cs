using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Intuit.Ipp.Core;
using Intuit.Ipp.Core.Configuration;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Intuit.Ipp.LinqExtender;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace DistributedDesktopApp
{
    public partial class Main : Form
    {
        #region " Member Variables "

        private IppRealmOAuthProfile _ippRealmOAuthProfile;

        #endregion

        #region " Constructor "

        public Main()
        {
            InitializeComponent();
        }

        #endregion

        private void Main_Load(object sender, EventArgs e)
        {
            _ippRealmOAuthProfile = readProfileFromDisk();
            if (_ippRealmOAuthProfile == null) { setManageConnectionToConnect(); }
            else if (_ippRealmOAuthProfile.expirationDateTime.Subtract(DateTime.Now).TotalDays < 30)
            {
                //We are within 30 days of token expiration, so call the Reconnect API to get new access tokens
                try
                {
                    callReconnectAndUpdateProfile();
                    setManageConnectionToDisconnect();
                    Application.DoEvents();
                    displayCustomers();
                }
                catch
                {
                    _ippRealmOAuthProfile = new IppRealmOAuthProfile();
                    saveProfileToDisk();
                    setManageConnectionToConnect();
                    if (File.Exists("ipp.xml")) { File.Delete("ipp.xml"); }
                }
            }
            else
            {
                setManageConnectionToDisconnect();
                Application.DoEvents();
                displayCustomers();
            }
        }

        private void manageConnection_Click(object sender, EventArgs e)
        {
            string actionTag = (manageConnection.Tag == null) ? "" : manageConnection.Tag.ToString();
            switch (actionTag)
            {
                case "Disconnect":
                    callPlatform("https://appcenter.intuit.com/api/v1/Connection/Disconnect");
                    _ippRealmOAuthProfile = new IppRealmOAuthProfile();
                    dataGridView1.DataSource = null;
                    setManageConnectionToConnect();
                    if (File.Exists("ipp.xml")) { File.Delete("ipp.xml"); }
                    break;
                default:
                    _ippRealmOAuthProfile = new IppRealmOAuthProfile();
                    OAuthPopup oauthPopup = new OAuthPopup(_ippRealmOAuthProfile);
                    DialogResult dialogResult = oauthPopup.ShowDialog();
                    switch (dialogResult)
                    {
                        case System.Windows.Forms.DialogResult.OK:
                            saveProfileToDisk();
                            setManageConnectionToDisconnect();
                            Application.DoEvents();
                            displayCustomers();
                            break;
                    }
                    break;
            }

        }

        private void callReconnectAndUpdateProfile()
        {
            string xmlResponse = callPlatform("https://appcenter.intuit.com/api/v1/Connection/Reconnect");
            XmlDocument reconnectResponse = new XmlDocument();
            reconnectResponse.LoadXml(xmlResponse);
            foreach (XmlNode childNode in reconnectResponse.DocumentElement)
            {
                switch (childNode.Name)
                {
                    case "OAuthToken": _ippRealmOAuthProfile.accessToken = childNode.InnerText; break;
                    case "OAuthTokenSecret": _ippRealmOAuthProfile.accessSecret = childNode.InnerText; break;
                    default: break;
                }
            }
            _ippRealmOAuthProfile.expirationDateTime = DateTime.Now.AddMonths(6);
            saveProfileToDisk();
        }


        #region " QuickBooks API call to display customers when connected "

        private void displayCustomers()
        {
            string consumerKey = ConfigurationSettings.AppSettings["consumerKey"];
            string consumerSecret = ConfigurationSettings.AppSettings["consumerSecret"];
            OAuthRequestValidator oauthRequestValidator = new OAuthRequestValidator(_ippRealmOAuthProfile.accessToken, _ippRealmOAuthProfile.accessSecret, consumerKey, consumerSecret);
            ServiceContext serviceContext = new ServiceContext(_ippRealmOAuthProfile.realmId, IntuitServicesType.QBO, oauthRequestValidator);
            DataService dataService = new DataService(serviceContext);
            QueryService<Customer> customerQueryService = new QueryService<Customer>(serviceContext);
            dataGridView1.DataSource = customerQueryService.ExecuteIdsQuery("Select * From Customer MaxResults 50").ToList();
        }

        #endregion

        #region " Platform API call to Disconnect Realm "

        public string callPlatform(string url)
        {

            OAuthConsumerContext consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = ConfigurationSettings.AppSettings["consumerKey"],
                SignatureMethod = SignatureMethod.HmacSha1,
                ConsumerSecret = ConfigurationSettings.AppSettings["consumerSecret"]
            };

            OAuthSession oSession = new OAuthSession(consumerContext, "http://www.example.com", "http://www.example.com", "http://www.example.com");

            oSession.ConsumerContext.UseHeaderForOAuthParameters = true;

            if (_ippRealmOAuthProfile.accessToken.Length > 0)
            {
                oSession.AccessToken = new TokenBase
                {
                    Token = _ippRealmOAuthProfile.accessToken,
                    ConsumerKey = ConfigurationSettings.AppSettings["consumerKey"],
                    TokenSecret = _ippRealmOAuthProfile.accessSecret
                };

                IConsumerRequest conReq = oSession.Request();
                conReq = conReq.Get();
                conReq = conReq.ForUrl(url);
                try
                {
                    conReq = conReq.SignWithToken();
                    return conReq.ReadBody();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return "";
        }

        #endregion

        #region " Connect/Disconnect Button Styles and Messaging "

        private void setManageConnectionToDisconnect()
        {
            manageConnection.BackColor = Color.Red;
            manageConnection.Text = "Disconnect from QuickBooks";
            manageConnection.Tag = "Disconnect";
            connectionMessage.Text = "You're connected!  View your customers below.";
        }

        private void setManageConnectionToConnect()
        {
            manageConnection.BackColor = Color.ForestGreen;
            manageConnection.Text = "Connect to QuickBooks";
            manageConnection.Tag = "Connect";
            connectionMessage.Text = "You're not connected!  Use the Connect to QuickBooks button below.";
        }

        #endregion

        #region " Encrypted User Profile Management on Disk "

        public void saveProfileToDisk()
        {
            using (var fileStream = File.OpenWrite("ipp.xml"))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(_ippRealmOAuthProfile.GetType());
                StringWriter textWriter = new StringWriter();
                xmlSerializer.Serialize(textWriter, _ippRealmOAuthProfile);
                string encryptedRealmProfile = StringCipher.Encrypt(textWriter.ToString(), "<FILL_IN_USER_PASSWORD_OR_OTHER_UNIQUE_USER_VALUE>");
                byte[] bytes = new byte[encryptedRealmProfile.Length * sizeof(char)];
                System.Buffer.BlockCopy(encryptedRealmProfile.ToCharArray(), 0, bytes, 0, bytes.Length);
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }


        public IppRealmOAuthProfile readProfileFromDisk()
        {
            try
            {
                if (!File.Exists("ipp.xml")) { return null; }
                string encryptedRealmProfile = File.ReadAllText("ipp.xml", Encoding.Unicode);
                if (encryptedRealmProfile.Length == 0) { return null; }
                string decryptedRealmProfile = StringCipher.Decrypt(encryptedRealmProfile, "<FILL_IN_USER_PASSWORD_OR_OTHER_UNIQUE_USER_VALUE>");
                TextReader textReader = new StringReader(decryptedRealmProfile);
                XmlSerializer xmlSerializer = new XmlSerializer((new IppRealmOAuthProfile()).GetType());
                return (IppRealmOAuthProfile)xmlSerializer.Deserialize(textReader);
            }
            catch (Exception ex) { return null; }
        }

        #endregion
    }
}
