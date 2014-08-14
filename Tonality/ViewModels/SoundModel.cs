using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using Tonality.ViewModels;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Tonality.ViewModels
{
    public class SoundModel : BindableBase
    {
        public SoundGroup Walter { get; set; }
        public SoundGroup Jesse { get; set; }
        public SoundGroup Saul { get; set; }
        public SoundGroup Gus { get; set; }
        public SoundGroup Jr { get; set; }
        public SoundGroup Mike { get; set; }
        public SoundGroup Sky { get; set; }
        public SoundGroup Groups { get; set; }

        public bool IsDataLoaded { get; set; }

        public const string CustomSoundKey = "CustomSound";

        //test
        public SoundModel()
        {
            this.LoadData();
        }

        public void LoadData()
        {
            Walter = LoadFromXml("Walter.xml");
            Jesse = LoadFromXml("Jesse.xml");
            Saul = LoadFromXml("Saul.xml");
            Gus = LoadFromXml("Gus.xml");
            Jr = LoadFromXml("Jr.xml");
            Mike = LoadFromXml("Mike.xml");
            Sky = LoadFromXml("Sky.xml");



            IsDataLoaded = true;
        }

        private SoundGroup LoadFromXml(string xmlName)
        {
            SaveSoundGroup isolatedGroup = new SaveSoundGroup();
            SaveSoundGroup assetsGroup;
            XmlSerializer serializer = new XmlSerializer(typeof(SaveSoundGroup));

            using (Stream fileStream = Application.GetResourceStream(new Uri("Xmls/" + xmlName, UriKind.Relative)).Stream)
            {
                assetsGroup = (SaveSoundGroup)serializer.Deserialize(fileStream);
            }

            if (IsolatedStorageFile.GetUserStoreForApplication().FileExists(xmlName))
            {
                using (Stream fileStream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(xmlName, FileMode.Open, FileAccess.Read))
                {
                    isolatedGroup = (SaveSoundGroup)serializer.Deserialize(fileStream);
                }

                foreach (var entry in assetsGroup.Items)
                {
                    if (!isolatedGroup.Items.Contains(entry))
                    {
                        isolatedGroup.Items.Add(entry);
                    }
                }
                for (int i = 0; i < isolatedGroup.Items.Count; i++)
                {
                    if (!assetsGroup.Items.Contains(isolatedGroup.Items[i]))
                    {
                        isolatedGroup.Items.RemoveAt(i);
                        i--;
                    }
                }

            }
            else
            {
                isolatedGroup = assetsGroup;
            }

            return new SoundGroup(isolatedGroup);
        }

        private void SaveToXml(SoundGroup group, string xmlName)
        {
            if (group != null)
            {
                Stream fileStream;
                fileStream = IsolatedStorageFile.GetUserStoreForApplication().CreateFile(xmlName);

                XmlSerializer serializer = new XmlSerializer(typeof(SaveSoundGroup));
                serializer.Serialize(fileStream, new SaveSoundGroup(group));

                fileStream.Close();
                fileStream.Dispose();
            }
        }

        public void SaveData()
        {
            this.SaveToXml(Walter, "Walter.xml");
            this.SaveToXml(Jesse, "Jesse.xml");
            this.SaveToXml(Saul, "Saul.xml");
            this.SaveToXml(Gus, "Gus.xml");
            this.SaveToXml(Jr, "Jr.xml");
            this.SaveToXml(Mike, "Mike.xml");
            this.SaveToXml(Sky, "Sky.xml");
        }

        private SoundGroup LoadCustomSounds()
        {
            SoundGroup data;
            string dataFromAppSettings;

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(CustomSoundKey, out dataFromAppSettings))
            {
                data = JsonConvert.DeserializeObject<SoundGroup>(dataFromAppSettings);
            }
            else
            {
                data = new SoundGroup();
                data.Title = "mine";
            }

            return data;
        }
    }
}

