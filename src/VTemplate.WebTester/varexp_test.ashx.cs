﻿using System;
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
            //注册一个变量函数,用于求取年龄的说明
            this.Document.RegisterGlobalFunction(this.GetAgeRemark);

            this.Document.Variables.SetValue("user", new { name = "张三", age = 20 });
            this.Document.GetChildTemplateById("t1").Variables.SetValue("user", new { name = "李四", age = 35 });
            this.Document.GetChildTemplateById("t2").Variables.SetValue("user", new { name = "王五", age = 50 });
        }

        /// <summary>
        /// 获取年龄的说明
        /// </summary>
        /// <param name="age"></param>
        /// <returns></returns>
        private object GetAgeRemark(object[] ages)
        {
            string remark = "未知";
            object age = ages[0];
            if (age != null && age != DBNull.Value && age is int)
            {
                int a = (int)age;
                if (a < 30)
                {
                    remark = "青年";
                }
                else if (a < 50)
                {
                    remark = "中年";
                }
                else
                {
                    remark = "老年";
                }
            }
            return remark;
        }
    }
}
