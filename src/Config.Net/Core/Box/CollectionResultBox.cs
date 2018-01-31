using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Config.Net.Core.Box
{
   class CollectionResultBox : ResultBox
   {
      private readonly ResultBox _elementResultBox;
      private string _basePath;
      private DynamicReader _reader;

      public CollectionResultBox(string name, ResultBox elementBox) : base(name, elementBox.ResultType, null)
      {
         _elementResultBox = elementBox;
      }

      public ResultBox ElementResultBox => _elementResultBox;

      public bool IsInitialised { get; private set; }

      public IEnumerable CollectionInstance { get; private set; }

      public void Initialise(string basePath, int length, DynamicReader reader)
      {
         _basePath = basePath;
         _reader = reader;

         CollectionInstance = CreateGenericEnumerable(length);

         IsInitialised = true;
      }

      private IEnumerable CreateGenericEnumerable(int count)
      {
         Type t = typeof(DynamicEnumerable<>);
         t = t.MakeGenericType(ResultType);

         IEnumerable instance = (IEnumerable)Activator.CreateInstance(t, count, this);

         return instance;
      }

      private object ReadAt(int index)
      {
         return _reader.Read(ElementResultBox, index);
      }

      private class DynamicEnumerable<T> : IEnumerable<T>
      {
         private readonly int _count;
         private readonly CollectionResultBox _parent;

         public DynamicEnumerable(int count, CollectionResultBox parent)
         {
            _count = count;
            _parent = parent;
         }

         public IEnumerator<T> GetEnumerator()
         {
            return new DynamicEnumerator<T>(_count, _parent);
         }

         IEnumerator IEnumerable.GetEnumerator()
         {
            return new DynamicEnumerator<T>(_count, _parent);
         }
      }

      private class DynamicEnumerator<T> : IEnumerator<T>
      {
         private int _index = -1;
         private readonly int _count;
         private readonly CollectionResultBox _parent;

         public DynamicEnumerator(int count, CollectionResultBox parent)
         {
            _count = count;
            _parent = parent;
         }

         public T Current { get; private set; }

         object IEnumerator.Current => (T)Current;

         public void Dispose()
         {
         }

         public bool MoveNext()
         {
            _index += 1;

            Current = (T)_parent.ReadAt(_index);

            return _index < _count;
         }

         public void Reset()
         {
            _index = -1;
         }
      }
   }
}
