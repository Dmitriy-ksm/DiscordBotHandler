using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
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
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mock;
        }
    }
}
