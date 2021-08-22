using DiscordBotHandler.Entity;
using DiscordBotHandler.Entity.Entities;
using DiscordBotHandler.Test.Classes;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace DiscordBotHandler.Test
{
    public class BaseTest
    {
        private readonly ITestOutputHelper output;

        public BaseTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void SetTestOutput(string message)
        {
            output.WriteLine(message);
        }

        public static Mock<DbSet<T>> GetMock<T>(Func<IQueryable<T>> getObject) where T : class
        {
            var data = getObject();

            var mock = new Mock<DbSet<T>>();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            return mock;
        }
        public delegate void SaveFunction(params IDbSetMock[] items);
        public void SaveChangesInFakeContext(Mock<FakeContext> mock, CancellationToken token, SaveFunction SaveFunc, params IDbSetMock[] items)
        {
            mock.Setup(c => c.SaveChangesAsync(token)).Returns(() => Task.Run(() => { SaveFunc(items); return 1; })).Verifiable();
            mock.Setup(c => c.SaveChanges()).Returns(() => { SaveFunc(items); return 1; }).Verifiable();
        }
        public void SaveFakeDbSets(params IDbSetMock[] items)
        {
            foreach (var item in items)
            {
                item.SaveChanges();
            }
        }

        protected IQueryable<Guilds> GetTestGuilds()
        {
            return new List<Guilds>()
            {
                new Guilds{ GuildId = 1, VoiceChannelId = 0, WordSearches = new List<WordSearch>()}
            }.AsQueryable();
        }
    }
}
