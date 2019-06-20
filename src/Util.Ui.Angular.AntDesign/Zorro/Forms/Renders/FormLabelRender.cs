﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Util.Ui.Angular.Base;
using Util.Ui.Angular.Resolvers;
using Util.Ui.Builders;
using Util.Ui.Configs;
using Util.Ui.Extensions;
using Util.Ui.Zorro.Forms.Builders;
using Util.Ui.Zorro.Forms.Configs;

namespace Util.Ui.Zorro.Forms.Renders {
    /// <summary>
    /// 表单标签渲染器
    /// </summary>
    public class FormLabelRender : AngularRenderBase {
        /// <summary>
        /// 配置
        /// </summary>
        private readonly Config _config;

        /// <summary>
        /// 初始化表单标签渲染器
        /// </summary>
        /// <param name="config">配置</param>
        public FormLabelRender( Config config ) : base( config ) {
            _config = config;
        }

        /// <summary>
        /// 获取标签生成器
        /// </summary>
        protected override TagBuilder GetTagBuilder() {
            ResolveExpression();
            var builder = new FormLabelBuilder();
            Config( builder );
            return builder;
        }

        /// <summary>
        /// 解析属性表达式
        /// </summary>
        private void ResolveExpression() {
            if( _config.Contains( UiConst.For ) == false )
                return;
            var expression = _config.GetValue<ModelExpression>( UiConst.For );
            ExpressionResolver.Init( expression, _config );
        }

        /// <summary>
        /// 配置
        /// </summary>
        private void Config( TagBuilder builder ) {
            ConfigId( builder );
            ConfigText( builder );
            ConfigRequired( builder );
            ConfigFor( builder );
            ConfigColon( builder );
            ConfigGrid( builder );
            ConfigContent( builder );
        }

        /// <summary>
        /// 配置文本
        /// </summary>
        private void ConfigText( TagBuilder builder ) {
            builder.AppendContent( _config.GetValue( UiConst.Label ) );
        }

        /// <summary>
        /// 配置必填样式
        /// </summary>
        private void ConfigRequired( TagBuilder builder ) {
            builder.AddAttribute( "[nzRequired]", _config.GetBoolValue( UiConst.Required ) );
        }

        /// <summary>
        /// 配置For
        /// </summary>
        private void ConfigFor( TagBuilder builder ) {
            builder.AddAttribute( "nzFor", _config.GetValue( UiConst.LabelFor ) );
        }

        /// <summary>
        /// 配置冒号
        /// </summary>
        private void ConfigColon( TagBuilder builder ) {
            builder.AddAttribute( "[nzNoColon]", ( !_config.GetValue<bool?>( UiConst.ShowColon ) ).SafeString().ToLower() );
        }

        /// <summary>
        /// 配置栅格
        /// </summary>
        private void ConfigGrid( TagBuilder builder ) {
            var shareConfig = GetShareConfig();
            ConfigSpan( builder, shareConfig );
        }

        /// <summary>
        /// 获取共享配置
        /// </summary>
        private GridShareConfig GetShareConfig() {
            return _config.Context?.GetValueFromItems<GridShareConfig>( GridShareConfig.Key );
        }

        /// <summary>
        /// 配置跨度
        /// </summary>
        private void ConfigSpan( TagBuilder builder,GridShareConfig shareConfig) {
            var span = _config.GetValue( UiConst.Span );
            if( span.IsEmpty() )
                span = shareConfig?.LabelSpan;
            builder.AddAttribute( "[nzSpan]", span );
        }
    }
}
