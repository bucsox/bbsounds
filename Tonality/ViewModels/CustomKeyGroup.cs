using System.Collections.Generic;


namespace Tonality
{
    //public static List<Group<T>> GetItemGroups<T>(IEnumerable<T> itemList, Func<T, string> getKeyFunc)
    //{
    //    IEnumerable<Group<T>> groupList = from item in itemList
    //                                      group item by getKeyFunc(item) into g
    //                                      orderby g.Key
    //                                      select new Group<T>(g.Key, g);

    //    return groupList.ToList();
    //}


    public class Group<T> : List<T>
    {
        public string Name
        {
            get;
            set;
        }

        public Group()
            : base()
        {
            this.Name = string.Empty;
        }

        public Group(string name, IEnumerable<T> items)
            : base(items)
        {
            this.Name = name;
        }
    }
}
