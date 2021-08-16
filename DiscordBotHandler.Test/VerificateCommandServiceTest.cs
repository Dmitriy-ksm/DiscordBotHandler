using DiscordBotHandler.Entity.Data;
using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Services;
using Moq;
using System.Collections.Generic;
using System.Linq;
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
            var mockSetChannel = GetMock(GetTestChannels);

            var mockSetGuild = GetMock(GetTestGuilds);

            var mock = new Mock<EFContext>();
            mock.Setup(c => c.Channels).Returns(mockSetChannel.Object);
            mock.Setup(c => c.Guilds).Returns(mockSetGuild.Object);

            var verificatorService = new VerificateCommandService(mock.Object);
            #endregion
        }



        private IQueryable<Channels> GetTestChannels()
        {
            return new List<Channels>()
            {
                new Channels{ChannelId = 1, GuildId = 1, Commands = new List<CommandAccess>()}
            }.AsQueryable();
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
