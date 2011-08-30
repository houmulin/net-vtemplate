/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  PanelTag
 *
 * ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VTemplate.Engine
{
    /// <summary>
    /// 面板数据标签,如: &lt;vt:panel container="header"&gt;&lt;/vt:panel&gt;
    /// </summary>
    public class PanelTag : Tag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTemplate"></param>
        internal PanelTag(Template ownerTemplate)
            : base(ownerTemplate)
        {  }

        #region 重写Tag的方法
        /// <summary>
        /// 返回标签的名称
        /// </summary>
        public override string TagName
        {
            get { return "panel"; }
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
        /// 面板所在的容器标签
        /// </summary>
        public string Container
        {
            get;
            protected set;
        }
        #endregion

        #region 呈现本元素的数据
        /// <summary>
        /// 呈现本元素的数据
        /// </summary>
        /// <param name="writer"></param>
        public override void Render(System.IO.TextWriter writer)
        {
            //不能直接呈现数据
        }
        /// <summary>
        /// 呈现数据到容器里
        /// </summary>
        /// <param name="writer"></param>
        internal void RenderToContainer(System.IO.TextWriter writer)
        {
            base.Render(writer);
        }
        #endregion

        #region 开始解析标签数据
        /// <summary>
        /// 开始解析标签数据
        /// </summary>
        /// <param name="ownerTemplate">宿主模板</param>
        /// <param name="container">标签的容器</param>
        /// <param name="tagStack">标签堆栈</param>
        /// <param name="text"></param>
        /// <param name="match"></param>
        /// <param name="isClosedTag">是否闭合标签</param>
        /// <returns>如果需要继续处理EndTag则返回true.否则请返回false</returns>
        internal override bool ProcessBeginTag(Template ownerTemplate, Tag container, Stack<Tag> tagStack, string text, ref Match match, bool isClosedTag)
        {
            if (string.IsNullOrEmpty(this.Container)) throw new ParserException(string.Format("{0}标签中必须定义container属性", this.TagName));
            
            var conTag = this.OwnerDocument.GetChildTagById(this.Container) as ContainerTag;
            if (conTag == null) throw new ParserException(string.Format("{0}标签中定义的Container“{1}”不存在", this.TagName, this.Container));
            conTag.AddPanel(this);

            bool flag = base.ProcessBeginTag(ownerTemplate, container, tagStack, text, ref match, isClosedTag);

            return flag;
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
                case "container":
                    this.Container = item.Text;
                    break;
            }
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
            PanelTag tag = new PanelTag(ownerTemplate);
            tag.Container = this.Container;
            this.CopyTo(tag);

            var conTag = ownerTemplate.OwnerDocument.GetChildTagById(this.Container) as ContainerTag;
            if (conTag != null) conTag.AddPanel(tag);

            return tag;
        }
        #endregion
    }
}
