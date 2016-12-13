using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Resources;
using System.Reflection;


namespace Common
{
    public class Extensions
    {
        public static string GetString(string key)
        {
            try
            {

                return new ResourceManager("Common.Resources.Global", Assembly.GetExecutingAssembly())
                .GetString(key);
                
            }
            catch (Exception)
            {

                throw;
            }
        }

       

        //public static void CopyObject(object source, object destination)
        //{
        //    var destProperties = destination.GetType().GetProperties();

        //    foreach (var sourceProperty in source.GetType().GetProperties())
        //    {
        //        foreach (var destProperty in destProperties)
        //        {
        //            if (destProperty.Name == sourceProperty.Name &&
        //        destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
        //            {
        //                destProperty.SetValue(destination, sourceProperty.GetValue(
        //                    source, new object[] { }), new object[] { });

        //                break;
        //            }
        //        }
        //    }
        //}

        public static void CopyObject(object source, object target)
        {
            CopyObjectData(source, target, String.Empty, BindingFlags.Public | BindingFlags.Instance);
        }

        private static void CopyObjectData(object source, object target, string excludedProperties, BindingFlags memberAccess)
        {
            string[] excluded = null;
            if (!string.IsNullOrEmpty(excludedProperties))
            {
                excluded = excludedProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            MemberInfo[] miT = target.GetType().GetMembers(memberAccess);
            foreach (MemberInfo Field in miT)
            {
                string name = Field.Name;

                // Skip over excluded properties
                if (string.IsNullOrEmpty(excludedProperties) == false
                    && excluded.Contains(name))
                {
                    continue;
                }


                if (Field.MemberType == MemberTypes.Field)
                {
                    FieldInfo sourcefield = source.GetType().GetField(name);
                    if (sourcefield == null) { continue; }

                    object SourceValue = sourcefield.GetValue(source);
                    ((FieldInfo)Field).SetValue(target, SourceValue);
                }
                else if (Field.MemberType == MemberTypes.Property)
                {
                    PropertyInfo piTarget = Field as PropertyInfo;
                    PropertyInfo sourceField = source.GetType().GetProperty(name, memberAccess);
                    if (sourceField == null) { continue; }

                    if (piTarget.CanWrite && sourceField.CanRead)
                    {
                        object targetValue = piTarget.GetValue(target, null);
                        object sourceValue = sourceField.GetValue(source, null);

                        if (sourceValue == null) { continue; }

                        if (sourceField.PropertyType.IsArray
                            && piTarget.PropertyType.IsArray
                            && sourceValue != null)
                        {
                            CopyArray(source, target, memberAccess, piTarget, sourceField, sourceValue);
                        }
                        else
                        {
                            CopySingleData(source, target, memberAccess, piTarget, sourceField, targetValue, sourceValue);
                        }
                    }
                }
            }
        }

        private static void CopySingleData(object source, object target, BindingFlags memberAccess, PropertyInfo piTarget, PropertyInfo sourceField, object targetValue, object sourceValue)
        {
            //instantiate target if needed
            if (targetValue == null
                && piTarget.PropertyType.IsValueType == false
                && piTarget.PropertyType != typeof(string))
            {
                if (piTarget.PropertyType.IsArray)
                {
                    targetValue = Activator.CreateInstance(piTarget.PropertyType.GetElementType());
                }
                else
                {
                    targetValue = Activator.CreateInstance(piTarget.PropertyType);
                }
            }

            if (piTarget.PropertyType.IsValueType == false
                && piTarget.PropertyType != typeof(string))
            {
                CopyObjectData(sourceValue, targetValue, "", memberAccess);
                piTarget.SetValue(target, targetValue, null);
            }
            else
            {
                if (piTarget.PropertyType.FullName == sourceField.PropertyType.FullName)
                {
                    object tempSourceValue = sourceField.GetValue(source, null);
                    piTarget.SetValue(target, tempSourceValue, null);
                }
                else
                {
                    CopyObjectData(piTarget, target, "", memberAccess);
                }
            }
        }

        private static void CopyArray(object source, object target, BindingFlags memberAccess, PropertyInfo piTarget, PropertyInfo sourceField, object sourceValue)
        {
            int sourceLength = (int)sourceValue.GetType().InvokeMember("Length", BindingFlags.GetProperty, null, sourceValue, null);
            Array targetArray = Array.CreateInstance(piTarget.PropertyType.GetElementType(), sourceLength);
            Array array = (Array)sourceField.GetValue(source, null);

            for (int i = 0; i < array.Length; i++)
            {
                object o = array.GetValue(i);
                object tempTarget = Activator.CreateInstance(piTarget.PropertyType.GetElementType());
                CopyObjectData(o, tempTarget, "", memberAccess);
                targetArray.SetValue(tempTarget, i);
            }
            piTarget.SetValue(target, targetArray, null);
        }
    }
}