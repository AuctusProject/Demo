using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/Ethereum")]
    public class EthereumController : Controller
    {
        [HttpGet("Get")]
        public string Get()
        {
            return "Ok";
        }

        [HttpPost("CreateAccount")]
        public Auctus.EthereumProxy.EthereumManager.WalletInfo CreateAccount(string password)
        {
            return Auctus.EthereumProxy.EthereumManager.CreateAccount(password);
        }

        [HttpPost("DeployContratc")]
        public string DeployContratc(string owner, string password, int gasLimit, int gwei)
        {
            return Auctus.EthereumProxy.EthereumManager.DeployContratc(owner, password, gasLimit, gwei);
        }

        [HttpGet("GetBalance")]
        public double GetBalance(string address)
        {
            return Auctus.EthereumProxy.EthereumManager.GetBalance(address);
        }

        [HttpPost("Send")]
        public string Send(string from, string password, string to, double value, int gasLimit, int gwei)
        {
            return Auctus.EthereumProxy.EthereumManager.Send(from, password, to, value, gasLimit, gwei);
        }

        [HttpPost("GetBalanceFromSmartContract")]
        public double GetBalanceFromSmartContract(string from, string smartContractAddress)
        {
            return Auctus.EthereumProxy.EthereumManager.GetBalanceFromSmartContract(from, smartContractAddress);
        }

        [HttpPost("DrainFromContract")]
        public string DrainFromContract(string owner, string password, int gasLimit, int gwei, string smartContractAddress)
        {
            return Auctus.EthereumProxy.EthereumManager.DrainFromContract(owner, password, gasLimit, gwei, smartContractAddress);
        }

        [HttpGet("GetTransactionInformation")]
        public Auctus.EthereumProxy.EthereumManager.TransactionInfo GetTransactionInformation(string transactionHash)
        {
            return Auctus.EthereumProxy.EthereumManager.GetTransactionInformation(transactionHash);
        }
    }
}
