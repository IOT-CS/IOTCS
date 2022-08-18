using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.FastReflection
{
    /// <summary>
    /// Delegate use to invoke any method
    /// </summary>
    public delegate object MethodInvoker(object instance, object[] parameters);
    /// <summary>
    /// Delegate use to set get property or field
    /// </summary>
    public delegate object MemberGetter(object instance);
    /// <summary>
    /// Delegate use to set any property or field
    /// </summary>
    public delegate void MemberSetter(object instance, object value);

    /// <summary>
    /// Extension methods for reflection
    /// </summary>
    public static class ReflectionExtensions
    {
        // Caches
        private static readonly ConcurrentDictionary<MethodInfo, MethodInvoker>
            MethodInvokerCache = new ConcurrentDictionary<MethodInfo, MethodInvoker>();
        private static readonly ConcurrentDictionary<MemberInfo, MemberSetter>
            MemberSetterCache = new ConcurrentDictionary<MemberInfo, MemberSetter>();
        private static readonly ConcurrentDictionary<MemberInfo, MemberGetter>
            MemberGetterCache = new ConcurrentDictionary<MemberInfo, MemberGetter>();
        private static readonly ConcurrentDictionary<StructTuple<Type, BindingFlags>, PropertyInfo[]>
            PropertiesCache = new ConcurrentDictionary<StructTuple<Type, BindingFlags>, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<StructTuple<Type, BindingFlags>, FieldInfo[]>
            FieldsCache = new ConcurrentDictionary<StructTuple<Type, BindingFlags>, FieldInfo[]>();
        private static readonly ConcurrentDictionary<StructTuple<Type, BindingFlags>, MethodInfo[]>
            MethodsCache = new ConcurrentDictionary<StructTuple<Type, BindingFlags>, MethodInfo[]>();
        private static readonly ConcurrentDictionary<StructTuple<MemberInfo, Type>, object[]>
            AttributesCache = new ConcurrentDictionary<StructTuple<MemberInfo, Type>, object[]>();

        /// <summary>
        /// Create dynamic delegate from method info
        /// </summary>
        public static MethodInvoker MakeInvoker(MethodInfo methodInfo)
        {
            // Target: ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                BinaryExpression valueObj = Expression.ArrayIndex(
                    parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(
                    valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = methodInfo.IsStatic ? null :
                Expression.Convert(instanceParameter, methodInfo.DeclaringType);

            // static invoke or ((TInstance)instance).Method
            var methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);

            // ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
            if (methodCall.Type == typeof(void))
            {
                var lambda = Expression.Lambda<Action<object, object[]>>(
                        methodCall, instanceParameter, parametersParameter);

                Action<object, object[]> execute = lambda.Compile();
                return (instance, parameters) =>
                {
                    execute(instance, parameters);
                    return null;
                };
            }
            else
            {
                var castMethodCall = Expression.Convert(methodCall, typeof(object));
                var lambda = Expression.Lambda<MethodInvoker>(
                    castMethodCall, instanceParameter, parametersParameter);

                return lambda.Compile();
            }
        }
        
        public static MemberSetter MakeSetter(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo)
            {
                // target: ((TInstance)instance).Property = (TPropertyType)value
                var propertyInfo = (PropertyInfo)memberInfo;
                if (!propertyInfo.CanWrite)
                    throw new NotSupportedException("Set method is not defined for this property.");

                // preparing parameter, object type
                var instance = Expression.Parameter(typeof(object), "instance");
                var value = Expression.Parameter(typeof(object), "value");

                // non-instance for static method, or ((TInstance)instance)
                var setMethod = propertyInfo.GetSetMethod(true);
                var instanceCast = setMethod.IsStatic ? null :
                    Expression.Convert(instance, propertyInfo.DeclaringType);

                // ((TInstance)instance).Property
                var propertyAccess = Expression.Property(instanceCast, propertyInfo);

                // (TPropertyType)value
                var castValue = Expression.Convert(value, propertyInfo.PropertyType);

                // assign expression
                var assign = Expression.Assign(propertyAccess, castValue);

                // Lambda expression
                var lambda = Expression.Lambda<MemberSetter>(assign, instance, value);
                return lambda.Compile();
            }
            else if (memberInfo is FieldInfo)
            {
                // target: ((TInstance)instance).Field = (TFieldType)value
                var fieldInfo = (FieldInfo)memberInfo;

                // preparing parameter, object type
                var instance = Expression.Parameter(typeof(object), "instance");
                var value = Expression.Parameter(typeof(object), "value");

                // non-instance for static method, or ((TInstance)instance)
                var instanceCast = fieldInfo.IsStatic ? null :
                    Expression.Convert(instance, fieldInfo.DeclaringType);

                // ((TInstance)instance).Field
                var fieldAccess = Expression.Field(instanceCast, fieldInfo);

                // (TFieldType)value
                var castValue = Expression.Convert(value, fieldInfo.FieldType);

                // assign expression
                var assign = Expression.Assign(fieldAccess, castValue);

                // Lambda expression
                var lambda = Expression.Lambda<MemberSetter>(assign, instance, value);
                return lambda.Compile();
            }
            else
            {
                throw new ArgumentException("Member isn't property or field");
            }
        }

        /// <summary>
        /// Create setter delegate from member info, support property and field
        /// </summary>
        public static MemberGetter MakeGetter(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo)
            {
                // Target: (object)(((TInstance)instance).Property)
                var propertyInfo = (PropertyInfo)memberInfo;
                if (!propertyInfo.CanRead)
                    throw new NotSupportedException("Get method is not defined for this property.");

                // preparing parameter, object type
                var instance = Expression.Parameter(typeof(object), "instance");

                // non-instance for static method, or ((TInstance)instance)
                var getMethod = propertyInfo.GetGetMethod(true);
                var instanceCast = getMethod.IsStatic ? null :
                    Expression.Convert(instance, propertyInfo.DeclaringType);

                // ((TInstance)instance).Property
                var propertyAccess = Expression.Property(instanceCast, propertyInfo);

                // (object)(((TInstance)instance).Property)
                var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));

                // Lambda expression
                var lambda = Expression.Lambda<MemberGetter>(castPropertyValue, instance);
                return lambda.Compile();
            }
            else if (memberInfo is FieldInfo)
            {
                // target: (object)(((TInstance)instance).Field)
                var fieldInfo = (FieldInfo)memberInfo;

                // preparing parameter, object type
                var instance = Expression.Parameter(typeof(object), "instance");

                // non-instance for static method, or ((TInstance)instance)
                var instanceCast = fieldInfo.IsStatic ? null :
                    Expression.Convert(instance, fieldInfo.DeclaringType);

                // ((TInstance)instance).Field
                var fieldAccess = Expression.Field(instanceCast, fieldInfo);

                // (object)(((TInstance)instance).Property)
                var castFieldValue = Expression.Convert(fieldAccess, typeof(object));

                // Lambda expression
                var lambda = Expression.Lambda<MemberGetter>(castFieldValue, instance);
                return lambda.Compile();
            }
            else
            {
                throw new ArgumentException("Member isn't property or field");
            }
        }

        /// <summary>
        /// Same as MethodInfo.Invoke, but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters)
        {
            var invoker = MethodInvokerCache.GetOrAdd(methodInfo, m => MakeInvoker(m));
            return invoker(instance, parameters);
        }

        /// <summary>
        /// Same as PropertyInfo.SetValue or FieldInfo.SetValue, but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastSetValue(this MemberInfo memberInfo, object instance, object value)
        {
            var setter = MemberSetterCache.GetOrAdd(memberInfo, m => MakeSetter(m));
            setter(instance, value);
        }

        /// <summary>
        /// Same as PropertyInfo.GetValue or FieldInfo.GetValue, but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object FastGetValue(this MemberInfo memberInfo, object instance)
        {
            var getter = MemberGetterCache.GetOrAdd(memberInfo, m => MakeGetter(m));
            return getter(instance);
        }

        /// <summary>
        /// Same as Type.GetProperties(bindFlags), but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo[] FastGetProperties(this Type type, BindingFlags bindingFlags)
        {
            return PropertiesCache.GetOrAdd(
                new StructTuple<Type, BindingFlags>(type, bindingFlags),
                key => key.First.GetTypeInfo().GetProperties(key.Second));
        }

        /// <summary>
        /// Same as Type.GetProperties(), but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo[] FastGetProperties(this Type type)
        {
            return type.FastGetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// Same as Type.GetFields(bindFlags), but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo[] FastGetFields(this Type type, BindingFlags bindingFlags)
        {
            return FieldsCache.GetOrAdd(
                new StructTuple<Type, BindingFlags>(type, bindingFlags),
                key => key.First.GetTypeInfo().GetFields(key.Second));
        }

        /// <summary>
        /// Same as Type.GetFields(), but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo[] FastGetFields(this Type type)
        {
            return type.FastGetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// Same as Type.GetMethods(bindingFlags), but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo[] FastGetMethods(this Type type, BindingFlags bindingFlags)
        {
            return MethodsCache.GetOrAdd(
                new StructTuple<Type, BindingFlags>(type, bindingFlags),
                key => key.First.GetTypeInfo().GetMethods(key.Second));
        }

        /// <summary>
        /// Same as Type.GetMethods(), but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo[] FastGetMethods(this Type type)
        {
            return type.FastGetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// Same as GetCustomAttributes(attributeType, false), but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] FastGetCustomAttributes(this MemberInfo memberInfo, Type attributeType)
        {
            return AttributesCache.GetOrAdd(
                new StructTuple<MemberInfo, Type>(memberInfo, attributeType),
                key => key.First.GetCustomAttributes(attributeType, false).ToArray<object>());
        }

        /// <summary>
        /// Same as GetCustomAttributes(attributeType, inherit), but work faster
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] FastGetCustomAttributes(this MemberInfo memberInfo, Type attributeType, bool inherit)
        {
            return AttributesCache.GetOrAdd(
                new StructTuple<MemberInfo, Type>(memberInfo, attributeType),
                key => key.First.GetCustomAttributes(attributeType, inherit).ToArray<object>());
        }

        /// <summary>
        /// Same as Type.GetInterfaces()
        /// Original implemenation is enough fast, here just call the original method
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] FastGetInterfaces(this Type type)
        {
            return type.GetInterfaces();
        }

        /// <summary>
        /// Same as Type.GetProperty(name, bindFlags)
        /// Original implemenation is enough fast, here just call the original method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo FastGetProperty(this Type type, string name, BindingFlags bindingFlags)
        {
            return type.GetProperty(name, bindingFlags);
        }

        /// <summary>
        /// Same as Type.GetProperty(name)
        /// Original implemenation is enough fast, here just call the original method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo FastGetProperty(this Type type, string name)
        {
            return type.GetProperty(name);
        }

        /// <summary>
        /// Same as Type.GetField(name, bindFlags)
        /// Original implemenation is enough fast, here just call the original method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo FastGetField(this Type type, string name, BindingFlags bindingFlags)
        {
            return type.GetField(name, bindingFlags);
        }

        /// <summary>
        /// Same as Type.GetField(name)
        /// Original implemenation is enough fast, here just call the original method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo FastGetField(this Type type, string name)
        {
            return type.GetField(name);
        }

        /// <summary>
        /// Same as Type.GetMethod(name, bindFlags)
        /// Original implemenation is enough fast, here just call the original method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo FastGetMethod(this Type type, string name, BindingFlags bindingFlags)
        {
            return type.GetMethod(name, bindingFlags);
        }

        /// <summary>
        /// Same as Type.GetMethod(name)
        /// Original implemenation is enough fast, here just call the original method
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo FastGetMethod(this Type type, string name)
        {
            return type.GetMethod(name);
        }

        /// <summary>
        /// Clear cached informations
        /// </summary>
        public static void ClearCache()
        {
            MethodInvokerCache.Clear();
            MemberSetterCache.Clear();
            MemberGetterCache.Clear();
            PropertiesCache.Clear();
            FieldsCache.Clear();
            MethodsCache.Clear();
            AttributesCache.Clear();
        }
    }
}
