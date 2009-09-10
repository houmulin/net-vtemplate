/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * DateTime		:  2009-9-10 17:31:28
 * Description	:  VariableFunction
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace VTemplate.Engine
{
    /// <summary>
    /// 变量函数委托
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object VariableFunction(object args);

    /// <summary>
    /// 变量函数集合
    /// </summary>
    public class VariableFunctionCollection : Dictionary<string, VariableFunction>
    {
        /// <summary>
        /// 
        /// </summary>
        public VariableFunctionCollection() : base(StringComparer.InvariantCultureIgnoreCase)
        {
            
        }

        /// <summary>
        /// 添加与方法名同名的变量函数
        /// </summary>
        /// <param name="function"></param>
        public void Add(VariableFunction function)
        {
            if (function != null)
                this.Add(function.Method.Name, function);
        }
    }
}
