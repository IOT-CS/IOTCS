using System;
using System.Collections.Generic;

namespace IOTCS.EdgeGateway.Core.Collections
{
	/// <summary>	
	/// 键值缓存的接口<br/>
	/// 它应该是线程安全的<br/>
	/// </summary>	
	public interface IKeyValueCache<TKey, TValue> {
		/// <summary>		
		/// 插入值到缓存<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <param name="keepTime">Keep time</param>
		void Put(TKey key, TValue value, TimeSpan keepTime);

		/// <summary>		
		/// 尝试获取缓存制<br/>
		/// 如果值不存在或者已过期则返回false<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <returns></returns>
		bool TryGetValue(TKey key, out TValue value);

		bool IsContainKey(TKey key);

		/// <summary>		
		/// 删除已缓存的值<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		void Remove(TKey key);

		/// <summary>		
		/// 获取缓存值的数量<br/>
		/// </summary>
		/// <returns></returns>
		int Count();

		/// <summary>		
		/// 删除所有缓存值<br/>
		/// </summary>
		void Clear();	
		
		TValue this[TKey index] { get;}

		IEnumerable<TKey> SKeys { get; }
	}
}
