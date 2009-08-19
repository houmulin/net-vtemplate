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
    /// 博客日记页面测试
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class blogarchive : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //VT模版的文件地址
            string fileName = context.Server.MapPath("template/blogarchive.html");

            //根据VT模版实例化TemplateDocument对象
            TemplateDocument document = new TemplateDocument(fileName, Encoding.UTF8);
            //或者从缓存模版里构造实例(在实际的Web项目里建议采用此方法构造实例)
            //TemplateDocument document = TemplateDocument.FromFileCache(fileName, Encoding.UTF8);

            //对VT模版里的blogarchive变量赋值
            document.Variables.SetValue("blogarchive", this.GetBlogArchive());

            //输出最终呈现的数据
            document.Render(context.Response.Output);
        }


        /// <summary>
        /// 获取博客日记数据
        /// </summary>
        /// <returns></returns>
        private object GetBlogArchive()
        {
            //示例里我们只随便构造一个对象.在实际项目里你可以从数据库获取博客日记的真正数据
            return new
            {
                title = "这只是一篇测试的博客日记",
                content = "你好,这只是用于测试VTemplate模版引擎所用的博客日记",
                author = "kingthy",
                time = DateTime.Now,
                comments = new object[]{
                    new {author = "张三", time = DateTime.Now.AddDays(-1), content = "沙发"},
                    new {author = "李四", time = DateTime.Now.AddDays(0), content = "顶楼主"},
                    new {author = "王五", time = DateTime.Now.AddDays(1), content = "板凳啊"}
                }
            };
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
