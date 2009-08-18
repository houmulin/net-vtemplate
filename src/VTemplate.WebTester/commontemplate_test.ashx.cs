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
    /// 共同模版块测试例子
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class commontemplate_test : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            TemplateDocument document = new TemplateDocument(context.Server.MapPath("template/commontemplate_test.html"), Encoding.UTF8);

            //定义男和女用户数据列表(当然这里的数据.你可以从数据库取得)
            Dictionary<string, List<object>> userTables = new Dictionary<string, List<object>>();
            List<object> users1 = new List<object>()
            {
                new {Name = "张三", age="20"},
                new {Name = "李四", age="24"},
                new {Name = "王五", age="30"},
                new {Name = "陈六", age="28"}
            };
            userTables.Add("男", users1);

            List<object> users2 = new List<object>()
            {
                new {Name = "黄丽", age="18"},
                new {Name = "张燕", age="20"},
                new {Name = "陈梅", age="23"},
                new {Name = "李花", age="19"}
            };
            userTables.Add("女", users2);


            //获取名称为usertable的模版块
            ElementCollection<Template> userTemplates = document.GetChildTemplatesByName("usertable");
            foreach (Template template in userTemplates)
            {
                //获取模版里定义的usertype属性条件
                string userType = template.Attributes.GetValue("usertype");
                if (!string.IsNullOrEmpty(userType) && userTables.ContainsKey(userType))
                {
                    //设置当前模版块的users变量值
                    template.Variables.SetValue("users", userTables[userType]);
                }
            }

            document.Render(context.Response.Output);
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
