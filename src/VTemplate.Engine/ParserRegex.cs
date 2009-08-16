﻿/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * Description	:  ParserRegex
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VTemplate.Engine
{
    /// <summary>
    /// 解析器的正则表达式库
    /// </summary>
    internal static class ParserRegex
    {
        /// <summary>
        /// 标签的正则表达.如标签: &lt;vt:for from="1" to="100" index="i"&gt;
        /// </summary>
        internal static readonly Regex TagRegex;

        /// <summary>
        /// 结束标签的正则表达式.如标签: &lt;/vt:for&gt;
        /// </summary>
        internal static readonly Regex EndTagRegex;

        /// <summary>
        /// 变量的正则表达式.如变量: {$:member.name} 或带前缀与属性的变量: {$:#.member.name htmlencode='true'}
        /// 前缀必须与#号开始.并且与.号结束.
        /// 如变量没有前缀.则表明此变量是根级模版的变量.
        /// 如果变量只有前缀"#."(如#.member.name)则表示当前模版的变量
        /// 如果变量只有前缀"##."(如##.member.name)则表示是当前模版的父级模版的变量.如果不存在父级模版.则归为当前模版.
        /// 如果变量是前缀"#"加其它字符.(如(#t1.member.name).则表示此变量是属于Id为"t1"的模版.如果不存在Id为"t1"的模版.则将产生解析错误.
        /// </summary>
        internal static readonly Regex VarRegex;

        /// <summary>
        /// 变量表达的正则表达式.如变量: member.name 或带前缀的变量: #.member.name
        /// </summary>
        internal static readonly Regex VarExpRegex;
        /// <summary>
        /// 
        /// </summary>
        static ParserRegex()
        {
            TagRegex = new Regex(@"\G<vt\:\s*(?<tagname>[\-\w]+)(\s+(?<attrname>[\-\w]+)(\s*=\s*""(?<attrval>[^""]*)""|\s*=\s*'(?<attrval>[^']*)'|\s*=\s*(?<attrval>[^\s=>]*)|(?<attrval>\s*?)))*\s*(?<closed>/)?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            EndTagRegex = new Regex(@"\G</vt\:\s*(?<tagname>[\-\w]+)(\s+(?<attrname>[\-\w]+)(\s*=\s*""(?<attrval>[^""]*)""|\s*=\s*'(?<attrval>[^']*)'|\s*=\s*(?<attrval>[^\s=>]*)|(?<attrval>\s*?)))*\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string varExp = @"(?:#(?<prefix>#|[\-\w]*)\.)?(?<name>(?!\d)\w+)(?:\.(?<field>(?!\d)\w+)(?<method>(?:\( *\))?))*";

            VarRegex = new Regex(@"\G{\$\:\s*" + varExp + @"(\s+(?<attrname>[\-\w]+)(\s*=\s*""(?<attrval>[^""]*)""|\s*=\s*'(?<attrval>[^']*)'|\s*=\s*(?<attrval>[^\s=}]*)|(?<attrval>\s*?)))*\s*}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            VarExpRegex = new Regex(@"^\s*" + varExp + @"\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}
