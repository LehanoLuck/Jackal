using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class ListExtension
    {
        private static Random random = new Random();

        public static List<T> Clone<T>(this List<T> list) where T :ICloneable
        {
            var newList = new List<T>();
            foreach (T item in list)
            {
                newList.Add((T)item.Clone());
            }
            return newList;
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            var newList = new List<T>();

            for (int i = list.Count-1; i >=0; i--)
            {
                T item = list[random.Next(list.Count)];
                newList.Add(item);
                list.Remove(item);
            }
            return newList;
        }

        public static List<T> CreateNewRandomList<T>(this List<T> list, int count) where T : ICloneable
        {
            var newList = new List<T>();

            for (int i = 0; i < count; i++)
            {
                newList.Add(list[random.Next(list.Count)]);
            }

            return newList;
        }
    }
}