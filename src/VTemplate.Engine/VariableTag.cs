/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  VariableTag
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;

namespace VTemplate.Engine
{
    /// <summary>
    /// 变量元素.如变量: {$:member.name} 或带前缀与属性的变量: {$:#.member.name htmlencode='true'}
    /// </summary>
    public class VariableTag : Element, IAttributesElement
    {
        /// <summary>
        /// 带变量字段的初始化
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <param name="varExp"></param>
        internal VariableTag(Template ownerTemplate, VariableExpression varExp)
            : base(ownerTemplate)
        {
            //注册添加属性时触发事件.用于设置自身的某些属性值
            this.Attributes = new AttributeCollection();
            this.Attributes.Adding += OnAddingAttribute;
            this.VarExpression = varExp;
            this.Charset = ownerTemplate.Charset;
            this.callFunctions = new List<string>();
        }

        #region 属性定义
        /// <summary>
        /// 此标签的属性集合
        /// </summary>
        public AttributeCollection Attributes { get; protected set; }

        /// <summary>
        /// 变量元素中的变量表达式
        /// </summary>
        public VariableExpression VarExpression { get; private set; }

        /// <summary>
        /// 是否需要对输出数据进行HTML数据格式化
        /// </summary>
        public bool HtmlEncode { get; protected set; }

        /// <summary>
        /// 是否需要对输出数据进行JS脚本格式化
        /// </summary>
        public bool JsEncode { get; protected set; }

        /// <summary>
        /// 是否需要对输出数据进行XML数据格式化
        /// </summary>
        public bool XmlEncode { get; protected set; }

        /// <summary>
        /// 是否需要对输出数据进行URL地址编码
        /// </summary>
        public bool UrlEncode { get; protected set; }

        /// <summary>
        /// 是否需要对输出数据进行文本数据格式化(先HtmlEncode格式化,再将"空格"转换为"&amp;nbsp;", "\n"转换为"&lt;br /&gt;")
        /// </summary>
        public bool TextEncode { get; protected set; }

        /// <summary>
        /// 是否需要对输出数据进行文本压缩(删除换行符和换行符前后的空格)
        /// </summary>
        public bool CompressText { get; protected set; }

        /// <summary>
        /// 数据的输出长度
        /// </summary>
        public int Length { get; protected set; }

        /// <summary>
        /// 附加文本(此属性只能配合Length属性使用.即当文本有被裁剪时才附加此文本)
        /// </summary>
        public string AppendText { get; protected set; }

        /// <summary>
        /// 数据的编码
        /// </summary>
        public Encoding Charset { get; protected set; }

        /// <summary>
        /// 数据输出时的格式化表达式
        /// </summary>
        public string Format { get; protected set; }

        /// <summary>
        /// 数据输出时是否删除HTML代码
        /// </summary>
        public bool RemoveHtml { get; protected set; }

        /// <summary>
        /// 要调用的函数列表
        /// </summary>
        private List<string> callFunctions { get; set; }
        /// <summary>
        /// 要调用的函数列表
        /// </summary>
        public string[] CallFunctions
        {
            get
            {
                return this.callFunctions.ToArray();
            }
        }
        #endregion

