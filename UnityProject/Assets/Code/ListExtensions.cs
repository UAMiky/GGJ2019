using System;
using System.Collections.Generic;

public static class ListExtensions
{
   #region Generic array extensions

   public static IList<T> Swap<T> (this IList<T> list, int indexA, int indexB)
   {
      T tmp = list [indexA];
      list [indexA] = list [indexB];
      list [indexB] = tmp;
      return list;
   }

   public static IList<T> Shuffle<T> (this IList<T> list)
   {
      for (int i = 0, n = list.Count - 1; i < n; i++)
      {
         Swap (list, i, UnityEngine.Random.Range (i, list.Count));
      }
      return list;
   }


   public static IList<T> Reverse<T> (this IList<T> list)
   {
      int nLast = list.Count - 1;

      for (int i = 0, n = UnityEngine.Mathf.FloorToInt (list.Count / 2f); i < n; i++)
      {
         Swap (list, i, nLast - i);
      }
      return list;
   }

   #endregion
}
