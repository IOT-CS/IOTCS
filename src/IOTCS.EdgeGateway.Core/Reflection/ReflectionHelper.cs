using System;
using System.Collections.Generic;
using System.Linq;
using System.FastReflection;

namespace System.Reflection
{
    //TODO: Consider to make internal
    public static class ReflectionHelper
    {
        static Type GetType(ref Object target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var type = target as Type;
            if (type == null)
                type = target.GetType();
            else
                target = null;

            return type;
        }

        /// <summary>获取目标对象的成员值</summary>
        /// <param name="target">目标对象</param>
        /// <param name="member">成员</param>
        /// <returns></returns>        
        public static Object GetValue(this Object target, MemberInfo member)
        {
            // 有可能跟普通的 PropertyInfo.GetValue(Object target) 搞混了
            if (member == null)
            {
                member = target as MemberInfo;
                target = null;
            }

            if (member is PropertyInfo)
                return member.FastGetValue(target);
            else if (member is FieldInfo)
            {
                member = member as FieldInfo;
                return member.FastGetValue(target);
            }
            else
                throw new ArgumentOutOfRangeException(nameof(member));
        }

        /// <summary>获取目标对象指定名称的属性/字段值</summary>
        /// <param name="target">目标对象</param>
        /// <param name="name">名称</param>
        /// <param name="value">数值</param>
        /// <returns>是否成功获取数值</returns>
        internal static Boolean TryGetValue(this Object target, String name, out Object value)
        {
            value = null;

            if (String.IsNullOrEmpty(name)) return false;

            var type = GetType(ref target);

            var mi = type.FastGetProperty(name);
            if (mi == null) return false;

            value = target.GetValue(mi);

            return true;
        }

        /// <summary>获取目标对象指定名称的属性/字段值</summary>
        /// <param name="target">目标对象</param>
        /// <param name="name">名称</param>
        /// <param name="throwOnError">出错时是否抛出异常</param>
        /// <returns></returns>      
        public static Object GetValue(this Object target, String name, Boolean throwOnError = true)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            if (TryGetValue(target, name, out var value)) return value;

            if (!throwOnError) return null;

            var type = GetType(ref target);
            throw new ArgumentException("类[" + type.FullName + "]中不存在[" + name + "]属性或字段。");
        }

        //TODO: Ehhance summary
        /// <summary>
        /// Checks whether <paramref name="givenType"/> implements/inherits <paramref name="genericType"/>.
        /// </summary>
        /// <param name="givenType">Type to check</param>
        /// <param name="genericType">Generic type</param>
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenTypeInfo.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenTypeInfo.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
        }

        //TODO: Summary
        public static List<Type> GetImplementedGenericTypes(Type givenType, Type genericType)
        {
            var result = new List<Type>();
            AddImplementedGenericTypes(result, givenType, genericType);
            return result;
        }

        private static void AddImplementedGenericTypes(List<Type> result, Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                result.Add(givenType);
            }

            foreach (var interfaceType in givenTypeInfo.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    result.Add(interfaceType);
                }
            }

            if (givenTypeInfo.BaseType == null)
            {
                return;
            }

            AddImplementedGenericTypes(result, givenTypeInfo.BaseType, genericType);
        }

        /// <summary>
        /// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        /// Returns default value if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="defaultValue">Default value (null as default)</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
        public static TAttribute GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default, bool inherit = true)
            where TAttribute : Attribute
        {
            //Get attribute on the member
            if (memberInfo.IsDefined(typeof(TAttribute), inherit))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
            }

            return defaultValue;
        }

        /// <summary>
        /// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        /// Returns default value if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="defaultValue">Default value (null as default)</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
        public static TAttribute GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default, bool inherit = true)
            where TAttribute : class
        {
            return memberInfo.GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
                   ?? memberInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
                   ?? defaultValue;
        }

        /// <summary>
        /// Gets value of a property by it's full path from given object
        /// </summary>
        public static object GetValueByPath(object obj, Type objectType, string propertyPath)
        {
            var value = obj;
            var currentType = objectType;
            var objectPath = currentType.FullName;
            var absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
            }

            foreach (var propertyName in absolutePropertyPath.Split('.'))
            {
                var property = currentType.GetProperty(propertyName);
                value = property.GetValue(value, null);
                currentType = property.PropertyType;
            }

            return value;
        }

        /// <summary>
        /// Sets value of a property by it's full path on given object
        /// </summary>
        internal static void SetValueByPath(object obj, Type objectType, string propertyPath, object value)
        {
            var currentType = objectType;
            PropertyInfo property;
            var objectPath = currentType.FullName;
            var absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
            }

            var properties = absolutePropertyPath.Split('.');

            if (properties.Length == 1)
            {
                property = objectType.GetProperty(properties.First());
                property.SetValue(obj, value);
                return;
            }

            for (int i = 0; i < properties.Length - 1; i++)
            {
                property = currentType.GetProperty(properties[i]);
                obj = property.GetValue(obj, null);
                currentType = property.PropertyType;
            }

            property = currentType.GetProperty(properties.Last());
            property.SetValue(obj, value);
        }


        /// <summary>
        /// Get all the constant values in the specified type (including the base type).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string[] GetPublicConstantsRecursively(Type type)
        {
            const int maxRecursiveParameterValidationDepth = 8;

            var publicConstants = new List<string>();

            void Recursively(List<string> constants, Type targetType, int currentDepth)
            {
                if (currentDepth > maxRecursiveParameterValidationDepth)
                {
                    return;
                }

                constants.AddRange(targetType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(x => x.IsLiteral && !x.IsInitOnly)
                    .Select(x => x.GetValue(null).ToString()));

                var nestedTypes = targetType.GetNestedTypes(BindingFlags.Public);

                foreach (var nestedType in nestedTypes)
                {
                    Recursively(constants, nestedType, currentDepth + 1);
                }
            }

            Recursively(publicConstants, type, 1);

            return publicConstants.ToArray();
        }
    }
}
