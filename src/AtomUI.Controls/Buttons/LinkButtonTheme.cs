﻿using AtomUI.Styling;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class LinkButtonTheme : BaseButtonTheme
{   
   public const string ID = "LinkButton";
   public LinkButtonTheme()
      : base(typeof(Button))
   {
   }
   
   public override string? ThemeResourceKey()
   {
      return ID;
   }
   
   protected override void BuildStyles()
   {
      base.BuildStyles();
      BuildEnabledStyle();
      BuildDisabledStyle();
   }
   
   private void BuildEnabledStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting());
      // 正常状态
      enabledStyle.Setters.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(ButtonResourceKey.DefaultBg)));
      enabledStyle.Setters.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorLink)));
      
      // 正常 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Setters.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorLinkHover)));
         enabledStyle.Add(hoverStyle);
      }
      // 正常按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Setters.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorLinkActive)));
         enabledStyle.Add(pressedStyle);
      }
      
      // 危险按钮状态
      var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
      dangerStyle.Setters.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorError)));
      
      // 危险状态 hover
      {
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Setters.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorHover)));
         dangerStyle.Add(hoverStyle);
      }
      
      // 危险状态按下
      {
         var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
         pressedStyle.Setters.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorErrorActive)));
         dangerStyle.Add(pressedStyle);
      }
      enabledStyle.Add(dangerStyle);

      Add(enabledStyle);
      
      BuildEnabledGhostStyle();
   }

   private void BuildEnabledGhostStyle()
   {
      var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
      // 正常状态
      ghostStyle.Setters.Add(new Setter(Button.BackgroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTransparent)));
      
      Add(ghostStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Setters.Add(new Setter(Button.ForegroundProperty, new DynamicResourceExtension(GlobalResourceKey.ColorTextDisabled)));
      Add(disabledStyle);
   }
}