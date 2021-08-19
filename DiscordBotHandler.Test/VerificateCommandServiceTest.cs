using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Helpers;
using DiscordBotHandler.Services;
using EntityFrameworkCoreMock;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DiscordBotHandler.Test
{
    public class VerificateCommandServiceTest : BaseTest
    {
        public VerificateCommandServiceTest(ITestOutputHelper output) : base(output) { }
        [Fact]
        public void Test()
        {
            #region Arrange
            //var mockSetChannel = GetMock(GetTestChannels);
            //var mockSetGuild = GetMock(GetTestGuilds);
            //var mockSetCommandAcces = GetMock(GetTestCommandAccesses);


            var dbContextMock = new DbContextMock<FakeContext>();

            var mockSetChannel = dbContextMock.CreateDbSetMock(x => x.Channels, GetTestChannels());
            var mockSetGuild = dbContextMock.CreateDbSetMock(x => x.Guilds, GetTestGuilds());
            var mockSetCommandAcces = dbContextMock.CreateDbSetMock(x => x.CommandAccesses, GetTestCommandAccesses());

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            var mock = new Mock<FakeContext>();
            mock.Setup(c => c.Channels).Returns(mockSetChannel.Object);
            mock.Setup(c => c.Guilds).Returns(mockSetGuild.Object);
            mock.Setup(c => c.CommandAccesses).Returns(mockSetCommandAcces.Object);
            mock.Setup(c => c.SaveChangesAsync(token)).Returns(() => Task.Run(() => { SaveFakeDbSets(mockSetChannel, mockSetGuild, mockSetCommandAcces); return 1; })).Verifiable();
            mock.Setup(c => c.SaveChanges()).Returns(() => { SaveFakeDbSets(mockSetChannel, mockSetGuild, mockSetCommandAcces); return 1; }).Verifiable();
            //mock.Object.CommandAccesses.Add(new CommandAccess() { Command = "all", Channels = new List<Channels>() });

            var verificatorService = new VerificateCommandService(mock.Object);
            #endregion

            #region Act
            var firstCheck = verificatorService.IsValid(Consts.CommandModuleNameCrypto, 1, 1, out _);
            verificatorService.SetPermit(Consts.CommandModuleNameCrypto, 1, 1);
            var secondCheck = verificatorService.IsValid(Consts.CommandModuleNameCrypto, 1, 1, out _);
            verificatorService.UnsetPermit(Consts.CommandModuleNameCrypto, 1, 1);
            var thirdCheck = verificatorService.IsValid(Consts.CommandModuleNameCrypto, 1, 1, out _);
            #endregion

            #region Assert
            Assert.True(!firstCheck);
            Assert.True(secondCheck);
            Assert.True(!thirdCheck);
            #endregion
        }



        private IQueryable<Channels> GetTestChannels()
        {
            return new List<Channels>()
            {
                new Channels{ChannelId = 1, GuildId = 1, Commands = new List<CommandAccess>()}
            }.AsQueryable();
        }

        private IQueryable<CommandAccess> GetTestCommandAccesses()
        {
            return new List<CommandAccess>().AsQueryable();
        }

        private IQueryable<Guilds> GetTestGuilds()
        {
            return new List<Guilds>()
            {
                new Guilds{ GuildId = 1, VoiceChannelId = 0, WordSearches = new List<WordSearch>()}
            }.AsQueryable();
        }
    }
}
