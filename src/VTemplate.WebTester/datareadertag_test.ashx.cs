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
    /// 测试DataReader标记
    /// </summary>
    public class datareadertag_test : PageBase
    {
        /// <summary>
        /// 重载父级的方法.在装载模版文件前设置模版文档的安全等级.以便可以使用datareader标签
        /// </summary>
        /// <param name="fileName"></param>
        protected override void LoadTemplateFile(string fileName)
        {
            //设置安全等级为完全.
            TemplateDocument.SafeLevel = SafeLevel.Full;

            base.LoadTemplateFile(fileName);
        }
        /// <summary>
        /// 初始化当前页面模版数据
        /// </summary>
        protected override void InitPageTemplate()
        {
        }
    }
}
