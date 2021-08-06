using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBotHandler.Interfaces
{
    public interface ICrypto
    {
        public Task<string> GetCryptoInfoAsync();
    }
    public class EthAnswer
    {
        public string status { get; set; }
        public string message { get; set; }
        public Dictionary<string, string> result { get; set; }
    }

    public class EtherGasBotData
    {
        public string GasAvarage { get; set; }
        public bool IsGasGet { get; set; }
        public bool IsEthGet { get; set; }
        public string EthUsd { get; set; }
        public string EthUsdTime { get; set; }
        public string EthBtc { get; set; }
        public string EthBtcTime { get; set; }
    }
}
