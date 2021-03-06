﻿using System.Collections.Generic;
using System;
using System.Linq;

namespace Barotrauma.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Randomizes the collection and returns it.
        /// </summary>
        public static IOrderedEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(i => Rand.Value());
        }

        /// <summary>
        /// Randomizes the list in place.
        /// </summary>
        public static void RandomizeList<T>(this List<T> list)
        {
            //Fisher-Yates shuffle
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rand.Int(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T GetRandom<T>(this IEnumerable<T> source, Func<T, bool> predicate, Rand.RandSync randSync = Rand.RandSync.Unsynced)
        {
            return source.Where(predicate).GetRandom(randSync);
        }

        public static T GetRandom<T>(this IEnumerable<T> source, Rand.RandSync randSync = Rand.RandSync.Unsynced)
        {
            int count = source.Count();
            return count == 0 ? default(T) : source.ElementAt(Rand.Range(0, count, randSync));
        }

        /// <summary>
        /// Executes an action that modifies the collection on each element (such as removing items from the list).
        /// Creates a temporary list.
        /// </summary>
        public static void ForEachMod<T>(this IEnumerable<T> source, Action<T> action)
        {
            var temp = new List<T>(source);
            temp.ForEach(action);
        }

        /// <summary>
        /// Generic version of List.ForEach.
        /// Performs the specified action on each element of the collection (short hand for a foreach loop).
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Shorthand for !source.Any(predicate) -> i.e. not any.
        /// </summary>
        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate = null)
        {
            if (predicate == null)
            {
                return !source.Any();
            }
            else
            {
                return !source.Any(predicate);
            }
        }

        public static bool Multiple<T>(this IEnumerable<T> source, Func<T, bool> predicate = null)
        {
            if (predicate == null)
            {
                return source.Count() > 1;
            }
            else
            {
                return source.Count(predicate) > 1;
            }
        }
        
        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }

        // source: https://stackoverflow.com/questions/19237868/get-all-children-to-one-list-recursive-c-sharp
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var result = source.SelectMany(selector);
            if (!result.Any())
            {
                return result;
            }
            return result.Concat(result.SelectManyRecursive(selector));
        }

        public static void AddIfNotNull<T>(this IList<T> source, T value)
        {
            if (value != null) { source.Add(value); }
        }
    }
}
