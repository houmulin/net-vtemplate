/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  VariableField
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace VTemplate.Engine
{
    /// <summary>
    /// 变量的表达式.如:{$:name.age} 变量元素中的变量表达式则是".age"
    /// </summary>
    public class VariableExpression : IExpression
    {
        /// <summary>
        /// 变量字段
        /// </summary>
        /// <param name="varPrefix"></param>
        /// <param name="variable"></param>
        internal VariableExpression(string varPrefix, Variable variable) : this(varPrefix, variable, null, false) { }
        /// <summary>
        /// 变量字段
        /// </summary>
        /// <param name="varPrefix"></param>
        /// <param name="variable"></param>
        /// <param name="fieldName"></param>
        /// <param name="isMethod"></param>
        internal VariableExpression(string varPrefix, Variable variable, string fieldName, bool isMethod)
        {
            this.Variable = variable;
            this.FieldName = fieldName;
            this.IsMethod = isMethod;
        }
        /// <summary>
        /// 变量字段
        /// </summary>
        /// <param name="parentExp"></param>
        /// <param name="fieldName"></param>
        /// <param name="isMethod"></param>
        internal VariableExpression(VariableExpression parentExp, string fieldName, bool isMethod)
        {
            parentExp.NextExpression = this;
            this.ParentExpression = parentExp;
            this.VariablePrefix = parentExp.VariablePrefix;
            this.Variable = parentExp.Variable;
            this.FieldName = fieldName;
            this.IsMethod = isMethod;
        }
        /// <summary>
        /// 变量的前缀
        /// </summary>
        public string VariablePrefix { get; private set; }
        /// <summary>
        /// 变量
        /// </summary>
        public Variable Variable { get; private set; }
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName { get; private set; }
        /// <summary>
        /// 是否是方法
        /// </summary>
        public bool IsMethod { get; private set; }

        /// <summary>
        /// 取得父级表达式
        /// </summary>
        private VariableExpression ParentExpression { get; set; }
        /// <summary>
        /// 取得下一个表达式
        /// </summary>
        private VariableExpression NextExpression { get; set; }

        /// <summary>
        /// 取得此变量字段的值
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return this.GetValue(this.Variable.Value);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private object GetValue(object data)
        {
            if (Utility.IsNothing(data)) return data;

            object value = data;
            if (!string.IsNullOrEmpty(this.FieldName))
            {
                bool exist;
                //优先从缓存中获取数据
                List<string> paths = new List<string>();
                VariableExpression exp = this;
                while (exp!= null)
                {
                    paths.Add(exp.IsMethod ? exp.FieldName + "()" : exp.FieldName);
                    exp = exp.ParentExpression;
                }
                paths.Reverse();
                string expPath = string.Join(".", paths.ToArray());

                value = this.Variable.GetExpValue(expPath, out exist);
                if (!exist)
                {
                    //缓存数据不存在.则从实体取数据
                    if (this.IsMethod)
                    {
                        value = Utility.GetMethodResult(data, this.FieldName, out exist);
                    }
                    else
                    {
                        value = Utility.GetPropertyValue(data, this.FieldName, out exist);
                    }
                    if (!exist) return null;
                    //缓存
                    this.Variable.AddExpValue(expPath, value);
                }
            }

            if (this.NextExpression != null)
            {
                return this.NextExpression.GetValue(value);
            }
            else
            {
                return value;
            }
        }

        #region 输出标签数据
        /// <summary>
        /// 输出表达式的原字符串数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            if (this.VariablePrefix != null)
            {
                buffer.AppendFormat("#{0}", this.VariablePrefix);
            }
            buffer.Append(this.Variable.Name);

            VariableExpression exp = this;
            while (exp != null)
            {
                if (!string.IsNullOrEmpty(exp.FieldName))
                {
                    buffer.Append(".");
                    buffer.Append(exp.FieldName);
                    if (exp.IsMethod) buffer.Append("()");
                }
                exp = exp.NextExpression;
            }
            return buffer.ToString();
        }
        #endregion

        #region ICloneableElement<IExpression> 成员
        /// <summary>
        /// 克隆表达式
        /// </summary>
        /// <param name="ownerTemplate"></param>
        /// <returns></returns>
        public IExpression Clone(Template ownerTemplate)
        {
            //获取新的变量
            Template varTemplate = Utility.GetVariableTemplateByPrefix(ownerTemplate, this.VariablePrefix);
            Variable variable = Utility.GetVariableOrAddNew(varTemplate, this.Variable.Name);
            VariableExpression exp = new VariableExpression(this.VariablePrefix, variable, this.FieldName, this.IsMethod);
            if (this.NextExpression != null)
            {
                exp.NextExpression = (VariableExpression)(this.NextExpression.Clone(ownerTemplate));
                exp.NextExpression.ParentExpression = exp;
            }
            return exp;
        }

        #endregion
    }
}
