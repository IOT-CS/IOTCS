using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IOTCS.EdgeGateway.Core.Collections
{
    public class SystemKeyValue<TKey, TValue> : IKeyValueCache<TKey, TValue>
    {
        private readonly object lockObject = new object();
        private ConcurrentDictionary<TKey, TValue> _keyValuePairs = new ConcurrentDictionary<TKey, TValue>();

        public IEnumerable<TKey> SKeys => _keyValuePairs.Keys;

        public void Put(TKey key, TValue value, TimeSpan keepTime)
        {
            lock (lockObject)
            {
                if (!_keyValuePairs.ContainsKey(key))
                {
                    _keyValuePairs.TryAdd(key, value);
                }
            }            
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            _keyValuePairs.TryGetValue(key, out value);

            return true;
        }

        public TValue this[TKey index]
        {
            get 
            {
                if (_keyValuePairs.ContainsKey(index))
                {
                    return _keyValuePairs[index];
                }
                else
                {
                    return default(TValue);
                }
            }            
        }

        public int Count()
        {
            return _keyValuePairs.Count;
        }

        public void Remove(TKey key)
        {
            lock (lockObject)
            {
                var value = default(TValue);
                if (_keyValuePairs.ContainsKey(key))
                {
                    _keyValuePairs.TryRemove(key, out value);
                }
            }            
        }

        public void Clear()
        {
            _keyValuePairs.Clear();
        }

        public bool IsContainKey(TKey key)
        {
            if (_keyValuePairs.ContainsKey(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
