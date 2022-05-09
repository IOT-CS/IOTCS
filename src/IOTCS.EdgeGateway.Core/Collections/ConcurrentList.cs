using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace IOTCS.EdgeGateway.Core.Collections
{
    public class ConcurrentList<T> : IConcurrentList<T>
    {
        private Object lockObject = new object();
        List<T> list;

        public int BinSearch(T value)
        {
            var upperBound = 0;
            var lowerBound = 0;
            var mid = 0;
            var currentIndex = -1;

            upperBound = list.Count - 1;
            while (lowerBound <= upperBound)
            {
                mid = (upperBound + lowerBound) / 2;
                currentIndex = list.IndexOf(value);
                if (mid == currentIndex)
                {
                    return mid;
                }
                else
                {
                    if (currentIndex < mid)
                    {
                        upperBound = mid - 1;
                    }
                    else
                    {
                        lowerBound = mid + 1;
                    }
                }
            }

            return -1;
        }
        
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public ConcurrentList()
        {
            list = new List<T>();
        }
       
        public ConcurrentList(IEnumerable<T> collection)
        {
            list = new List<T>(collection);
        }
        
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public ConcurrentList(int capacity)
        {
            list = new List<T>(capacity);
        }


        public void CopyTo(T[] array, int index)
        {
            lock (lockObject)
            {
                list.CopyTo(array, index);
            }          
        }

        public int Count
        {
            get { return list.Count; }
        }

        public void Clear()
        {
            lock (lockObject)
            {
                list.Clear();
            }                
        }

        public bool Contains(T value)
        {
            lock (lockObject)
            {
                return list.Contains(value);
            }                
        }

        public int IndexOf(T value)
        {
            lock (lockObject)
            {
                return list.IndexOf(value);
            }                
        }

        public void Insert(int index, T value)
        {
            lock (lockObject)
            {
                list.Insert(index, value);
            }                
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (lockObject)
            {
                return list.GetEnumerator();
            }                
        }

        public void Add(T item)
        {
            lock (lockObject)
            {
                list.Add(item);
            }                
        }

        public bool Remove(T item)
        {
            lock (lockObject)
            {
                return list.Remove(item);
            }                
        }

        public void RemoveAt(int index)
        {
            lock (lockObject)
            {
                list.RemoveAt(index);
            }                
        }

        T IList<T>.this[int index]
        {
            get
            {
                lock (lockObject)
                {
                    return list[index];
                }                    
            }
            set
            {
                lock (lockObject)
                {
                    list[index] = value;
                }                    
            }
        }

        public T Find(Predicate<T> match)
        {
            lock (lockObject)
            {
                return list.Find(match);
            }
        }

        public bool Exists(Predicate<T> match)
        {
            lock (lockObject)
            {
                return list.Exists(match);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (lockObject)
            {
                return list.GetEnumerator();
            }                
        }

        public bool IsReadOnly
        {
            get { return false; }
        }       
    }
}
