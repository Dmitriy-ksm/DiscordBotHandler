using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Interfaces;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBotHandler.Services
{
    class Crypto:ICrypto
    {
        private EFContext _dbContext;
        public Crypto(EFContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> GetCryptoInfoAsync()
        {
            var newCrypto = await getEtherGas(ConfigurationManager.AppSettings["etherScanApi"]);
            //string directory = ConfigurationManager.AppSettings["CryptoContainter"];
            var result = "Текущийкурс Ether: " + newCrypto.EthUsd + "$" + Environment.NewLine;
            /*if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string lastCrypto = Path.Combine(directory, "crypto.txt");*/
            var lastCryptoDataDb = _dbContext.CryptoInfo.AsQueryable().OrderByDescending(x => x.Id).FirstOrDefault();
            EtherGasBotData lastCryptoData = null;
            if (lastCryptoDataDb != null)
            {
                lastCryptoData = new EtherGasBotData()
                {
                    EthUsdTime = lastCryptoDataDb.EthUsdTime,
                    EthBtc = lastCryptoDataDb.EthBtc.ToString(CultureInfo.InvariantCulture),
                    EthBtcTime = lastCryptoDataDb.EthBtcTime,
                    EthUsd = lastCryptoDataDb.EthUsd.ToString(CultureInfo.InvariantCulture)
                };

            }
            CryptoInfo cryptoInfoNew = new CryptoInfo() { 
                EthBtc = Convert.ToDouble(newCrypto.EthBtc, CultureInfo.InvariantCulture),
                EthBtcTime = newCrypto.EthBtcTime,
                EthUsd = Convert.ToDouble(newCrypto.EthUsd,CultureInfo.InvariantCulture),
                EthUsdTime = newCrypto.EthUsdTime,
                GasAvarage = Convert.ToInt32(newCrypto.GasAvarage)
            };

            _dbContext.CryptoInfo.Add(cryptoInfoNew);
            /*if (!File.Exists(lastCrypto))
            {
                File.Create(lastCrypto);
            }
            else
            {
                using (StreamReader reader = new StreamReader(lastCrypto))
                {
                    var file = reader.ReadToEnd();
                    try
                    {
                        lastCryptoData = JsonSerializer.Deserialize<EtherGasBotData>(file);
                    }
                    catch
                    {
                        result += "Данных прошлой сессии не найдено" + Environment.NewLine;
                    }
                }
            }
            using (StreamWriter writer = new StreamWriter(lastCrypto))
            {
                var jsonString = JsonSerializer.Serialize(newCrypto);
                writer.Write(jsonString);
            }*/
            await _dbContext.SaveChangesAsync();
            if (lastCryptoData != null)
            {
                double procent = ((Convert.ToDouble(newCrypto.EthUsd, CultureInfo.InvariantCulture) / Convert.ToDouble(lastCryptoData.EthUsd, CultureInfo.InvariantCulture) - 1) * 100);
                long timePassed = long.Parse(newCrypto.EthUsdTime) - long.Parse(lastCryptoData.EthUsdTime);
                TimeSpan timePassedTime = new TimeSpan(0, 0, (int)timePassed);
                result += "Разница между сессиями: " + Math.Round(procent, 2) + "% за " + timePassedTime.ToString(@"dd\ hh\:mm\:ss") + Environment.NewLine;
            }
            result += "Газ: " + newCrypto.GasAvarage + " gwei";
            return result;
        }
        public static async Task<EtherGasBotData> getEtherGas(string apiKey)
        {
            EtherGasBotData returnValue = new EtherGasBotData();
            HttpClient client = new HttpClient();
            var streamTask = client.GetStreamAsync("https://api.etherscan.io/api?module=gastracker&action=gasoracle&apikey=" + apiKey);
            var streamTask2 = client.GetStreamAsync("https://api.etherscan.io/api?module=stats&action=ethprice&apikey=" + apiKey);
            var responseFirst = await JsonSerializer.DeserializeAsync<EthAnswer>(await streamTask);
            var responseSecond = await JsonSerializer.DeserializeAsync<EthAnswer>(await streamTask2);
            if (responseFirst.status == "1")
            {
                returnValue.GasAvarage = responseFirst.result["ProposeGasPrice"];
                returnValue.IsGasGet = true;
            }
            if (responseSecond.status == "1")
            {
                returnValue.EthBtc = responseSecond.result["ethbtc"];
                returnValue.EthBtcTime = responseSecond.result["ethbtc_timestamp"];
                returnValue.EthUsd = responseSecond.result["ethusd"];
                returnValue.EthUsdTime = responseSecond.result["ethusd_timestamp"];
                returnValue.IsEthGet = true;
            }
            return returnValue;
        }

    }
}
