/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  ForTag
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VTemplate.Engine
{
    /// <summary>
    /// For标签.如:&lt;vt:for from="1" to="100" step="1" index="i"&gt;...&lt;/vt:for&gt;
    /// </summary>
    public class ForTag : Tag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTemplate"></param>
        internal ForTag(Template ownerTemplate)
            : base(ownerTemplate)
        {
            this.Step = new ConstantExpression(1);
        }

        #region 重写Tag的方法
        /// <summary>
        /// 返回标签的名称
        /// </summary>
        public override string TagName
        {
            get { return "for"; }
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
        /// 起始值
        /// </summary>
        public IExpression From { get; protected set; }
        /// <summary>
        /// 结束值
        /// </summary>
        public IExpression To { get; protected set; }
        /// <summary>
        /// 步长
        /// </summary>
        public IExpression Step { get; protected set; }
        /// <summary>
        /// 索引变量
        /// </summary>
        public VariableIdentity Index { get; protected set; }
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
                case "from":
                    this.From = ParserHelper.CreateExpression(this.OwnerTemplate, item.Value);
                    break;
                case "to":
                    this.To = ParserHelper.CreateExpression(this.OwnerTemplate, item.Value);
                    break;
                case "step":
                    this.Step = ParserHelper.CreateExpression(this.OwnerTemplate, item.Value);
                    break;
                case "index":
                    this.Index = ParserHelper.CreateVariableIdentity(this.OwnerTemplate, item.Value);
                    break;
            }
        }
        #endregion

        #region 呈现本元素的数据
        /// <summary>
        /// 呈现本元素的数据
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderTagData(System.IO.TextWriter writer)
        {
            decimal from = Utility.ConverToDecimal(this.From.GetValue());
            decimal step = Utility.ConverToDecimal(this.Step.GetValue());
            decimal to = Utility.ConverToDecimal(this.To.GetValue());
            decimal index = from;

            LoopIndex li = new LoopIndex(index);
            if (this.Index != null) this.Index.Value = li;
            if (step >= 0)
            {
                while (index <= to)
                {
                    li.Value = index;
                    li.IsFirst = (index == from);
                    li.IsLast = (index == to);
                    if (this.Index != null) this.Index.Variable.Reset();
                    base.RenderTagData(writer);
                    index += step;
                }
            }
            else
            {
                while (index >= to)
                {
                    li.Value = index;
                    li.IsFirst = (index == from);
                    li.IsLast = (index == to);
                    if (this.Index != null) this.Index.Variable.Reset();
                    base.RenderTagData(writer);
                    index += step;
                }
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
            if (this.From == null) throw new ParserException(string.Format("{0}标签中缺少from属性", this.TagName));
            if (this.To == null) throw new ParserException(string.Format("{0}标签中缺少to属性", this.TagName));

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
            ForTag tag = new ForTag(ownerTemplate);
            base.CopyTo(tag);
            tag.From = this.From == null ? null : this.From.Clone(ownerTemplate);
            tag.To = this.To == null ? null : this.To.Clone(ownerTemplate);
            tag.Step = this.Step == null ? null : this.Step.Clone(ownerTemplate);
            tag.Index = this.Index == null ? null : this.Index.Clone(ownerTemplate);
            return tag;
        }
        #endregion
    }
}
