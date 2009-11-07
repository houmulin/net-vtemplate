/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  Template
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VTemplate.Engine
{
    /// <summary>
    /// 模版块标签.如: &lt;vt:template id="member"&gt;.......&lt;/vt:template&gt; 或自闭合的模版:&lt;vt:template id="member" file="member.html" /&gt;
    /// </summary>
    public class Template : Tag
    {
        /// <summary>
        /// 
        /// </summary>
        internal Template() : this(null) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTemplate"></param>
        internal Template(Template ownerTemplate)
            : base(ownerTemplate)
        {
            this.Visible = true;
            this.Charset = (ownerTemplate == null ? Encoding.Default : ownerTemplate.Charset);
            this.Variables = new VariableCollection();
            this.ChildTemplates = new ElementCollection<Template>();
            this.fileDependencies = new List<string>();
            this.UserDefinedFunctions = new UserDefinedFunctionCollection();
            this.TagContainer = this;
        }

        #region 重写Tag的方法
        /// <summary>
        /// 返回标签的名称
        /// </summary>
        public override string TagName
        {
            get { return "template"; }
        }

        /// <summary>
        /// 返回此标签是否是单一标签.即是不需要配对的结束标签
        /// </summary>
        internal override bool IsSingleTag
        {
            get { return false; }
        }
        #endregion

        #region 属性定义
        /// <summary>
        /// 模版的关联文件
        /// </summary>
        public string File { get; internal set; }

        /// <summary>
        /// 模版数据采用的编码
        /// </summary>
        public Encoding Charset { get; internal set; }

        /// <summary>
        /// 设置或返回此模版是否可见
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 返回此模版下的变量集合
        /// </summary>
        public VariableCollection Variables { get; private set; }

        /// <summary>
        /// 自定义函数集合
        /// </summary>
        public UserDefinedFunctionCollection UserDefinedFunctions { get; private set; }

        /// <summary>
        /// 返回此模版下的子模版元素
        /// </summary>
        public ElementCollection<Template> ChildTemplates { get; set; }

        /// <summary>
        /// 标签容器
        /// </summary>
        protected Template TagContainer { get; set; }

        /// <summary>
        /// 返回处理模版数据的实例
        /// </summary>
        public string RenderInstance { get; protected set; }

        /// <summary>
        /// 返回处理模版数据的特性方法
        /// </summary>
        public string RenderMethod { get; protected set; }
        #endregion

        #region 模版的依赖文件
        /// <summary>
        /// 模版的依赖文件列表
        /// </summary>
        protected List<string> fileDependencies;
        /// <summary>
        /// 返回模版的依赖文件
        /// </summary>
        public string[] FileDependencies
        {
            get
            {
                return this.fileDependencies.ToArray();
            }
        }
        /// <summary>
        /// 添加模版的依赖文件
        /// </summary>
        /// <param name="fileName"></param>
        internal void AddFileDependency(string fileName)
        {
            foreach (string item in this.fileDependencies)
            {
                if (item.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)) return;
            }
            this.fileDependencies.Add(fileName);
            if (this.OwnerTemplate != null) this.OwnerTemplate.AddFileDependency(this.File);
        }
        #endregion

        #region 添加标签属性时的触发函数.用于设置自身的某些属性值
        /// <summary>
        /// 添加标签属性时的触发函数.用于设置自身的某些属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        protected override void OnAddingAttribute(string name, Attribute item)
        {
            switch (name)
            {
                case "file":
                    this.File = Utility.ResolveFilePath(this.OwnerTemplate, item.Value);
                    this.AddFileDependency(this.File);                    
                    break;
                case "charset":
                    this.Charset = Utility.GetEncodingFromCharset(item.Value, this.OwnerTemplate.Charset);
                    break;
                case "render":
                    this.RenderInstance = item.Value.Trim();
                    break;
                case "rendermethod":
                    this.RenderMethod = item.Value.Trim();
                    break;
            }
        }
        #endregion

        #region 注册全局的自定义函数
        /// <summary>
        /// 注册全局的自定义函数
        /// </summary>
        /// <param name="function">函数</param>
        public void RegisterGlobalFunction(UserDefinedFunction function)
        {
            this.RegisterGlobalFunction(function.Method.Name, function);
        }
        /// <summary>
        /// 注册全局的自定义函数
        /// </summary>
        /// <param name="functionName">函数名称</param>
        /// <param name="function">函数</param>
        public void RegisterGlobalFunction(string functionName, UserDefinedFunction function)
        {
            if (string.IsNullOrEmpty(functionName)) throw new ArgumentNullException("functionName");
            if (function == null) throw new ArgumentNullException("function");

            TagContainer.UserDefinedFunctions.Add(functionName, function);
            foreach (Template child in TagContainer.ChildTemplates)
            {
                child.RegisterGlobalFunction(functionName, function);
            }
        }
        #endregion

        #region 子模版函数
        /// <summary>
        /// 获取某个Id的子模版.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Template GetChildTemplateById(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            foreach (Template template in TagContainer.ChildTemplates)
            {
                if (id.Equals(template.Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    return template;
                }
                else
                {
                    Template t = template.GetChildTemplateById(id);
                    if (t != null) return t;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取所有具有同名称的模版列表.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ElementCollection<Template> GetChildTemplatesByName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            ElementCollection<Template> items = new ElementCollection<Template>();
            foreach (Template template in TagContainer.ChildTemplates)
            {
                if (name.Equals(template.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    items.Add(template);
                }
                items.AddRange(template.GetChildTemplatesByName(name));
            }
            return items;
        }
        #endregion

        #region 设置变量的值
        /// <summary>
        /// 设置变量的值,此操作是对所有模版(包括子模版)下的同名称变量有效
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="varValue"></param>
        public void SetValue(string varName, object varValue)
        {
            TagContainer.Variables.SetValue(varName, varValue);
            foreach (Template child in TagContainer.ChildTemplates)
            {
                child.SetValue(varName, varValue);
            }
        }
        /// <summary>
        /// 设置变量表达式的值,此操作是对所有模版(包括子模版)下的同名称变量有效
        /// </summary>
        /// <param name="exp">变量表达式.如"user.age"则表示设置user变量的age属性/字段值</param>
        /// <param name="value">表达式的值</param>
        public void SetExpValue(string exp, object value)
        {
            TagContainer.Variables.SetExpValue(exp, value);
            foreach (Template child in TagContainer.ChildTemplates)
            {
                child.SetExpValue(exp, value);
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
            //优先将处理权交给RenderInstance
            if (!string.IsNullOrEmpty(this.RenderInstance))
            {
                if (!string.IsNullOrEmpty(this.RenderMethod))
                {
                    //采用特性方法处理
                    Utility.PreRenderTemplateByAttributeMethod(this.RenderInstance, this.RenderMethod, this);
                }
                else
                {
                    //采用接口处理
                    Utility.PreRenderTemplate(this.RenderInstance, this);
                }
            }
            if (this.Visible)
            {
                base.Render(writer);
            }
        }
        #endregion

        #region 开始解析标签数据
        /// <summary>
        /// 开始解析标签数据
        /// </summary>
        /// <param name="ownerTemplate">宿主模版</param>
        /// <param name="container">标签的容器</param>
        /// <param name="tagStack">标签堆栈</param>
        /// <param name="text"></param>
        /// <param name="match"></param>
        /// <param name="isClosedTag">是否闭合标签</param>
        /// <returns>如果需要继续处理EndTag则返回true.否则请返回false</returns>
        internal override bool ProcessBeginTag(Template ownerTemplate, Tag container, Stack<Tag> tagStack, string text, ref Match match, bool isClosedTag)
        {
            //将自身加入到宿主的子模版列表中
            ownerTemplate.ChildTemplates.Add(this);
            //加入到标签容器的元素列表中
            container.AppendChild(this);
            
            if (!string.IsNullOrEmpty(this.File) && System.IO.File.Exists(this.File))
            {
                //读取文件数据进行解析
                new TemplateDocument(this, System.IO.File.ReadAllText(this.File, this.Charset), ownerTemplate.OwnerDocument.DocumentConfig);
            }
            //非闭合标签则查找结束标签
            if (!isClosedTag)
            {
                this.ProcessEndTag(this, this, tagStack, text, ref match);                
            }
            //因为已处理EndTag.所以不需要外部继续再处理
            return false;
        }
        #endregion

        #region 克隆当前元素到新的宿主模版
        /// <summary>
        /// 克隆当前元素到新的宿主模版
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <returns></returns>
        internal override Element Clone(Template ownerTemplate)
        {
            Template tag = new Template(ownerTemplate);
            //加入到宿主模版的子模版列表
            ownerTemplate.ChildTemplates.Add(tag);

            //优先拷贝变量
            foreach (Variable var in this.Variables)
            {
                tag.Variables.Add(var.Clone(tag));
            }
            
            //复制其它属性
            tag.Id = this.Id;
            tag.Name = this.Name;
            tag.Attributes = this.Attributes;
            tag.Charset = this.Charset;
            tag.File = this.File;
            tag.fileDependencies = this.fileDependencies;
            tag.Visible = this.Visible;
            tag.RenderInstance = this.RenderInstance;
            tag.RenderMethod = this.RenderMethod;

            foreach (Element element in this.InnerElements)
            {
                tag.AppendChild(element.Clone(tag));
            }
            
            return tag;
        }
        #endregion
    }
}
