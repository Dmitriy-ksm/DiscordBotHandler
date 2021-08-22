using DiscordBotHandler.Interfaces;
using DiscordBotHandler.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DiscordBotHandler.Test
{
    public class CryptoServiceTest : BaseTest
    {
        public CryptoServiceTest(ITestOutputHelper output) : base(output) { }
        
        [Fact]
        public void CreptoCheck()
        {
            #region Arrange
            #endregion

            #region Act
            var res = CryptoService.GetEtherGas(ConfigurationManager.AppSettings["etherScanApi"]).Result;
            #endregion

            #region Assert
            Assert.IsType<EtherGasBotData>(res);
            Assert.NotNull(res);
            #endregion

            SetTestOutput("Crypro test passed");
        }
    }
}
