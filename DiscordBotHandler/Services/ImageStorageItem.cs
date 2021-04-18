using DiscordBotHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiscordBotHandler.Services
{
    class ImageStorageItem : IStorage<Image>
    {
        private bool _disposed = false;
        public string Name { get; set; }
        Dictionary<string, string> Storage = new Dictionary<string, string>();
        public IStorageProvider<Image> Provider { get; set; }
        public ImageStorageItem(IServiceProvider services)
        {
            var providerDelegat = services.GetRequiredService<Func<StorageContains, IStorageProvider<Image>>>();
            Provider = providerDelegat(StorageContains.DotaItem);
            string path = Path.Combine("Cache", "itemImage");
            if (Directory.Exists("Cache") && Directory.Exists(path))
            {
                foreach (var fileName in Directory.GetFiles(path, "*.jpg"))
                {
                    Storage.Add(Path.GetFileNameWithoutExtension(fileName), fileName);
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }
        public bool HasObject(string key)
        {
            if (Storage.ContainsKey(key))
            { return true; }
            else return false;
        }
        public Image GetObject(string key)
        {
            if (HasObject(key))
            {
                return Image.Load(Storage[key]);
            }
            else
            {
                var image = Provider.GetObject(key);
                string path = Path.Combine("Cache", "itemImage", key);
                image.SaveAsJpeg(Path.Combine(path + ".jpg"));
                Storage.Add(key, path + ".jpg");
                return image;
            }
        }
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {

            }

            _disposed = true;
        }
    }
}
