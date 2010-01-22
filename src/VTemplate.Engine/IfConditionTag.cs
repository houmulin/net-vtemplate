/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  IfConditionTag
 *
 * ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VTemplate.Engine
{
    #region If条件的比较类型
    /// <summary>
    /// If条件的比较类型
    /// </summary>
    public enum IfConditionCompareType
    {
        /// <summary>
        /// 相等比较"="或"=="
        /// </summary>
        Equal,
        /// <summary>
        /// 小于比较"&lt;"
        /// </summary>
        LT,
        /// <summary>
        /// 小于或等于比较"&lt;="
        /// </summary>
        LTAndEqual,
        /// <summary>
        /// 大于比较"&gt;"
        /// </summary>
        GT,
        /// <summary>
        /// 大于或等于比较"&gt;="
        /// </summary>
        GTAndEqual,
        /// <summary>
        /// 不等于比较"&lt;&gt;"或"!="
        /// </summary>
        UnEqual
    }
    #endregion

    /// <summary>
    /// If条件标签.
    /// </summary>
    public class IfConditionTag : Tag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerTemplate"></param>
        internal IfConditionTag(Template ownerTemplate)
            : base(ownerTemplate)
        {
            this.Compare = IfConditionCompareType.Equal;
            this.Values = new ElementCollection<IExpression>();
        }
        #region 重写Tag的方法
        /// <summary>
        /// 返回标签的名称
        /// </summary>
        public override string TagName
        {
            get { return "elseif"; }
        }
        /// <summary>
        /// 返回标签的结束标签名称.
        /// </summary>
        public override string EndTagName
        {
            get
            {
                return "if";
            }
        }
        /// <summary>
        /// 返回此标签是否是单一标签.即是不需要配对的结束标签
        /// </summary>
        internal override bool IsSingleTag
        {
            get { return true; }
        }
        #endregion

        #region 属性定义
        /// <summary>
        /// 条件变量
        /// </summary>
        public virtual VariableExpression VarExpression { get; protected set; }
        /// <summary>
        /// 比较值列表
        /// </summary>
        public virtual ElementCollection<IExpression> Values { get; protected set; }
        /// <summary>
        /// 比较类型
        /// </summary>
        public IfConditionCompareType Compare { get; protected set; }
        /// <summary>
        /// 表达式.
        /// </summary>
        /// <remarks>表达式中可用"{0}"标记符表示条件变量的值</remarks>
        public string Expression { get; protected set; }
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
                case "var":
                    this.VarExpression = ParserHelper.CreateVariableExpression(this.OwnerTemplate, item.Value);
                    break;
                case "value":
                    this.Values.Add(ParserHelper.CreateExpression(this.OwnerTemplate, item.Value));
                    break;
                case "expression":
                    this.Expression = item.Value.Trim();
                    break;
                case "compare":
                    switch (item.Value.Trim())
                    {
                        case ">":
                            this.Compare = IfConditionCompareType.GT;
                            break;
                        case ">=":
                            this.Compare = IfConditionCompareType.GTAndEqual;
                            break;
                        case "<":
                            this.Compare = IfConditionCompareType.LT;
                            break;
                        case "<=":
                            this.Compare = IfConditionCompareType.LTAndEqual;
                            break;
                        case "<>":
                        case "!=":
                            this.Compare = IfConditionCompareType.UnEqual;
                            break;
                        default:
                            this.Compare = IfConditionCompareType.Equal;
                            break;
                    }
                    break;
            }
        }
        #endregion

        #region 测试条件是否成功
        /// <summary>
        /// 判断测试条件是否成功
        /// </summary>
        /// <returns></returns>
        internal virtual bool IsTestSuccess()
        {
            //本元素没有条件.所以匹配成功
            if (this.Values == null || this.VarExpression == null) return true;

            //取得条件变量的值
            object testValue = this.VarExpression.GetValue();
            if (Utility.IsNothing(testValue))
            {
                //条件变量的值为null或DBNull.Value.
                switch (this.Compare)
                {
                    case IfConditionCompareType.Equal:
                        //如果相等比较.则比较值列表中是否有空值比较,如果有则认为成立.否则不成立
                        foreach (IExpression exp in this.Values)
                        {
                            object obj = exp.GetValue();
                            string value = Utility.IsNothing(obj) ? string.Empty : obj.ToString();
                            if (value == string.Empty) return true;
                        }
                        return false;
                    case IfConditionCompareType.UnEqual:
                        //如果不相等比较.则比较值列表中是否有和非空值比较,如果有则认为成立.否则不成立
                        foreach (IExpression exp in this.Values)
                        {
                            object obj = exp.GetValue();
                            string value = Utility.IsNothing(obj) ? string.Empty : obj.ToString();
                            if (value != string.Empty) return true;
                        }
                        return false;
                    default:
                        //其它比较方式.都认为条件不成立
                        return false;
                }
            }
            else
            {
                //处理表达式
                if (!string.IsNullOrEmpty(this.Expression))
                {
                    object v = testValue;
                    try
                    {
                        testValue = Evaluator.ExpressionEvaluator.Eval(string.Format(this.Expression, testValue));
                    }
                    catch
                    {
                        testValue = v;
                    }
                }

                //取IComparable接口
                IComparable comparer;
                bool isComparer = testValue is IComparable;
                if (isComparer)
                {
                    comparer = (IComparable)testValue;
                }
                else
                {
                    //如果值未实现IComparable接口.则默认取其字符串形式的IComparable接口
                    comparer = (testValue.ToString() as IComparable);
                }

                switch (this.Compare)
                {
                    case IfConditionCompareType.GT:
                        foreach (IExpression exp in this.Values)
                        {
                            object obj = exp.GetValue();
                            if (!Utility.IsNothing(obj))
                            {
                                object compareValue = Utility.ConvertTo(obj.ToString(), testValue.GetType());
                                if (compareValue != null && comparer.CompareTo(compareValue) > 0) return true;
                            }
                        }
                        return false;
                    case IfConditionCompareType.GTAndEqual:
                        foreach (IExpression exp in this.Values)
                        {
                            object obj = exp.GetValue();
                            if (!Utility.IsNothing(obj))
                            {
                                object compareValue = Utility.ConvertTo(obj.ToString(), testValue.GetType());
                                if (compareValue != null && comparer.CompareTo(compareValue) >= 0) return true;
                            }
                        }
                        return false;
                    case IfConditionCompareType.LT:
                        foreach (IExpression exp in this.Values)
                        {
                            object obj = exp.GetValue();
                            if (!Utility.IsNothing(obj))
                            {
                                object compareValue = Utility.ConvertTo(obj.ToString(), testValue.GetType());
                                if (compareValue != null && comparer.CompareTo(compareValue) < 0) return true;
                            }
                        }
                        return false;
                    case IfConditionCompareType.LTAndEqual:
                        foreach (IExpression exp in this.Values)
                        {
                            object obj = exp.GetValue();
                            if (!Utility.IsNothing(obj))
                            {
                                object compareValue = Utility.ConvertTo(obj.ToString(), testValue.GetType());
                                if (compareValue != null && comparer.CompareTo(compareValue) <= 0) return true;
                            }
                        }
                        return false;
                    case IfConditionCompareType.UnEqual:
                        //不等于比较
                        foreach (IExpression exp in this.Values)
                        {
                            object obj = exp.GetValue();
                            if (!Utility.IsNothing(obj))
                            {
                                if (testValue is string)
                                {
                                    //字符串比较.则不区分大小写
                                    if(!string.Equals(obj.ToString(), (string)testValue, StringComparison.InvariantCultureIgnoreCase))return true;
                                }
                                else if (!isComparer)
                                {
                                    //优先进行类型比较
                                    if (obj is Type)
                                    {
                                        if(testValue.GetType() != (Type)obj)return true;
                                    }
                                    else if (testValue.GetType() == obj.GetType())
                                    {
                                        if (testValue != obj) return true;
                                    }
                                    else
                                    {
                                        object compareValue = Utility.ConvertTo(obj.ToString(), testValue.GetType());
                                        if (compareValue == null || comparer.CompareTo(compareValue) != 0) return true;
                                    }
                                }
                                else
                                {
                                    object compareValue = Utility.ConvertTo(obj.ToString(), testValue.GetType());
                                    if (compareValue == null || comparer.CompareTo(compareValue) != 0) return true;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        return false;
                    default:
                        //相等比较
                        foreach (IExpression exp in this.Values)
                        {
                            object obj = exp.GetValue();
                            if (!Utility.IsNothing(obj))
                            {
                                if (testValue is string)
                                {
                                    //字符串比较.则不区分大小写
                                    if (string.Equals(obj.ToString(), (string)testValue, StringComparison.InvariantCultureIgnoreCase)) return true;
                                }
                                else if (!isComparer)
                                {
                                    //优先进行类型比较
                                    if (obj is Type)
                                    {
                                        if (testValue.GetType() == (Type)obj) return true;
                                    }
                                    else if (testValue.GetType() == obj.GetType())
                                    {
                                        if (testValue == obj) return true;
                                    }
                                    else
                                    {
                                        object compareValue = Utility.ConvertTo(obj.ToString(), testValue.GetType());
                                        if (compareValue != null && comparer.CompareTo(compareValue) == 0) return true;
                                    }
                                }
                                else
                                {
                                    object compareValue = Utility.ConvertTo(obj.ToString(), testValue.GetType());
                                    if (compareValue != null && comparer.CompareTo(compareValue) == 0) return true;
                                }
                            }
                        }
                        return false;
                }
            }
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
            if (this.Values.Count == 0) throw new ParserException(string.Format("{0}标签中缺少value属性", this.TagName));

            //判断标签的容器是否为IfTag标签
            if (!(container is IfTag)) throw new ParserException(string.Format("未找到和{0}标签对应的{1}标签", this.TagName, this.EndTagName));

            IfTag ifTag = (IfTag)container;
            //判断此条件是否带var属性.如果不带则设置为if的条件
            if (this.VarExpression == null) this.VarExpression = ifTag.VarExpression;

            //加入到If标签的ElseIf队列
            ifTag.AddElseCondition(this);

            return true;
        }
        #endregion

        #region 克隆当前元素到新的宿主模板
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        protected void CopyTo(IfConditionTag tag)
        {
            base.CopyTo(tag);
            tag.Compare = this.Compare;
            tag.Expression = this.Expression;
            tag.VarExpression = this.VarExpression == null ? null : (VariableExpression)(this.VarExpression.Clone(tag.OwnerTemplate));
            if (this.Values != null)
            {
                foreach (IExpression exp in this.Values)
                {
                    tag.Values.Add(exp.Clone(tag.OwnerTemplate));
                }
            }
        }
        /// <summary>
        /// 克隆当前元素到新的宿主模板
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <returns></returns>
        internal override Element Clone(Template ownerTemplate)
        {
            IfConditionTag tag = new IfConditionTag(ownerTemplate);
            this.CopyTo(tag);
            return tag;
        }
        #endregion
    }
}
