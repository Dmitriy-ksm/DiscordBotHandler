using Xunit;
using DiscordBotHandler.Services;
using System;
using System.Configuration;
using Xunit.Abstractions;

namespace DiscordBotHandler.Test
{
    public class DotaAssistansServiceTest : BaseTest
    {

        public DotaAssistansServiceTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void DotaAssistansTest()
        {
            #region Arrange
            var dotaAssistans = new DotaAssistansService();
            #endregion
            
            #region Act
            var resultSteamId = dotaAssistans.GetSteamIdAsync("https://steamcommunity.com/profiles/76561198064401017/").Result;
            var lastMatchId = dotaAssistans.GetLastMatchBySteamId(resultSteamId).Result;
            var resultMatchById = dotaAssistans.GetDotaByMatchIdAsync(lastMatchId).Result;
            var resultMatchBySteamId = dotaAssistans.GetDotaAsync(resultSteamId).Result;
            #endregion

            #region Assert
            Assert.NotEqual((ulong)0, resultSteamId);
            Assert.NotEqual((ulong)0, lastMatchId);
            Assert.NotNull(resultMatchById);
            Assert.NotNull(resultMatchBySteamId);
            Assert.Equal(resultMatchById, resultMatchBySteamId);
            #endregion

            SetTestOutput("DotaAssistans test passed");
        }

    }
}
