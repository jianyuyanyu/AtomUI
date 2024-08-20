﻿using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class ArrowDecoratedBoxTheme : BaseControlTheme
{
   public const string DecoratorPart = "PART_Decorator";
   
   public ArrowDecoratedBoxTheme() : base(typeof(ArrowDecoratedBox)) {}

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<ArrowDecoratedBox>((box, scope) =>
      {
         var decorator = new Border()
         {
            Name = DecoratorPart,
            Margin = new Thickness(0),
         };
         decorator.RegisterInNameScope(scope);
         
         if (box.Child?.Parent is not null) {
            UIStructureUtils.ClearLogicalParentRecursive(box.Child, null);
            UIStructureUtils.ClearVisualParentRecursive(box.Child, null);
         }

         CreateTemplateParentBinding(decorator, Border.BackgroundSizingProperty, ArrowDecoratedBox.BackgroundSizingProperty);
         CreateTemplateParentBinding(decorator, Border.BackgroundProperty, ArrowDecoratedBox.BackgroundProperty);
         CreateTemplateParentBinding(decorator, Border.CornerRadiusProperty, ArrowDecoratedBox.CornerRadiusProperty);
         CreateTemplateParentBinding(decorator, Border.ChildProperty, ArrowDecoratedBox.ChildProperty);
         CreateTemplateParentBinding(decorator, Border.PaddingProperty, ArrowDecoratedBox.PaddingProperty);
         
         return decorator;
      });
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(ArrowDecoratedBox.ForegroundProperty, GlobalTokenResourceKey.ColorText);
      commonStyle.Add(ArrowDecoratedBox.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      commonStyle.Add(ArrowDecoratedBox.MinHeightProperty, GlobalTokenResourceKey.ControlHeight);
      commonStyle.Add(ArrowDecoratedBox.PaddingProperty, ArrowDecoratedBoxTokenResourceKey.Padding);
      commonStyle.Add(ArrowDecoratedBox.ArrowSizeProperty, ArrowDecoratedBoxTokenResourceKey.ArrowSize);
      commonStyle.Add(ArrowDecoratedBox.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      Add(commonStyle);
   }
}