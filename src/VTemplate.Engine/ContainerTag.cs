/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  ContainerTag
 *
 * ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace VTemplate.Engine
{
    /// <summary>
    /// 容器标签.如: &lt;vt:container id="header" /&gt;
    /// </summary>
    public class ContainerTag : Tag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTemplate"></param>
        internal ContainerTag(Template ownerTemplate)
            : base(ownerTemplate)
        {
            this.Panels = new List<PanelTag>();
        }

        #region 重写Tag的方法
        /// <summary>
        /// 返回标签的名称
        /// </summary>
        public override string TagName
        {
            get { return "container"; }
        }

        /// <summary>
        /// 返回此标签是否是单一标签.即是不需要配对的结束标签
        /// </summary>
        internal override bool IsSingleTag
        {
            get { return false; }
        }
        #endregion

        #region 属性方法定义
        /// <summary>
        /// 此容器下所拥有的面板标签
        /// </summary>
        protected List<PanelTag> Panels { get; private set; }

        /// <summary>
        /// 增加一个面板
        /// </summary>
        /// <param name="panelTag"></param>
        internal void AddPanel(PanelTag panelTag)
        {
            this.Panels.Add(panelTag);
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
            if (string.IsNullOrEmpty(this.Id)) throw new ParserException(string.Format("{0}标签中必须定义id属性", this.TagName));

            bool flag = base.ProcessBeginTag(ownerTemplate, container, tagStack, text, ref match, isClosedTag);

            return flag;
        }
        #endregion

        #region 呈现本元素的数据
        /// <summary>
        /// 呈现本元素的数据
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderTagData(System.IO.TextWriter writer)
        {
            CancelEventArgs args = new CancelEventArgs();
            this.OnBeforeRender(args);
            if (!args.Cancel)
            {
                foreach (var panelTag in this.Panels)
                {
                    panelTag.RenderToContainer(writer);
                }
                foreach (Element item in this.InnerElements)
                {
                    item.Render(writer);
                }
            }
            this.OnAfterRender(EventArgs.Empty);
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
            ContainerTag tag = new ContainerTag(ownerTemplate);
            this.CopyTo(tag);
            return tag;
        }
        #endregion
    }
}
