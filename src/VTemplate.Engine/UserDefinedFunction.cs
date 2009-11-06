/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  UserDefinedFunction
 *
 * ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace VTemplate.Engine
{
    /// <summary>
    /// 自定义函数委托
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object UserDefinedFunction(object[] args);

    /// <summary>
    /// 自定义函数集合
    /// </summary>
    public class UserDefinedFunctionCollection
        : Dictionary<string, UserDefinedFunction>
    {
        /// <summary>
        /// 
        /// </summary>
        public UserDefinedFunctionCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {

        }

        /// <summary>
        /// 添加与方法名同名的变量函数
        /// </summary>
        /// <param name="function"></param>
        public void Add(UserDefinedFunction function)
        {
            if (function != null)
                this.Add(function.Method.Name, function);
        }
    }
}
