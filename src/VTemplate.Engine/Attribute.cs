/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  Attribute
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace VTemplate.Engine
{
    /// <summary>
    /// 元素属性
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// 元素属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        internal Attribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 属性的值
        /// </summary>
        public string Value { get; private set; }
    }
}
