#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  public class ItemList<T> : ItemBase, IList<T> where T : IItem {
    private List<T> list;

    public ItemList() {
      list = new List<T>();
    }

    public override IView CreateView() {
      return new ItemListView<T>(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ItemList<T> clone = new ItemList<T>();
      clonedObjects.Add(Guid, clone);
      CloneElements(clone, clonedObjects);
      return clone;
    }
    protected void CloneElements(ItemList<T> destination, IDictionary<Guid, object> clonedObjects) {
      for (int i = 0; i < list.Count; i++)
        destination.list.Add((T) Auxiliary.Clone(list[i], clonedObjects));
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      for (int i = 0; i < list.Count; i++)
        node.AppendChild(PersistenceManager.Persist(list[i], document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      for (int i = 0; i < node.ChildNodes.Count; i++)
        list.Add((T) PersistenceManager.Restore(node.ChildNodes[i], restoredObjects));
    }

    public override string ToString() {
      if (list.Count > 0) {
        StringBuilder builder = new StringBuilder();
        builder.Append(list[0].ToString());
        for (int i = 1; i < list.Count; i++) {
          builder.Append(";");
          builder.Append(list[i].ToString());
        }
        return builder.ToString();
      } else {
        return "Empty List";
      }
    }

    #region IList<T> Members
    public int IndexOf(T item) {
      return list.IndexOf(item);
    }
    public void Insert(int index, T item) {
      list.Insert(index, item);
      OnItemAdded(item, index);
    }
    public void RemoveAt(int index) {
      IItem item = list[index];
      list.RemoveAt(index);
      OnItemRemoved(item, index);
    }
    public T this[int index] {
      get { return list[index]; }
      set { list[index] = value; }
    }
    #endregion

    #region ICollection<T> Members
    public void Add(T item) {
      list.Add(item);
      OnItemAdded(item, list.Count - 1);
    }
    public void Clear() {
      list.Clear();
      OnCleared();
    }
    public bool Contains(T item) {
      return list.Contains(item);
    }
    public void CopyTo(T[] array, int arrayIndex) {
      list.CopyTo(array, arrayIndex);
    }
    public int Count {
      get { return list.Count; }
    }
    public bool IsReadOnly {
      get { return false; }
    }
    public bool Remove(T item) {
      int index = list.IndexOf(item);
      if (list.Remove(item)) {
        OnItemRemoved(item, index);
        return true;
      } else {
        return false;
      }
    }
    #endregion

    #region IEnumerable<T> Members
    public IEnumerator<T> GetEnumerator() {
      return list.GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator() {
      return list.GetEnumerator();
    }
    #endregion

    #region List<T> Methods
    public int LastIndexOf(T item) {
      return list.LastIndexOf(item);
    }

    public int LastIndexOf(T item, int index) {
      return list.LastIndexOf(item, index);
    }

    public int LastIndexOf(T item, int index, int count) {
      return list.LastIndexOf(item, index, count);
    }

    public int IndexOf(T item, int index) {
      return list.IndexOf(item, index);
    }

    public int IndexOf(T item, int index, int count) {
      return list.IndexOf(item, index, count);
    }

    public void AddRange(IEnumerable<T> collection) {
      foreach (T obj in collection) {
        this.Add(obj);
      }
    }

    public bool Exists(Predicate<T> match) {
      return list.Exists(match);
    }

    public int BinarySearch(T item) {
      return list.BinarySearch(item);
    }

    public int BinarySearch(T item, IComparer<T> comparer) {
      return list.BinarySearch(item, comparer);
    }

    public int BinarySearch(int index, int count, T item, IComparer<T> comparer) {
      return list.BinarySearch(index, count, item, comparer);
    }

    public T Find(Predicate<T> match) {
      return list.Find(match);
    }

    public List<T> FindAll(Predicate<T> match) {
      return list.FindAll(match);
    }

    public int FindIndex(Predicate<T> match) {
      return list.FindIndex(match);
    }

    public T FindLast(Predicate<T> match) {
      return list.FindLast(match);
    }

    public int FindLastIndex(Predicate<T> match) {
      return list.FindLastIndex(match);
    }

    public void Sort() {
      list.Sort();
    }

    public void Sort(IComparer<T> comparer) {
      list.Sort(comparer);
    }

    public void Sort(Comparison<T> comparison) {
      list.Sort(comparison);
    }

    public void Reverse() {
      list.Reverse();
    }

    public ItemList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) where TOutput : IItem {
      ItemList<TOutput> targetList = new ItemList<TOutput>();
      foreach (T item in list) {
        targetList.Add(converter.Invoke(item));
      }
      return targetList;
    }

    public bool TrueForAll(Predicate<T> match) {
      return list.TrueForAll(match);
    }

    #endregion

    public event EventHandler<ItemIndexEventArgs> ItemAdded;
    protected virtual void OnItemAdded(IItem item, int index) {
      if (ItemAdded != null)
        ItemAdded(this, new ItemIndexEventArgs(item, index));
      OnChanged();
    }
    public event EventHandler<ItemIndexEventArgs> ItemRemoved;
    protected virtual void OnItemRemoved(IItem item, int index) {
      if (ItemRemoved != null)
        ItemRemoved(this, new ItemIndexEventArgs(item, index));
      OnChanged();
    }
    public event EventHandler Cleared;
    protected virtual void OnCleared() {
      if (Cleared != null)
        Cleared(this, new EventArgs());
      OnChanged();
    }
  }
}
