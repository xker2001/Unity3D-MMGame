using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMGame
{
    public static class ListExtension
    {
        /// <summary>
        /// 插入元素到一个已排序的 List 并排序。
        /// 代码参考：
        /// http://stackoverflow.com/questions/12172162/how-to-insert-item-into-list-in-order
        /// </summary>
        /// <param name="self">List。</param>
        /// <param name="item">待插入的元素。</param>
        /// <returns>新元素的索引值。</returns>
        public static int AddSorted<T>(this List<T> self, T item) where T : IComparable<T>
        {
            if (self.Count == 0)
            {
                self.Add(item);
                return 0;
            }

            if (self[self.Count - 1].CompareTo(item) <= 0)
            {
                self.Add(item);
                return self.Count - 1;
            }

            if (self[0].CompareTo(item) >= 0)
            {
                self.Insert(0, item);
                return 0;
            }

            int index = self.BinarySearch(item);

            if (index < 0)
                index = ~index;

            self.Insert(index, item);
            return index;
        }

        /// <summary>
        /// 插入元素到一个已排序的 List 并排序。
        /// </summary>
        /// <param name="self">List。</param>
        /// <param name="item">待插入的元素。</param>
        /// <param name="comparer">比较器实例。</param>
        /// <returns>新元素的索引值。</returns>
        public static int AddSorted<T>(this List<T> self, T item, IComparer<T> comparer)
        {
            if (self.Count == 0)
            {
                self.Add(item);
                return 0;
            }

            if (comparer.Compare(self[self.Count - 1], item) <= 0)
            {
                self.Add(item);
                return self.Count - 1;
            }

            if (comparer.Compare(self[0], item) >= 0)
            {
                self.Insert(0, item);
                return 0;
            }

            int index = self.BinarySearch(item, comparer);

            if (index < 0)
                index = ~index;

            self.Insert(index, item);
            return index;
        }

        /// <summary>
        /// 将 List 乱序重排。
        /// </summary>
        /// <param name="self">List。</param>
        public static void Shuffle<T>(this List<T> self)
        {
            for (int i = 0; i < self.Count; i++)
            {
                T temp = self[i];
                int randomIndex = UnityEngine.Random.Range(0, self.Count);
                self[i] = self[randomIndex];
                self[randomIndex] = temp;
            }
        }
    }
}