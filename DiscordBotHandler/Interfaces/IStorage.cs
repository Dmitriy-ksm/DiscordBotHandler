using System;

namespace DiscordBotHandler.Interfaces
{
    public interface IStorage<T> : IDisposable 
    {
        public string Name { get; set; }
        public IStorageProvider<T> Provider { get; set; }
        public bool HasObject(string key);
        public T GetObject(string key);
    }
    public enum StorageContains
    {
        DotaHero = 0,
        DotaItem = 1
    }
}
