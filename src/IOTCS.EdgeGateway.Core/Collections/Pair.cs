using System;

namespace IOTCS.EdgeGateway.Core.Collections
{
	/// <summary>
	/// 对类型<br/>	
	/// </summary>
	public struct Pair<TFirst, TSecond> : IEquatable<Pair<TFirst, TSecond>> {
		/// <summary>		
		/// 第一个值<br/>
		/// </summary>
		public TFirst First { get; private set; }
		/// <summary>
		/// 第二个值<br/>
		/// <br/>
		/// </summary>
		public TSecond Second { get; private set; }

		/// <summary>		
		/// 初始化<br/>
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		public Pair(TFirst first, TSecond second) {
			First = first;
			Second = second;
		}

		/// <summary>		
		/// 检查是否与参数中对象相等<br/>
		/// </summary>
		/// <param name="other">Other object</param>
		/// <returns></returns>
		public bool Equals(Pair<TFirst, TSecond> other) {
			return First.EqualsSupportsNull(other.First) && Second.EqualsSupportsNull(other.Second);
		}

		/// <summary>		
		/// 检查是否与参数中的对象相等<br/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj) {
			return (obj is Pair<TFirst, TSecond>) && Equals((Pair<TFirst, TSecond>)obj);
		}

		/// <summary>		
		/// 获取哈希值<br/>
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			// same with Tuple.CombineHashCodess
			var hash_1 = First?.GetHashCode() ?? 0;
			var hash_2 = Second?.GetHashCode() ?? 0;
			return (hash_1 << 5) + hash_1 ^ hash_2;
		}

		/// <summary>		
		/// 转换到字符串<br/>
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return $"({First?.ToString() ?? "null"}, {Second.ToString() ?? "null"})";
		}

		/// <summary>		
		/// 支持解构<br/>
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		public void Deconstruct(out TFirst first, out TSecond second) {
			first = First;
			second = Second;
		}
	}

	/// <summary>	
	/// 对类型的工具函数<br/>
	/// </summary>	
	public static class Pair {
		/// <summary>		
		/// 创建对实例<br/>
		/// </summary>	
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static Pair<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second) {
			return new Pair<TFirst, TSecond>(first, second);
		}
	}
}
