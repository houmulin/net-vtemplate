using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using VTemplate.Engine;
using System.Text;

namespace VTemplate.WebTester
{
    /// <summary>
    /// 测试变量表达式
    /// </summary>
    public class varexp_test : PageBase
    {
        /// <summary>
        /// 初始化当前页面模版数据
        /// </summary>
        protected override void InitPageTemplate()
        {
            this.Document.Variables.SetValue("user", new { name = "张三", age = 20 });
            this.Document.GetChildTemplateById("t1").Variables.SetValue("user", new { name = "李四", age = 26 });
            this.Document.GetChildTemplateById("t2").Variables.SetValue("user", new { name = "王五", age = 30 });
        }
    }
}
