using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;

namespace Kulki
{
    public static class Config
    {
        private static Storage storage;
        public static ProfileList ProfileList;
        public static List<ProfileData> ProfileData;
        public static Storage Storage
        {
            get
            {
                if (storage == null)
                {
                    try
                    {
                        StorageReader sr = new StorageReader("config.conf");
                        storage = sr.Load();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Config file not exsist - creating default config file");
                        storage = new Storage();
                        storage.Set("width", 800);
                        storage.Set("height", 600);
                        storage.Set("fullscreen", false);
                        storage.Set("boardSize", 10);
                        storage.Set("colors", 5);
                        storage.Set("ballStyle", 0);
                        storage.Set("volume", 0.30f);
                        storage.Set("showFps", false);
                        storage.Save("config.conf");
                    }
                }
                return storage;

            }
        }

        
    }
}
