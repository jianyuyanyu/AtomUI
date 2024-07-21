﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Styling;

internal interface IControlCustomStyle
{
   void InitOnConstruct() {}
   // 当模板
   void SetupUI() {}
   void AfterUIStructureReady() {}
   void SetupTransitions() {}
   void CollectStyleState() {}
   void SetupTokenBindings() {}
   
   #region NeedReview
   // 这些 API 需要重新评估，进来不要再使用
   void ApplyVariableStyleConfig() {}
   void ApplyRenderScalingAwareStyleConfig() {}
   void ApplySizeTypeStyleConfig() {}
   #endregion

   void HandleAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {}
   void HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e) {}
   void HandleTemplateApplied(INameScope scope) {}
   
   /// <summary>
   /// 有时候需要针对实例的样式生成，可以在这个接口中进行配置
   /// </summary>
   void BuildStyles() {}
   
   /// <summary>
   /// 在渲染的时候我们通常需要一些上下文信息，为了不跟对象实例的其他内部字段相互干扰，我们把跟渲染相关的字段都放到一个逻辑容器
   /// </summary>
   void PrepareRenderInfo() {}
   
   /// <summary>
   /// 在这个接口方法中，我们设置跟伪类相关的西悉尼
   /// </summary>
   void UpdatePseudoClasses() {}
}