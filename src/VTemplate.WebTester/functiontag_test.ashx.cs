using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using VTemplate.Engine;
using System.Text;
using System.Collections.Generic;
using VTemplate.WebTester.Core;

namespace VTemplate.WebTester
{

    /// <summary>
    /// 测试function标签
    /// </summary>
    public class functiontag_test : PageBase
    {
        /// <summary>
        /// 获取热门的新闻数据
        /// </summary>
        /// <returns></returns>
        public static List<News> GetHotingNews()
        {
            return NewsDbProvider.GetNewsData("hoting");
        }

        /// <summary>
        /// 初始化当前页面模版数据
        /// </summary>
        protected override void InitPageTemplate()
        {
            this.Document.SetValue("newsdata", NewsDbProvider.GetNewsData("relating"));
        }
    }
}
