﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Collections {
  [Serializable]
  [StorableClass(StorableClassType.MarkedOnly)]
  public class ReadOnlyObservableList<T> : IObservableList<T> {
    [Storable]
    private IObservableList<T> list;

    #region persistence
    private ReadOnlyObservableList() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void PostDeserizlationHook() {
      RegisterEvents();
    }
    #endregion

    #region Properties
    public int Count {
      get { return ((ICollection<T>)list).Count; }
    }
    bool ICollection<T>.IsReadOnly {
      get { return true; }
    }

    public T this[int index] {
      get { return list[index]; }
    }
    T IList<T>.this[int index] {
      get { return list[index]; }
      set { throw new NotSupportedException(); }
    }
    #endregion

    #region Constructors
    public ReadOnlyObservableList(IObservableList<T> list) {
      if (list == null) throw new ArgumentNullException();
      this.list = list;
      RegisterEvents();
    }

    private void RegisterEvents() {
      list.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<T>>(list_ItemsAdded);
      ((IObservableCollection<T>)list).ItemsAdded += new CollectionItemsChangedEventHandler<T>(list_ItemsAdded);
      list.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(list_ItemsRemoved);
      ((IObservableCollection<T>)list).ItemsRemoved += new CollectionItemsChangedEventHandler<T>(list_ItemsRemoved);
      list.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<T>>(list_ItemsReplaced);
      list.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(list_ItemsMoved);
      list.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<T>>(list_CollectionReset);
      ((IObservableCollection<T>)list).CollectionReset += new CollectionItemsChangedEventHandler<T>(list_CollectionReset);
      list.PropertyChanged += new PropertyChangedEventHandler(list_PropertyChanged);
    }
    #endregion

    #region Access
    public bool Contains(T item) {
      return list.Contains(item);
    }

    public int IndexOf(T item) {
      return list.IndexOf(item);
    }
    #endregion

    #region Manipulation
    void ICollection<T>.Add(T item) {
      throw new NotSupportedException();
    }

    void IList<T>.Insert(int index, T item) {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Remove(T item) {
      throw new NotSupportedException();
    }
    void IList<T>.RemoveAt(int index) {
      throw new NotSupportedException();
    }

    void ICollection<T>.Clear() {
      throw new NotSupportedException();
    }
    #endregion

    #region Conversion
    public void CopyTo(T[] array, int arrayIndex) {
      list.CopyTo(array, arrayIndex);
    }
    #endregion

    #region Enumeration
    public IEnumerator<T> GetEnumerator() {
      return ((ICollection<T>)list).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)list).GetEnumerator();
    }
    #endregion

    #region Events
    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> ItemsAdded;
    protected virtual void OnItemsAdded(IEnumerable<IndexedItem<T>> items) {
      if (ItemsAdded != null)
        ItemsAdded(this, new CollectionItemsChangedEventArgs<IndexedItem<T>>(items));
    }

    [field: NonSerialized]
    private event CollectionItemsChangedEventHandler<T> itemsAdded;
    event CollectionItemsChangedEventHandler<T> INotifyObservableCollectionItemsChanged<T>.ItemsAdded {
      add { itemsAdded += value; }
      remove { itemsAdded -= value; }
    }
    private void OnItemsAdded(IEnumerable<T> items) {
      if (itemsAdded != null)
        itemsAdded(this, new CollectionItemsChangedEventArgs<T>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> ItemsRemoved;
    protected virtual void OnItemsRemoved(IEnumerable<IndexedItem<T>> items) {
      if (ItemsRemoved != null)
        ItemsRemoved(this, new CollectionItemsChangedEventArgs<IndexedItem<T>>(items));
    }

    [field: NonSerialized]
    private event CollectionItemsChangedEventHandler<T> itemsRemoved;
    event CollectionItemsChangedEventHandler<T> INotifyObservableCollectionItemsChanged<T>.ItemsRemoved {
      add { itemsRemoved += value; }
      remove { itemsRemoved -= value; }
    }
    private void OnItemsRemoved(IEnumerable<T> items) {
      if (itemsRemoved != null)
        itemsRemoved(this, new CollectionItemsChangedEventArgs<T>(items));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> ItemsReplaced;
    protected virtual void OnItemsReplaced(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      if (ItemsReplaced != null)
        ItemsReplaced(this, new CollectionItemsChangedEventArgs<IndexedItem<T>>(items, oldItems));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> ItemsMoved;
    protected virtual void OnItemsMoved(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      if (ItemsMoved != null)
        ItemsMoved(this, new CollectionItemsChangedEventArgs<IndexedItem<T>>(items, oldItems));
    }

    [field: NonSerialized]
    public event CollectionItemsChangedEventHandler<IndexedItem<T>> CollectionReset;
    protected virtual void OnCollectionReset(IEnumerable<IndexedItem<T>> items, IEnumerable<IndexedItem<T>> oldItems) {
      if (CollectionReset != null)
        CollectionReset(this, new CollectionItemsChangedEventArgs<IndexedItem<T>>(items, oldItems));
    }

    [field: NonSerialized]
    private event CollectionItemsChangedEventHandler<T> collectionReset;
    event CollectionItemsChangedEventHandler<T> INotifyObservableCollectionItemsChanged<T>.CollectionReset {
      add { collectionReset += value; }
      remove { collectionReset -= value; }
    }
    private void OnCollectionReset(IEnumerable<T> items, IEnumerable<T> oldItems) {
      if (collectionReset != null)
        collectionReset(this, new CollectionItemsChangedEventArgs<T>(items, oldItems));
    }

    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private void list_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      OnItemsAdded(e.Items);
    }
    private void list_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) {
      OnItemsAdded(e.Items);
    }
    private void list_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      OnItemsRemoved(e.Items);
    }
    private void list_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      OnItemsRemoved(e.Items);
    }
    private void list_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      OnItemsReplaced(e.Items, e.OldItems);
    }
    private void list_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      OnItemsMoved(e.Items, e.OldItems);
    }
    private void list_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      OnCollectionReset(e.Items, e.OldItems);
    }
    private void list_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      OnCollectionReset(e.Items, e.OldItems);
    }
    private void list_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName.Equals("Item[]") || e.PropertyName.Equals("Count"))
        OnPropertyChanged(e.PropertyName);
    }
    #endregion
  }
}
