<img src="http://dl.auctus.org/img/auctus-01.png" width="200px" >

---

# Auctus Platform Demo

The idea behind the Auctus platform is to provide a global smart contract based platform for pension fund management. The platform will allow to easily configure pension fund rules and to automatically generate smart contract code, as well as to deploy it on the Ethereum network. The demo version of the platform includes simulation features because it would be very difficult to understand the usability with an alpha release, without a simulation feature that mimics changing funds over time.

## Getting Started

The platform is being built in C#, using the .NET Core framework. The backend code, to dynamically generate and deploy smart contracts in the Ethereum network are organized in the folder /EthereumProxy.

### Compiling

To secure sensitive data, some classes are not included in the repository. To compile the solution, You will need to include the following classes in the Util project:

```cs
namespace Auctus.Util.NotShared
{
    public static class Config
    {
        public readonly static string DbConnString = "db_connection_string";

        public readonly static string GOOGLE_CAPTCHA_SECRET = "captcha_secret";
        public readonly static string GOOGLE_CAPTCHA_KEY = "captcha_key";

        // Ethereum Proxy Config Strings
        public static readonly string AUCTUS_ADDRESS = "eth_address";
        public static readonly string AUCTUS_PASSWORD = Security.Encrypt("strong_password");
        public static readonly string URL_GAS_PRICE = "https://ropsten.etherscan.io/api?module=proxy&action=eth_gasPrice&apikey=KEY";

        public static readonly bool IS_WINDOWS = true;

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
         * ----------------------- WINDOWS ----------------------------------------- *     
         * MAIN:      C:\\Users\\Dell\\AppData\\Roaming\\Ethereum\\                  *
         * TESTNET:   C:\\Users\\Dell\\AppData\\Roaming\\Ethereum\\testnet\\         *
         * DEV:       C:\\Users\\Dell\\AppData\\Local\\Temp\\ethereum_dev_mode\\     *
         * ----------------------- LINUX ------------------------------------------- *
         * MAIN:      /home/ubuntu/.ethereum/                                        *
         * TESTNET:   /home/ubuntu/.ethereum/testnet/                                *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        public readonly static string GETH_PATH = "C:\\Users\\Dell\\AppData\\Roaming\\Ethereum\\testnet\\";
    }
}
```

```cs
using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace Auctus.Util.NotShared
{
    public static class Security
    {
        public static string GenerateEncryptedPassword()
        {
            return Encrypt(Guid.NewGuid().ToString());
        }

        public static string Encrypt(string plainText)
        {
            //implement encrypt function
			return string.Empty;
        }

        public static string Decrypt(string encryptedText)
        {
            //implement decrypt function
			return string.Empty;
        }
    }
}
```