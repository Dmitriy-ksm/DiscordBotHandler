using DiscordBotHandler.Entity;
using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Interfaces;
using DiscordBotHandler.Services;
using DiscordBotHandler.Test.Classes;
using EntityFrameworkCoreMock;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DiscordBotHandler.Test
{
    public class CooldownServiceTest : BaseTest
    {
        public CooldownServiceTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void DotaAssistansTest()
        {
            #region Arrange
            var dbContextMock = new DbContextMock<FakeContext>();

            var mockSet = dbContextMock.CreateDbSetMock(x => x.Cooldowns, GetTestCooldowns());

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            var mock = new Mock<FakeContext>();
            mock.Setup(c => c.Cooldowns).Returns(mockSet.Object);
            
            var services = new ServiceCollection()
               .AddScoped<IEFContext>(provider => mock.Object)
               .AddSingleton<ILogger, LoggerEmptyService>()
               .BuildServiceProvider();

            SaveChangesInFakeContext(mock, token, SaveFakeDbSets, mockSet);

            var cooldownService = new CooldownService(services);
            #endregion

            #region Act
            bool firstCheck = cooldownService.Check("test");
            cooldownService.Set("test");
            bool secondCheck = cooldownService.Check("test");
            Thread.Sleep(5000);
            bool thirdCheck = cooldownService.Check("test");
            #endregion

            #region Assert
            Assert.True(firstCheck);
            Assert.False(secondCheck);
            Assert.True(firstCheck);
            #endregion

            SetTestOutput("Cooldown Service test passed");
        }

        private IQueryable<Cooldown> GetTestCooldowns()
        {
            return new List<Cooldown>() { new Cooldown { Key = "test", KeyCooldown = 5, LastUse = DateTime.MinValue } }.AsQueryable();
        }
    }
}