        #region 添加标签属性时的触发事件函数.用于设置自身的某些属性值
        /// <summary>
        /// 添加标签属性时的触发事件函数.用于设置自身的某些属性值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddingAttribute(object sender, AttributeCollection.AttributeAddingEventArgs e)
        {
            switch (e.Item.Name.ToLower())
            {
                case "length":
                    //显示字符串的长度
                    this.Length = Utility.ConverToInt32(e.Item.Value);
                    break;
                case "format":
                    //数据格式化表达式
                    this.Format = e.Item.Value;
                    break;
                case "htmlencode":
                    //数据输出时是否进行HTML编码
                    this.HtmlEncode = Utility.ConverToBoolean(e.Item.Value);
                    break;
                case "jsencode":
                    //数据输出时是否进行Js脚本编码
                    this.JsEncode = Utility.ConverToBoolean(e.Item.Value);
                    break;
                case "xmlencode":
                    //数据输出时是否进行Xml字符编码
                    this.XmlEncode = Utility.ConverToBoolean(e.Item.Value);
                    break;
                case "textencode":
                    //数据输出时是否进行文本字符编码
                    this.TextEncode = Utility.ConverToBoolean(e.Item.Value);
                    break;
                case "urlencode":
                    //数据输出时是否进行URL地址编码
                    this.UrlEncode = Utility.ConverToBoolean(e.Item.Value);
                    break;
                case "compresstext":
                    //是否需要对输出数据进行文本压缩(删除换行符和换行符前后的空格)
                    this.CompressText = Utility.ConverToBoolean(e.Item.Value);
                    break;
                case "charset":
                    //进行URL编码时使用的文本编码
                    this.Charset = Utility.GetEncodingFromCharset(e.Item.Value, this.OwnerTemplate.Charset);
                    break;
                case "appendtext":
                    //附加文本
                    this.AppendText = e.Item.Value;
                    break;
                case "removehtml":
                    //是否在输出数据时删除其中的HTML代码
                    this.RemoveHtml = Utility.ConverToBoolean(e.Item.Value);
                    break;
                case "call":
                    //要调用的方法
                    string method = e.Item.Value.Trim();
                    if (!string.IsNullOrEmpty(method))
                    {
                        if (!this.callFunctions.Exists(x => x.Equals(method, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            this.callFunctions.Add(method);
                        }
                    }
                    break;
            }
        }
        #endregion

        #region 呈现本元素的数据
        /// <summary>
        /// 呈现本元素的数据
        /// </summary>
        /// <param name="writer"></param>
        public override void Render(System.IO.TextWriter writer)
        {
            VariableTag tag = this;
            object value = this.VarExpression.GetValue();

            //调用自定函数处理此变量的值
            if (this.callFunctions.Count > 0)
            {
                foreach (string method in this.callFunctions)
                {
                    UserDefinedFunction func;
                    if (this.OwnerTemplate.UserDefinedFunctions.TryGetValue(method, out func))
                    {
                        value = func(new object[] { value });
                    }
                }
            }

            if (Utility.IsNothing(value)) return;

            bool formated = false;
            string text = string.Empty;
            if (value is string)
            {
                //字符串
                text = (string)value;
            }
            else
            {
                //非字符串.则判断处理format
                if (!string.IsNullOrEmpty(this.Format))
                {
                    IFormattable iformat = value as IFormattable;
                    if (iformat != null)
                    {
                        text = iformat.ToString(this.Format, CultureInfo.InvariantCulture);
                        formated = true;
                    }
                }
                //非IFormattable接口.则直接取字符串处理
                if (!formated)
                {
                    //如果数据是IEnumerable,IEnumerator接口则进行数据拼凑
                    IEnumerator ie = null;
                    if (value is IEnumerable)
                    {
                        ie = ((IEnumerable)value).GetEnumerator();
                    }
                    else if (value is IEnumerator)
                    {
                        ie = (IEnumerator)value;
                    }
                    if (ie != null)
                    {
                        StringBuilder buffer = new StringBuilder();
                        ie.Reset();
                        while (ie.MoveNext())
                        {
                            if (buffer.Length != 0) buffer.Append(",");
                            buffer.Append(ie.Current);
                        }
                        text = buffer.ToString();
                    }
                    else
                    {
                        text = value.ToString();
                    }
                }
            }

            if (text.Length > 0)
            {
                if (this.RemoveHtml) text = Utility.RemoveHtmlCode(text);
                if (this.Length > 0) text = Utility.CutString(text, this.Length, this.Charset, this.AppendText);
                if (this.TextEncode)
                {
                    text = Utility.TextEncode(text);
                }
                else if (this.HtmlEncode)
                {
                    text = HttpUtility.HtmlEncode(text);
                }
                if (this.XmlEncode) text = Utility.XmlEncode(text);
                if (this.JsEncode) text = Utility.JsEncode(text);
                if (this.UrlEncode) text = HttpUtility.UrlEncode(text, this.Charset);
                if (this.CompressText) text = Utility.CompressText(text);

                if (!formated && !string.IsNullOrEmpty(this.Format))
                {
                    text = string.Format(this.Format, text);
                }
                

                writer.Write(text);
            }
        }
        #endregion

        #region 输出变量元素的原字符数据
        /// <summary>
        /// 输出变量元素的原字符串数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("{$:");
            buffer.Append(this.VarExpression.ToString());
            foreach (Attribute attribute in this.Attributes)
            {
                buffer.AppendFormat(" {0}=\"{1}\"", attribute.Name, HttpUtility.HtmlEncode(attribute.Value));
            }
            buffer.Append("}");
            return buffer.ToString();
        }
        #endregion

        #region 克隆当前元素到新的宿主模板
        /// <summary>
        /// 克隆当前元素到新的宿主模板
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <returns></returns>
        internal override Element Clone(Template ownerTemplate)
        {
            VariableTag tag = new VariableTag(ownerTemplate, (VariableExpression)this.VarExpression.Clone(ownerTemplate));
            tag.Attributes = this.Attributes;
            tag.Charset = this.Charset;
            tag.Format = this.Format;
            tag.HtmlEncode = this.HtmlEncode;
            tag.JsEncode = this.JsEncode;
            tag.Length = this.Length;
            tag.TextEncode = this.TextEncode;
            tag.UrlEncode = this.UrlEncode;
            tag.XmlEncode = this.XmlEncode;
            tag.CompressText = this.CompressText;
            tag.AppendText = this.AppendText;
            tag.RemoveHtml = this.RemoveHtml;
            tag.callFunctions = this.callFunctions;
            return tag;
        }
        #endregion
    }
}
