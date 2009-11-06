/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  RenderTag
 *
 * ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace VTemplate.Engine
{
    /// <summary>
    /// 用于呈现某个标签的数据.如: &lt;vt:render tagid="list" /&gt;
    /// </summary>
    public class RenderTag : Tag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTemplate"></param>
        internal RenderTag(Template ownerTemplate)
            : base(ownerTemplate)
        { }

        #region 重写Tag的方法
        /// <summary>
        /// 返回标签的名称
        /// </summary>
        public override string TagName
        {
            get { return "render"; }
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
        /// 
        /// </summary>
        protected Tag renderTarget;
        /// <summary>
        /// 呈现的目标
        /// </summary>
        protected Tag RenderTarget
        {
            get
            {
                if (this.renderTarget == null)
                {
                    this.renderTarget = this.OwnerDocument.GetChildTagById(this.TagId);
                }
                return this.renderTarget;
            }
        }
        /// <summary>
        /// 需要呈现数据的标签的Id
        /// </summary>
        public string TagId { get; set; }
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
                case "tagid":
                    this.TagId = item.Value;
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
            CancelEventArgs args = new CancelEventArgs();
            this.OnBeforeRender(args);
            if (!args.Cancel)
            {
                if (this.RenderTarget != null)
                    this.RenderTarget.Render(writer);
            }
            this.OnAfterRender(EventArgs.Empty); 
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
            if (string.IsNullOrEmpty(this.TagId)) throw new ParserException(string.Format("{0}标签中缺少tagid属性", this.TagName));

            return base.ProcessBeginTag(ownerTemplate, container, tagStack, text, ref match, isClosedTag);
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
            RenderTag tag = new RenderTag(ownerTemplate);
            this.CopyTo(tag);
            tag.TagId = this.TagId;
            return tag;
        }
        #endregion
    }
}
