using DiscordBotHandler.Entity;
using DiscordBotHandler.Entity.Entities;
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
    public class WordSearchServiceTest : BaseTest
    {
        public WordSearchServiceTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Test()
        {
            #region Arrange

            var dbContextMock = new DbContextMock<FakeContext>();

            var mockSetGuild = dbContextMock.CreateDbSetMock(x => x.Guilds, GetTestGuilds());

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            var mock = new Mock<FakeContext>();
            mock.Setup(c => c.Guilds).Returns(mockSetGuild.Object);

            SaveChangesInFakeContext(mock, token, SaveFakeDbSets, mockSetGuild);
            
            var services = new ServiceCollection()
                .AddScoped<IEFContext>(provider => mock.Object)
                .BuildServiceProvider();

            var wordSearchesService = new WordSearchService(services);
            var searchText = "Test";
            var reply = "Test passed";
            #endregion

            #region Act
            wordSearchesService.AddSearchWord(1, reply, searchText);
            var answer = wordSearchesService.SearchWord(1, searchText);
            var nullAnswer = wordSearchesService.SearchWord(1, "randomtexthere");
            #endregion

            #region Assert
            Assert.NotNull(answer);
            Assert.Equal(answer, reply);
            Assert.Null(nullAnswer);
            #endregion

            SetTestOutput("WordSearchService test passed");
        }
    }

   
}
