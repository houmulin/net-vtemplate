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
    /// 博客园新闻列表测试例子
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class cnblogs_newslist : PageBase
    {
        #region 新闻数据源
        //新闻的实体
        public class News
        {
            /// <summary>
            /// 新闻Id
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// 新闻标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 是否可见
            /// </summary>
            public bool Visible { get; set; }
        }
        /// <summary>
        /// 根据类型获取新闻数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<News> GetNewsData(string type)
        {
            //以下数据只是测试.在实际项目中可从数据库获取
            List<News> data = new List<News>();

            switch (type)
            {
                case "relating":
                    data.Add(new News() { Id = 48791, Title = "消息称盛大收购成都星漫科技 涉资1.4亿元", Visible= true });
                    data.Add(new News() { Id = 48785, Title = "巴伦周刊：暴雪称游戏有望5年内击败电影电视业", Visible = true });
                    data.Add(new News() { Id = 48774, Title = "盛大称私服运营商煽动酿成游戏堵门事件已报案", Visible = true });
                    data.Add(new News() { Id = 48734, Title = "开心农场今年有望盈利300万 活跃玩家达1600万", Visible = true });
                    data.Add(new News() { Id = 48673, Title = "暴雪的秘密：游戏首重平衡性训练创意思考", Visible = true });
                    //
                    data.Add(new News() { Id = 11111, Title = "这是一条多余的数据,所有显示时不可见", Visible = false });
                    data.Add(new News() { Id = 48670, Title = "魔兽世界第三资料片官方中文介绍视频", Visible = true });
                    break;
                case "hoting":
                    data.Add(new News() { Id = 48741, Title = "全球网速排名揭晓 中国仅排71", Visible = true });
                    data.Add(new News() { Id = 48742, Title = "[视频]傲游陈明杰:只有IE配做我们的竞争对手", Visible = true });
                    data.Add(new News() { Id = 48574, Title = "魔兽玩家驻留网易 每天烧钱百万叫苦不迭", Visible = true });
                    data.Add(new News() { Id = 48781, Title = "专家称白领\"偷菜\"不能排解焦虑 最好融入生活", Visible = true });
                    data.Add(new News() { Id = 48780, Title = "英网站评出IT业十大错误决策", Visible = true });
                    data.Add(new News() { Id = 48702, Title = "中国雅虎近百北京员工可能离职", Visible = true });
                    //
                    data.Add(new News() { Id = 11111, Title = "这是一条多余的数据,所有显示时不可见", Visible = false });
                    break;
            }
            return data;
        }

        /// <summary>
        /// 获取新闻的访问地址
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        private string GetNewsUrl(News news)
        {
            return string.Format("http://news.cnblogs.com/n/{0}", news.Id);
        }
        #endregion

        /// <summary>
        /// 初始化当前页面模版数据
        /// </summary>
        protected override void InitPageTemplate()
        {
            //获取所有名称为topnews的模版块
            ElementCollection<Template> templates = this.Document.GetChildTemplatesByName("topnews");
            foreach (Template template in templates)
            {
                //根据模版块里定义的type属性条件取得新闻数据
                List<News> newsData = GetNewsData(template.Attributes.GetValue("type"));
                //设置变量newsdata的值
                template.Variables.SetValue("newsdata", newsData);

                //取得模版块下Id为newslist的标签(也即是在cnblogs_newsdata.html文件中定义的foreach标签)
                Tag tag = template.GetChildTagById("newslist");
                if (tag is ForEachTag)
                {
                    //如果标签为foreach标签则设置其BeforeRender事件用于设置变量表达式{$:#.news.url}的值
                    tag.BeforeRender += new System.ComponentModel.CancelEventHandler(Tag_BeforeRender);
                }
            }
        }

        /// <summary>
        /// 在标签每次呈现数据前触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tag_BeforeRender(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ForEachTag t = (ForEachTag)sender;

            #region 方法一: 根据foreach标签的Item属性取得变量
            //取得当前项的值(因为foreach标签的数据源是List<News>集合,所以当前项的值类型为News实体)
            //News news = (News)t.Item.Value;
            //设置当前项的变量表达式的值.也即是"{$:#.news.url}"变量表达式
            //t.Item.SetExpValue("url", GetNewsUrl(news));
            #endregion

            #region 方法二: 直接获取news变量
            //或者也可以直接取得news变量
            Variable newsVar = t.OwnerTemplate.Variables["news"];
            News news = (News)newsVar.Value;
            newsVar.SetExpValue("url", GetNewsUrl(news));
            #endregion

            //当新闻不可见时.你可以取消本次输出
            if (!news.Visible) e.Cancel = true;
        }
    }
}
