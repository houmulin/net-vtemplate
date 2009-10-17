﻿/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  Utility
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;

namespace VTemplate.Engine
{
    /// <summary>
    /// 实用类
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// 
        /// </summary>
        static Utility()
        {
            RenderInstanceCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            DbFactoriesCache = new Dictionary<string, DbProviderFactory>(StringComparer.InvariantCultureIgnoreCase);
            TypeCache = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
        }

        #region 数据判断函数块
        /// <summary>
        /// 判断是否是空数据(null或DBNull)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsNothing(object value)
        {
            return value == null || value == DBNull.Value;
        }
        /// <summary>
        /// 判断是否是整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsInteger(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            if (value[0] != '-' && !char.IsDigit(value[0])) return false;

            int i;
            return int.TryParse(value, out i);
        }
        #endregion

        #region 数据格式化函数块
        /// <summary>
        /// XML编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string XmlEncode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter))
                    {
                        xmlWriter.WriteString(value);
                        xmlWriter.Flush();
                        value = stringWriter.ToString();
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// 文本编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string TextEncode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = HttpUtility.HtmlEncode(value);
                value = value.Replace(" ", "&nbsp;");
                value = value.Replace("\t", "&nbsp;&nbsp;");
                value = Regex.Replace(value, "\r\n|\r|\n", "<br />");
            }
            return value;
        }

        /// <summary>
        /// JS脚本编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string JsEncode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("\\", "\\\\");
                value = value.Replace("\"", "\\\"");
                value = value.Replace("\'", "\\'");
                value = value.Replace("\r", "\\r");
                value = value.Replace("\n", "\\n");
            }
            return value;
        }

        /// <summary>
        /// 压缩文本
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string CompressText(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = Regex.Replace(value, @"[ \t]*\r?\n[ \t]*", "");
            }
            return value;
        }
        #endregion

        #region 数据转换函数块
        /// <summary>
        /// 转换字符串为布尔值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool ConverToBoolean(string value)
        {
            if (value == "1" || string.Equals(value, Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 转换字符串为整型值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static int ConverToInt32(string value)
        {
            int v;
            if (!int.TryParse(value, out v))
            {
                v = 0;
            }
            return v;
        }

        /// <summary>
        /// 转换字符串为数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static decimal ConverToDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return 0M;
            decimal v;
            try
            {
                v = Convert.ToDecimal(value);
            }
            catch
            {
                v = 0M;
            }
            return v;
        }

        /// <summary>
        /// 截取字符
        /// </summary>
        /// <param name="value">要截取的字符串</param>
        /// <param name="maxLength">最大大小</param>
        /// <param name="charset">采用的编码</param>
        /// <param name="appendText">附加字符</param>
        /// <returns></returns>
        internal static string CutString(string value, int maxLength, Encoding charset, string appendText)
        {
            StringBuilder buffer = new StringBuilder(maxLength);
            int length = 0;
            int index = 0;
            while (index < value.Length)
            {
                char c = value[index];
                length += charset.GetByteCount(new char[] { c });
                if (length <= maxLength)
                {
                    buffer.Append(c);
                }
                else
                {
                    break;
                }
                index++;
            }
            if (index < value.Length && !string.IsNullOrEmpty(appendText)) buffer.Append(appendText);
            return buffer.ToString();
        }

        /// <summary>
        /// 从字符集名称获取编码器
        /// </summary>
        /// <param name="charset"></param>
        /// <returns></returns>
        internal static Encoding GetEncodingFromCharset(string charset)
        {
            Encoding e = Encoding.Default;
            try
            {
                e = Encoding.GetEncoding(charset);
            }
            catch
            {
                e = Encoding.Default;
            }
            return e;
        }

        /// <summary>
        /// 转换为某种数据类型
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <param name="type">最终的数据类型</param>
        /// <returns>如果转换失败返回null</returns>
        internal static object ConvertTo(string value, Type type)
        {
            object result = value;
            if (value != null)
            {
                try
                {
                    if (type.IsEnum)
                    {
                        //枚举类型
                        result = Enum.Parse(type, value, true);
                    }
                    else if (Type.GetTypeCode(type) == TypeCode.DateTime)
                    {
                        //日期型
                        result = DateTime.Parse(value);
                    }
                    else
                    {
                        //其它值
                        result = (value as IConvertible).ToType(type, null);
                    }
                }
                catch
                {
                    result = null;
                }
            }
            return result;
        }
        #endregion

        #region 数据源处理函数块
        /// <summary>
        /// 获取某个对象对应的DbType
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static DbType GetObjectDbType(object value)
        {
            if (value == null) return DbType.Object;
            switch (Type.GetTypeCode(value is Type ? (Type)value : value.GetType()))
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Char:
                case TypeCode.String:
                    return DbType.String;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.SByte:
                    return DbType.SByte;
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.UInt16:
                    return DbType.UInt16;
                case TypeCode.UInt32:
                    return DbType.UInt32;
                case TypeCode.UInt64:
                    return DbType.UInt64;
                default:
                    return DbType.Object;
            }
        }
        /// <summary>
        /// 获取某个属性的值
        /// </summary>
        /// <param name="container">数据源</param>
        /// <param name="propName">属性名</param>
        /// <param name="exist">是否存在此属性</param>
        /// <returns>属性值</returns>
        internal static object GetPropertyValue(object container, string propName, out bool exist)
        {
            exist = false;
            object value = null;
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            if (string.IsNullOrEmpty(propName))
            {
                throw new ArgumentNullException("propName");
            }
            if (Utility.IsInteger(propName))
            {
                #region 索引值部分
                //属性名只为数字.则取数组索引
                int index = Utility.ConverToInt32(propName);
                if (container is IList)
                {
                    IList iList = (IList)container;
                    if (iList.Count > index)
                    {
                        exist = true;
                        value = iList[index];
                    }
                }
                else if (container is ICollection)
                {
                    ICollection ic = (ICollection)container;
                    if (ic.Count > index)
                    {
                        exist = true;
                        IEnumerator ie = ic.GetEnumerator();
                        int i = 0;
                        while (i++ <= index) { ie.MoveNext(); }
                        value = ie.Current;
                    }
                }
                else
                {
                    //判断是否含有索引属性
                    PropertyInfo item = container.GetType().GetProperty("Item", new Type[] { typeof(int) });
                    if (item != null)
                    {
                        try
                        {
                            value = item.GetValue(container, new object[] { index });
                            exist = true;
                        }
                        catch
                        {
                            exist = false;
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region 字段/属性/键值
                //容器是类型.则查找静态属性或字段
                Type type = container is Type ? (Type)container : container.GetType();
                BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;
                if (!(container is Type)) flags |= BindingFlags.Instance;

                //查找字段
                FieldInfo field = type.GetField(propName, flags);
                if (field != null)
                {
                    exist = true;
                    value = field.GetValue(container);
                }
                else
                {
                    //查找属性
                    PropertyInfo property = type.GetProperty(propName, flags, null, null, Type.EmptyTypes, new ParameterModifier[0]);
                    if (property != null)
                    {
                        exist = true;
                        value = property.GetValue(container, null);
                    }
                    else if (container is ICustomTypeDescriptor)
                    {
                        //已实现ICustomTypeDescriptor接口
                        ICustomTypeDescriptor ictd = (ICustomTypeDescriptor)container;
                        PropertyDescriptor descriptor = ictd.GetProperties().Find(propName, true);
                        if (descriptor != null)
                        {
                            exist = true;
                            value = descriptor.GetValue(container);
                        }
                    }
                    else if (container is IDictionary)
                    {
                        //是IDictionary集合
                        IDictionary idic = (IDictionary)container;
                        if (idic.Contains(propName))
                        {
                            exist = true;
                            value = idic[propName];
                        }
                    }
                    else if (container is NameObjectCollectionBase)
                    {
                        //是NameObjectCollectionBase派生对象
                        NameObjectCollectionBase nob = (NameObjectCollectionBase)container;

                        //调用私有方法
                        MethodInfo method = nob.GetType().GetMethod("BaseGet", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, new ParameterModifier[] { new ParameterModifier(1) });
                        if (method != null)
                        {
                            value = method.Invoke(container, new object[] { propName });
                            exist = value == null;
                        }
                    }
                    else
                    {
                        //判断是否含有索引属性
                        PropertyInfo item = type.GetProperty("Item", new Type[] { typeof(string) });
                        if (item != null)
                        {
                            try
                            {
                                value = item.GetValue(container, new object[] { propName });
                                exist = true;
                            }
                            catch
                            {
                                exist = false;
                            }
                        }
                    }
                }
                #endregion
            }
            return value;
        }
        /// <summary>
        /// 获取方法的结果值
        /// </summary>
        /// <param name="container"></param>
        /// <param name="methodName"></param>
        /// <param name="exist"></param>
        /// <returns></returns>
        internal static object GetMethodResult(object container, string methodName, out bool exist)
        {
            exist = false;
            Type type = (container is Type ? (Type)container : container.GetType());
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance |
                                                           BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase,
                                                                          null, Type.EmptyTypes, new ParameterModifier[0]);
            object result = null;
            if (method != null)
            {
                exist = true;
                return method.Invoke(method.IsStatic ? null : container, null);
            }
            return result;
        }

        /// <summary>
        /// 返回数据源的枚举数
        /// </summary>
        /// <param name="dataSource">要处理的数据源</param>
        /// <returns>如果非IListSource与IEnumerable实例则返回null</returns>
        internal static IEnumerable GetResolvedDataSource(object dataSource)
        {
            if (dataSource != null)
            {
                if (dataSource is IListSource)
                {
                    IListSource source = (IListSource)dataSource;
                    IList list = source.GetList();
                    if (!source.ContainsListCollection)
                    {
                        return list;
                    }
                    if ((list != null) && (list is ITypedList))
                    {
                        PropertyDescriptorCollection itemProperties = ((ITypedList)list).GetItemProperties(new PropertyDescriptor[0]);
                        if ((itemProperties == null) || (itemProperties.Count == 0))
                        {
                            return null;
                        }
                        PropertyDescriptor descriptor = itemProperties[0];
                        if (descriptor != null)
                        {
                            object component = list[0];
                            object value = descriptor.GetValue(component);
                            if ((value != null) && (value is IEnumerable))
                            {
                                return (IEnumerable)value;
                            }
                        }
                        return null;
                    }
                }
                if (dataSource is IEnumerable)
                {
                    return (IEnumerable)dataSource;
                }
            }
            return null;
        }
        #endregion

        #region 模版引擎相关辅助函数块
        /// <summary>
        /// 修正文件地址
        /// </summary>
        /// <param name="template"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static string ResolveFilePath(Template template, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && fileName.IndexOf(":") == -1 && !fileName.StartsWith("\\\\"))
            {
                string referPath = string.Empty;
                while (string.IsNullOrEmpty(referPath) && template != null)
                {
                    referPath = template.File;
                    template = template.OwnerTemplate;
                }
                if (!string.IsNullOrEmpty(referPath))
                {
                    fileName = Path.Combine(Path.GetDirectoryName(referPath), fileName);
                }
                fileName = Path.GetFullPath(fileName);
            }
            return fileName;
        }
        /// <summary>
        /// 统计行号与列号(x = 列号, y = 行号)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static Point GetLineAndColumnNumber(string text, int offset)
        {
            int line, column, p;
            line = column = 1;
            p = 0;
            while (p < offset && p < text.Length)
            {
                char c = text[p];
                if (c == '\r' || c == '\n')
                {
                    if (c == '\r' && p < (text.Length - 1))
                    {
                        //\r\n字符
                        if (text[p + 1] == '\n') p++;
                    }
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
                p++;
            }
            return new Point(column, line);
        }
        /// <summary>
        /// 从模版中获取某个变量.如果不存在此变量则添加新的变量
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="varName"></param>
        /// <returns></returns>
        internal static Variable GetVariableOrAddNew(Template ownerTemplate, string varName)
        {
            Variable var = ownerTemplate.Variables[varName];
            if (var == null)
            {
                var = new Variable(ownerTemplate, varName);
                ownerTemplate.Variables.Add(var);
            }
            return var;
        }

        /// <summary>
        /// 根据前缀获取变量的模版所有者
        /// </summary>
        /// <param name="template"></param>
        /// <param name="prefix"></param>
        /// <returns>如果prefix值为null则返回template的根模版.如果为空值.则为template.如果为#则返回template的父模版.否则返回对应Id的模版</returns>
        internal static Template GetVariableTemplateByPrefix(Template template, string prefix)
        {
            if (prefix == string.Empty) return template;               //前缀为空.则返回当前模版
            if (prefix == "#") return template.OwnerTemplate ?? template;   //前缀为#.则返回父模版(如果父模版不存在则返回当前模版)

            //取得根模版
            while (template.OwnerTemplate != null) template = template.OwnerTemplate;

            //如果没有前缀.则返回根模版.否则返回对应Id的模版
            return prefix == null ? template : template.GetChildTemplateById(prefix);
        }
        #endregion

        #region 模版数据解析相关辅助函数块
        /// <summary>
        /// 类型的缓存
        /// </summary>
        private static Dictionary<string, Type> TypeCache;
        /// <summary>
        /// 建立某个类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal static Type CreateType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;

            Type type;
            bool flag = false;
            lock (TypeCache)
            {
                flag = TypeCache.TryGetValue(typeName, out type);
            }
            if (!flag)
            {
                type = Type.GetType(typeName, false, true);
                if (type == null)
                {
                    //搜索当前程序域里的所有程序集
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly assembly in assemblies)
                    {
                        type = assembly.GetType(typeName, false, true);
                        if (type != null) break;
                    }
                }
                //缓存
                lock (TypeCache)
                {
                    if (!TypeCache.ContainsKey(typeName))
                    {
                        TypeCache.Add(typeName, type);
                    }
                }
            }
            return type;
        }
        /// <summary>
        /// 存储模版解析器实例的缓存
        /// </summary>
        private static Dictionary<string, object> RenderInstanceCache;
        /// <summary>
        /// 获取解析器的实例
        /// </summary>
        /// <param name="renderInstance"></param>
        /// <returns></returns>
        private static object GetRenderInstance(string renderInstance)
        {
            if (string.IsNullOrEmpty(renderInstance)) return null;

            string[] k = renderInstance.Split(new char[] { ',' }, 2);
            if (k.Length != 2) return null;

            string assemblyKey = k[1].Trim();
            string typeKey = k[0].Trim();
            string cacheKey = string.Concat(typeKey, ",", assemblyKey);

            //从缓存读取
            object render;
            bool flag = false;
            lock (RenderInstanceCache)
            {
                flag = RenderInstanceCache.TryGetValue(cacheKey, out render);
            }
            if (!flag || render == null)
            {
                //重新生成实例
                render = null;
                //生成实例
                Assembly assembly;
                if (assemblyKey.IndexOf(":") != -1)
                {
                    assembly = Assembly.LoadFrom(assemblyKey);
                }
                else
                {
                    assembly = Assembly.Load(assemblyKey);
                }
                if (assembly != null)
                {
                    render = assembly.CreateInstance(typeKey, false);
                }
                if (render != null)
                {
                    //缓存
                    lock (RenderInstanceCache)
                    {
                        if (RenderInstanceCache.ContainsKey(cacheKey))
                        {
                            RenderInstanceCache[cacheKey] = render;
                        }
                        else
                        {
                            RenderInstanceCache.Add(cacheKey, render);
                        }
                    }
                }
            }
            return render;
        }

        /// <summary>
        /// 预解析模版数据
        /// </summary>
        /// <param name="renderInstance">模版解析器实例的配置</param>
        /// <param name="template">要解析处理的模版</param>
        internal static void PreRenderTemplate(string renderInstance, Template template)
        {
            ITemplateRender render = GetRenderInstance(renderInstance) as ITemplateRender;
            if (render != null) render.PreRender(template);
        }
        /// <summary>
        /// 使用特性方法预解析模版数据
        /// </summary>
        /// <param name="renderInstance"></param>
        /// <param name="renderMethod"></param>
        /// <param name="template"></param>
        internal static void PreRenderTemplateByAttributeMethod(string renderInstance, string renderMethod, Template template)
        {
            object render = GetRenderInstance(renderInstance);
            if (render != null)
            {
                //取得特性方法
                MethodInfo method = null;

                MethodInfo[] methods = render.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (MethodInfo m in methods)
                {
                    TemplateRenderMethodAttribute att = System.Attribute.GetCustomAttribute(m, typeof(TemplateRenderMethodAttribute)) as TemplateRenderMethodAttribute;
                    if (att != null)
                    {
                        if (renderMethod.Equals(m.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            method = m;
                            break;
                        }
                    }
                }

                if (method != null)
                {
                    ParameterInfo[] pars = method.GetParameters();
                    if (pars.Length == 1 && pars[0].ParameterType == typeof(Template))
                    {
                        method.Invoke(method.IsStatic ? null : render, new object[] { template });
                    }
                }
            }
        }


        /// <summary>
        /// 数据驱动工厂实例的缓存
        /// </summary>
        private static Dictionary<string, DbProviderFactory> DbFactoriesCache;

        /// <summary>
        /// 建立数据驱动工厂
        /// </summary>
        /// <param name="providerName"></param>
        internal static DbProviderFactory CreateDbProviderFactory(string providerName)
        {
            if(string.IsNullOrEmpty(providerName)) return null;

            //从缓存读取
            DbProviderFactory factory;
            bool flag = false;
            lock (DbFactoriesCache)
            {
                flag = DbFactoriesCache.TryGetValue(providerName, out factory);
            }
            if (!flag || factory == null)
            {
                factory = DbProviderFactories.GetFactory(providerName);
                //缓存
                if (factory != null)
                {
                    lock (DbFactoriesCache)
                    {
                        if (DbFactoriesCache.ContainsKey(providerName))
                        {
                            DbFactoriesCache[providerName] = factory;
                        }
                        else
                        {
                            DbFactoriesCache.Add(providerName, factory);
                        }
                    }
                }
            }
            return factory;
        }
        #endregion
    }
}
