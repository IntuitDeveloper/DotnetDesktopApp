using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedDesktopApp
{
    [Serializable]
    public class IppRealmOAuthProfile
    {
        public string accessToken { get; set; }
        public string accessSecret { get; set; }
        public string realmId { get; set; }
        public string dataSource { get; set; }
        public DateTime expirationDateTime { get; set; }
    }
}
