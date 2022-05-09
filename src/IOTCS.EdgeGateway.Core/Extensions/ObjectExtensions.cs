using Newtonsoft.Json;

namespace System
{

	public static class ObjectExtensions {

		/// <summary>
		/// 对象是空
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsNull(this object obj)
		{
			return obj == null;
		}

		/// <summary>
		/// 对象不为空
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsNotNull(this object obj)
		{
			return obj != null;
		}

		public static string ToStr(this object input)
		{
			return input.IsNull() ? null : input.ToString();
		}

		public static void ThrowIfNull(this object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
		}

		/// <summary>		
		/// 比较对象, 对象为null时不会抛出例外<br/>
		/// </summary>
		/// <param name="obj">Object</param>
		/// <param name="target">Target object</param>
		/// <returns></returns>		
		public static bool EqualsSupportsNull(this object obj, object target) {
			if (obj == null && target == null) {
				return true;
			} else if (obj == null || target == null) {
				return false;
			}
			return object.ReferenceEquals(obj, target) || obj.Equals(target);
		}

		/// <summary>	
		/// 转换对象到指定的类型<br/>
		/// 失败时返回默认值<br/>
		/// </summary>		
		/// <param name="obj">Object</param>
		/// <param name="defaultValue">Default value</param>
		/// <returns></returns>		
		public static T ConvertOrDefault<T>(this object obj, T defaultValue = default(T)) {
			return (T)obj.ConvertOrDefault(typeof(T), defaultValue);
		}

		/// <summary>		
		/// 转换对象到指定的类型<br/>
		/// 失败时返回默认值<br/>	
		/// </summary>
		/// <param name="obj">Object</param>
		/// <param name="type">Target type</param>
		/// <param name="defaultValue">Default value</param>
		/// <returns></returns>	
		public static object ConvertOrDefault(this object obj, Type type, object defaultValue) {			
			if (obj == null) {
				return defaultValue;
			}			
			var objType = obj.GetType();
			if (type.IsAssignableFrom(objType)) {
				return obj;
			}			
			try {
				if (objType.IsEnum && type == typeof(int)) {
					// enum => int
					return Convert.ToInt32(obj);
				} else if (objType == typeof(string) && type.IsEnum) {
					// string => enum
					return Enum.Parse(type, (string)obj);
				}
				return Convert.ChangeType(obj, type);
			} catch {
				
			}			
			if (obj is string) {
				try {
					return JsonConvert.DeserializeObject(obj as string, type);
				} catch {					
				}
			}			
			try {
				return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(obj), type);
			} catch {				
			}
			return defaultValue;
		}

		/// <summary>		
		/// 使用json序列化来克隆对象<br/>
		/// 请确保对象可以通过json.net序列化和反序列化<br/>
		/// </summary>		
		/// <param name="obj">Object</param>
		/// <returns></returns>	
		public static T CloneByJson<T>(this T obj) {
			var json = JsonConvert.SerializeObject(obj);
			var objClone = JsonConvert.DeserializeObject<T>(json);
			return objClone;
		}

		/// <summary>反射创建指定类型的实例</summary>
		/// <param name="type">类型</param>
		/// <param name="parameters">参数数组</param>
		/// <returns></returns>
		public static Object CreateInstance(this Type type, params Object[] parameters)
		{
			try
			{
				if (parameters == null || parameters.Length == 0)
				{
					// 基元类型
					switch (Type.GetTypeCode(type))
					{
						case TypeCode.Empty:
						case TypeCode.DBNull: return null;
						case TypeCode.Boolean: return false;
						case TypeCode.Char: return '\0';
						case TypeCode.SByte: return (SByte)0;
						case TypeCode.Byte: return (Byte)0;
						case TypeCode.Int16: return (Int16)0;
						case TypeCode.UInt16: return (UInt16)0;
						case TypeCode.Int32: return 0;
						case TypeCode.UInt32: return 0U;
						case TypeCode.Int64: return 0L;
						case TypeCode.UInt64: return 0UL;
						case TypeCode.Single: return 0F;
						case TypeCode.Double: return 0D;
						case TypeCode.Decimal: return 0M;
						case TypeCode.DateTime: return DateTime.MinValue;
						case TypeCode.String: return String.Empty;
					}

					return Activator.CreateInstance(type, true);
				}
				else
					return Activator.CreateInstance(type, parameters);
			}
			catch (Exception ex)
			{
				//throw new Exception("创建对象失败 type={0} parameters={1}".F(type.FullName, parameters.Join()), ex);
				throw new Exception("创建对象失败 type={0} parameters={1} {2}".F(type.FullName, parameters.Join(), ex?.Message), ex);
			}
		}
	}
}
