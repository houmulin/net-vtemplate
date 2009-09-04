/* ***********************************************
 * Author		:  kingthy
 * Email		:  kingthy@gmail.com
 * DateTime		:  2009-9-4 11:06:45
 * Description	:  TemplateDocumentConfig
 *
 * ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace VTemplate.Engine
{
    #region 标签的开放模式
    /// <summary>
    /// 标签的开放模式
    /// </summary>
    public enum TagOpenMode
    {
        /// <summary>
        /// 简单的.不支持&lt;vt:datareader&gt;等标签
        /// </summary>
        Simple,
        /// <summary>
        /// 完全的.将支持所有标签
        /// </summary>
        Full
    }
    #endregion

    /// <summary>
    /// 模版文档的配置参数
    /// </summary>
    public class TemplateDocumentConfig
    {
        /// <summary>
        /// 默认的配置.标签的开放模式为简单的并且不压缩文本
        /// </summary>
        public static volatile TemplateDocumentConfig Default;
        /// <summary>
        /// 
        /// </summary>
        static TemplateDocumentConfig()
        {
            TemplateDocumentConfig.Default = new TemplateDocumentConfig();
        }
        /// <summary>
        /// 实例化默认的配置.标签的开放模式为简单的并且不压缩文本
        /// </summary>
        public TemplateDocumentConfig()
        {
            this.TagOpenMode = TagOpenMode.Simple;
            this.CompressText = false;
        }
        /// <summary>
        /// 根据参数实例化
        /// </summary>
        /// <param name="tagOpenMode">标签的开放模式</param>
        public TemplateDocumentConfig(TagOpenMode tagOpenMode)
        {
            this.TagOpenMode = tagOpenMode;
            this.CompressText = false;
        }
        /// <summary>
        /// 根据参数实例化
        /// </summary>
        /// <param name="tagOpenMode">标签的开放模式</param>
        /// <param name="compressText">是否压缩文本</param>
        public TemplateDocumentConfig(TagOpenMode tagOpenMode, bool compressText)
        {
            this.TagOpenMode = tagOpenMode;
            this.CompressText = compressText;
        }

        /// <summary>
        /// 标签的开放模式,默认为Simple
        /// </summary>
        public TagOpenMode TagOpenMode { get; private set; }

        /// <summary>
        /// 是否压缩文本.默认不压缩
        /// </summary>
        /// <remarks>
        /// 压缩文本.即是删除换行符和无用的空格(换行符前后的空格)
        /// </remarks>
        public bool CompressText { get; private set; }
    }
}
