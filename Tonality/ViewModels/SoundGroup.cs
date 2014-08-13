using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tonality.ViewModels
{
    public class SoundGroup
    {
        public string Title { get; set; }
        public List<Group<SoundData>> Items { get; set; }

        public SoundGroup()
        {
            Items = new List<Group<SoundData>>();
        }

        public SoundGroup(SaveSoundGroup data)
        {
            this.Title = data.Title;
            this.Items = new List<Group<SoundData>>();

            foreach (var group in data.Items.GroupBy(item => item.Groups))
            {
                this.Items.Add(new Group<SoundData>(group.Key, group.ToList()));
            }
        }
    }

    public class SaveSoundGroup
    {
        public string Title { get; set; }
        public List<SoundData> Items { get; set; }

        public SaveSoundGroup()
        {
            this.Title = string.Empty;
            this.Items = new List<SoundData>();
        }

        public SaveSoundGroup(SoundGroup data)
            : this()
        {
            if (data != null)
            {
                this.Title = data.Title;
                this.Items = new List<SoundData>();

                foreach (var group in data.Items)
                {
                    this.Items.AddRange(group);
                }
            }
        }
    }
}
