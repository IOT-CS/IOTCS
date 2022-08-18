namespace IOTCS.EdgeGateway.CmdHandler
{
    public static class DataObject2DataObject
    {
        public static T ToModel<K, T>(this K m) where T : new()
        {
            T t = (default(T) == null) ? System.Activator.CreateInstance<T>() : default(T);
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties();
            System.Reflection.PropertyInfo[] properties2 = m.GetType().GetProperties();
            System.Reflection.PropertyInfo[] array = properties;
            for (int i = 0; i < array.Length; i++)
            {
                System.Reflection.PropertyInfo propertyInfo = array[i];
                System.Reflection.PropertyInfo[] array2 = properties2;
                for (int j = 0; j < array2.Length; j++)
                {
                    System.Reflection.PropertyInfo propertyInfo2 = array2[j];
                    if (propertyInfo2.Name.ToLower() == propertyInfo.Name.ToLower())
                    {
                        propertyInfo.SetValue(t, propertyInfo2.GetValue(m, null), null);
                        break;
                    }
                }
            }
            return t;
        }

        public static T ToModel<K, T>(this K m, T q) where T : new()
        {
            System.Reflection.PropertyInfo[] properties = q.GetType().GetProperties();
            System.Reflection.PropertyInfo[] properties2 = m.GetType().GetProperties();
            System.Reflection.PropertyInfo[] array = properties;
            for (int i = 0; i < array.Length; i++)
            {
                System.Reflection.PropertyInfo propertyInfo = array[i];
                System.Reflection.PropertyInfo[] array2 = properties2;
                for (int j = 0; j < array2.Length; j++)
                {
                    System.Reflection.PropertyInfo propertyInfo2 = array2[j];
                    if (propertyInfo2.Name.ToLower() == propertyInfo.Name.ToLower() && propertyInfo.GetValue(q, null) != null && propertyInfo2.GetValue(m, null) != null && propertyInfo.GetValue(q, null).ToString() != propertyInfo2.GetValue(m, null).ToString())
                    {
                        propertyInfo.SetValue(q, propertyInfo2.GetValue(m, null), null);
                        break;
                    }
                }
            }
            return q;
        }

        private static System.Type GetCoreType(System.Type t)
        {
            if (!(t != null) || !DataObject2DataObject.IsNullable(t))
            {
                return t;
            }
            if (!t.IsValueType)
            {
                return t;
            }
            return System.Nullable.GetUnderlyingType(t);
        }

        private static bool IsNullable(System.Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(System.Nullable<>));
        }
    }
}
