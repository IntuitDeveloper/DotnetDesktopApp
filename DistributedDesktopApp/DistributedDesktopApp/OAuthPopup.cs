using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace DistributedDesktopApp
{
    public partial class OAuthPopup : Form
    {
        #region " Member Variables "

        private const string _dummyProtocol = "https://";
        private const string _dummyHost = "www.example.com";
        private IppRealmOAuthProfile _ippRealmOAuthProfile;
        private IToken _requestToken;
        private string _consumerKey = "";
        private string _consumerSecret = "";
        private string _oauthVerifier = "";
        private bool _caughtCallback = false;

        #endregion

        public OAuthPopup(IppRealmOAuthProfile ippRealmOAuthProfile)
        {
            InitializeComponent();
            _ippRealmOAuthProfile = ippRealmOAuthProfile;
            readConsumerKeysFromConfiguration();
            startOAuthHandshake();
        }

        #region " App Configuration "

        private void readConsumerKeysFromConfiguration()
        {
            _consumerKey = ConfigurationSettings.AppSettings["consumerKey"];
            _consumerSecret = ConfigurationSettings.AppSettings["consumerSecret"];
        }

        #endregion

        #region " Start OAuth Handshake - Get Request Token and Redirect to IPP for User Authorization "

        private void startOAuthHandshake()
        {
            _requestToken = getOauthRequestTokenFromIpp(_consumerKey, _consumerSecret);
            redirectToIppForUserAuthorization(_requestToken);
        }

        #region " Get Request Token "

        private IToken getOauthRequestTokenFromIpp(string consumerKey, string consumerSecret)
        {
            IOAuthSession oauthSession = createDevDefinedOAuthSession(consumerKey, consumerSecret);
            return oauthSession.GetRequestToken();
        }

        #endregion

        #region " Redirect to Service Provider - IPP "

        private void redirectToIppForUserAuthorization(IToken requestToken)
        {
            var oauthUserAuthorizeUrl = ConfigurationSettings.AppSettings["oauthUserAuthUrl"];
            oauthBrowser.Navigate(oauthUserAuthorizeUrl + "?oauth_token=" + requestToken.Token + "&oauth_callback=" + UriUtility.UrlEncode(_dummyProtocol + _dummyHost));
        }

        #endregion

        #endregion

        #region " Capture Callback, Exchange Request Token for Access Token and Close Dialog "

        private void oauthBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.Host == _dummyHost)
            {
                NameValueCollection query = HttpUtility.ParseQueryString(e.Url.Query);
                _oauthVerifier = query["oauth_verifier"];
                _ippRealmOAuthProfile.realmId = query["realmId"];
                _ippRealmOAuthProfile.dataSource = query["dataSource"];
                _caughtCallback = true;
                oauthBrowser.Navigate("about:blank");
            }
            else if (_caughtCallback)
            {
                IToken accessToken = exchangeRequestTokenForAccessToken(_consumerKey, _consumerSecret, _requestToken);
                _ippRealmOAuthProfile.accessToken = accessToken.Token;
                _ippRealmOAuthProfile.accessSecret = accessToken.TokenSecret;
                _ippRealmOAuthProfile.expirationDateTime = DateTime.Now.AddMonths(6);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        #region " Exhange Request Token for Access Token "


        public IToken exchangeRequestTokenForAccessToken(string consumerKey, string consumerSecret, IToken requestToken)
        {
            IOAuthSession oauthSession = createDevDefinedOAuthSession(consumerKey, consumerSecret);
            return oauthSession.ExchangeRequestTokenForAccessToken(requestToken, _oauthVerifier);
        }

        #endregion

        #endregion

        #region " DevDefined Helper Functions "

        private IOAuthSession createDevDefinedOAuthSession(string consumerKey, string consumerSecret)
        {

            var oauthRequestTokenUrl = ConfigurationSettings.AppSettings["oauthBaseUrl"] + ConfigurationSettings.AppSettings["oauthRequestTokenEndpoint"];
            var oauthAccessTokenUrl = ConfigurationSettings.AppSettings["oauthBaseUrl"] + ConfigurationSettings.AppSettings["oauthAccessTokenEndpoint"];
            var oauthUserAuthorizeUrl = ConfigurationSettings.AppSettings["oauthUserAuthUrl"];

            OAuthConsumerContext consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1
            };

            return new OAuthSession(consumerContext, oauthRequestTokenUrl, oauthUserAuthorizeUrl, oauthAccessTokenUrl);

        }

        #endregion

        private void OAuthPopup_Load(object sender, EventArgs e)
        {

        }

        private void oauthBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
