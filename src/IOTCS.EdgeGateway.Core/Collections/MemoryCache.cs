using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IOTCS.EdgeGateway.Core.Collections
{
	/// <summary>	
	/// 基于内存的键值缓存<br/>
	/// </summary>	
	public class MemoryCache<TKey, TValue> : IKeyValueCache<TKey, TValue> {
		/// <summary>		
		/// 删除已过期值的检查间隔<br/>
		/// 默认 180s
		/// </summary>
		public TimeSpan RevokeExpiresInterval { get; set; }
		/// <summary>		
		/// 缓存词典<br/>		
		/// </summary>
		protected ConcurrentDictionary<TKey, Pair<TValue, DateTime>> Cache { get; set; }
		/// <summary>		
		/// 上次检查的时间<br/>
		/// </summary>
		protected DateTime LastRevokeExpires { get; set; }

		public IEnumerable<TKey> SKeys => throw new NotImplementedException();

		/// <summary>
		/// 此类暂不实现
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public TValue this[TKey index]
		{ 
			get => throw new NotImplementedException(); 			
		}

		/// <summary>		
		/// 初始化<br/>
		/// </summary>
		public MemoryCache() {
			RevokeExpiresInterval = TimeSpan.FromSeconds(180);
			Cache = new ConcurrentDictionary<TKey, Pair<TValue, DateTime>>();
			LastRevokeExpires = DateTime.UtcNow;
		}

		/// <summary>		
		/// 删除已过期的值如果检查间隔已到<br/>
		/// </summary>
		protected void RevokeExpires() {
			var now = DateTime.UtcNow;
			if ((now - LastRevokeExpires) < RevokeExpiresInterval) {
				return;
			}
			if ((now - LastRevokeExpires) < RevokeExpiresInterval) {
				return; // double check
			}
			LastRevokeExpires = now;
			var expireKeys = Cache
				.Where(c => c.Value.Second < now).Select(c => c.Key)
				.ToList();
			foreach (var key in expireKeys) {
				Cache.TryRemove(key, out var _);
			}
		}

		/// <summary>		
		/// 插入值到缓存中<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <param name="keepTime">Keep time</param>
		public void Put(TKey key, TValue value, TimeSpan keepTime) {
			RevokeExpires();
			if (keepTime == TimeSpan.Zero) {
				return;
			}
			var now = DateTime.UtcNow;
			Cache[key] = Pair.Create(value, now + keepTime);
		}

		/// <summary>
		/// 尝试获取已缓存的值<br/>
		/// 如果值不存在或已过期则返回false<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <returns></returns>
		public bool TryGetValue(TKey key, out TValue value) {
			RevokeExpires();
			var now = DateTime.UtcNow;
			Pair<TValue, DateTime> pair;
			if (Cache.TryGetValue(key, out pair) && pair.Second > now) {
				value = pair.First;
				return true;
			} else {
				value = default(TValue);
				return false;
			}
		}

		/// <summary>		
		/// 删除已缓存的值<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		public void Remove(TKey key) {
			RevokeExpires();
			Cache.TryRemove(key, out var _);
		}

		/// <summary>		
		/// 获取已缓存的值数量<br/>
		/// </summary>
		/// <returns></returns>
		public int Count() {
			return Cache.Count;
		}

		/// <summary>		
		/// 删除所有已缓存的值<br/>
		/// </summary>
		public void Clear() {
			Cache.Clear();
		}

		/// <summary>
		/// 此类暂不实现
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsContainKey(TKey key)
		{
			return true;
		}
	}
}
