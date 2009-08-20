using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using VTemplate.Engine;
using System.Text;
using System.Collections.Generic;

namespace VTemplate.WebTester
{
    /// <summary>
    /// 测试foreach标签
    /// </summary>
    public class foreachtag_test : PageBase
    {
        /// <summary>
        /// 初始化当前页面模版数据
        /// </summary>
        protected override void InitPageTemplate()
        {
            List<object> users = new List<object>()
            {
                new {Name = "张三", age="20"},
                new {Name = "李四", age="24"},
                new {Name = "王五", age="30"}
            };
            this.Document.Variables.SetValue("users", users);
        }
    }
}
