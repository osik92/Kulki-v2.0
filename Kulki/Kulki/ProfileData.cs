using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace Kulki
{
    public class ProfileData
    {
        public int MaxCombo;
        public int TotalGameCount;
    }

    public class ProfileList
    {
        public List<PlayerNickIdValue> Profiles;

        public ProfileList()
        {
            Profiles = new List<PlayerNickIdValue>();
        }
    }

    public class PlayerNickIdValue
    {
        public int Id;
        public String Nick;
    }



    public class ProfileSaver
    {
        private string gamename;
        private string filename;
        private StorageDevice storageDevice;

        public event EventHandler ProfileDataSaved;
        public event EventHandler ProfileDataReady;
        public event EventHandler StorageReady;

        public ProfileSaver(string gamename, string filename)
        {
            this.gamename = gamename;
            this.filename = filename;
        }
        public ProfileList Load()
        {
            ProfileList data = new ProfileList();

            var result = StorageDevice.BeginShowSelector(null, null);
            storageDevice = StorageDevice.EndShowSelector(result);
            if (storageDevice != null)
            {
                var openResult = storageDevice.BeginOpenContainer(gamename, null,  null);
                openResult.AsyncWaitHandle.WaitOne();
                using (StorageContainer file = storageDevice.EndOpenContainer(openResult))
                        {
                            using (Stream stream = file.OpenFile(filename, FileMode.OpenOrCreate))
                            {
                                if (stream.Length == 0)
                                {
                                    Debug.WriteLine("Profile list not exsist - create empty file");
                                    return data;
                                }
                                XmlSerializer serializer = new XmlSerializer(typeof(ProfileList));
                                var res = serializer.Deserialize(stream);
                                if (!(res is ProfileList))
                                {
                                    Debug.WriteLine("Profile list not exsist");
                                    return data;
                                }
                                Debug.WriteLine("Profile list loaded");
                                data = res as ProfileList;
                                if (ProfileDataReady != null)
                                    ProfileDataReady(this, EventArgs.Empty);
                            }
                        }
                    }
            
            return  data;
        }

        public void Save(ProfileList data)
        {
            if (storageDevice == null)
            {
                var begin = StorageDevice.BeginShowSelector(null, null);
                while (true)
                {
                    if (begin.IsCompleted)
                    {
                        storageDevice = StorageDevice.EndShowSelector(begin);
                        break;
                    }
                }
            }

            if (storageDevice != null)
            {

                storageDevice.BeginOpenContainer(gamename, new AsyncCallback(
                    (openResult) =>
                    {
                        using (StorageContainer file = storageDevice.EndOpenContainer(openResult))
                        {
                            using (Stream stream = file.CreateFile(filename))
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(ProfileList));
                                serializer.Serialize(stream, data);
                                Debug.WriteLine("Profile list saved");
                                if (ProfileDataSaved != null)
                                    ProfileDataSaved(this, EventArgs.Empty);
                            }
                            // save file
                        }
                    }
                ), null);
            }
        }

    }




    public class ProfileSaveerTest
    {
        private ProfileData data;
        enum SavingState
        {
            NotSaving,
            ReadyToSelectStorageDevice,
            SelectingStorageDevice,
            ReadyToOpenStorageContainer,
            OpeningStorageContainer,
            ReadyToSave
        }

        StorageDevice storageDevice;
        SavingState saveState = SavingState.NotSaving;
        IAsyncResult asyncResult;
        StorageContainer storageContainer;
        string filename = "savegame.sav";
        private PlayerIndex player = PlayerIndex.One;
        public ProfileSaveerTest()
        {

            data = new ProfileData()
            {
                MaxCombo = 3,
                TotalGameCount = 4
            };

        }

        public void SaveProfile()
        {
            saveState = SavingState.ReadyToOpenStorageContainer;
        }

        public void Update()
        {
            switch (saveState)
            {
                case SavingState.ReadyToSelectStorageDevice:
                    asyncResult = StorageDevice.BeginShowSelector(player, null, null);
                    saveState = SavingState.SelectingStorageDevice;
                    break;
                case SavingState.SelectingStorageDevice:
                    if (asyncResult.IsCompleted)
                    {
                        storageDevice = StorageDevice.EndShowSelector(asyncResult);
                        saveState = SavingState.ReadyToOpenStorageContainer;
                    }
                    break;
                case SavingState.ReadyToOpenStorageContainer:
                    if (storageDevice == null || !storageDevice.IsConnected)
                    {
                        saveState = SavingState.ReadyToSelectStorageDevice;
                    }
                    else
                    {
                        asyncResult = storageDevice.BeginOpenContainer("Kulki 2.0", null, null);
                        saveState = SavingState.OpeningStorageContainer;
                    }
                    break;
                case SavingState.OpeningStorageContainer:
                    if (asyncResult.IsCompleted)
                    {
                        storageContainer = storageDevice.EndOpenContainer(asyncResult);
                        saveState = SavingState.ReadyToSave;
                    }
                    break;
                case SavingState.ReadyToSave:
                    if (storageContainer == null)
                        saveState = SavingState.ReadyToOpenStorageContainer;
                    else
                    {
                        try
                        {
                            DeleteExisting();
                            Save();
                        }
                        catch (IOException e)
                        {

                            Debug.WriteLine(e.Message);
                        }
                        finally
                        {
                            storageContainer.Dispose();
                            storageContainer = null;
                            saveState = SavingState.NotSaving;
                        }
                    }
                    break;
            }
        }

        private void DeleteExisting()
        {
            if (storageContainer.FileExists(filename))
                storageContainer.DeleteFile(filename);
        }

        private void Save()
        {
            using (Stream stream = storageContainer.CreateFile(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ProfileData));
                serializer.Serialize(stream, data);
                Debug.WriteLine("Profile Saved");
            }
        }
    }

}
