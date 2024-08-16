﻿using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Controls;

public class LineEdit : TextBox
{
   public const string ErrorPC = ":error";
   public const string WarningPC = ":warning";
   
   #region 功能属性定义

   public static readonly StyledProperty<object?> LeftAddOnProperty =
      AvaloniaProperty.Register<LineEdit, object?>(nameof(LeftAddOn));

   public static readonly StyledProperty<object?> RightAddOnProperty =
      AvaloniaProperty.Register<LineEdit, object?>(nameof(RightAddOn));
   
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<LineEdit, SizeType>(nameof(SizeType), SizeType.Middle);
   
   public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
      AvaloniaProperty.Register<LineEdit, AddOnDecoratedVariant>(nameof(StyleVariant), AddOnDecoratedVariant.Outline);
   
   public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
      AvaloniaProperty.Register<LineEdit, AddOnDecoratedStatus>(nameof(Status), AddOnDecoratedStatus.Default);
   
   public static readonly StyledProperty<bool> IsEnableClearButtonProperty =
      AvaloniaProperty.Register<LineEdit, bool>(nameof(IsEnableClearButton), false);
   
   public static readonly StyledProperty<bool> IsEnableRevealButtonProperty =
      AvaloniaProperty.Register<LineEdit, bool>(nameof(IsEnableRevealButton), false);
   
   public object? LeftAddOn
   {
      get => GetValue(LeftAddOnProperty);
      set => SetValue(LeftAddOnProperty, value);
   }
   
   public object? RightAddOn
   {
      get => GetValue(RightAddOnProperty);
      set => SetValue(RightAddOnProperty, value);
   }
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public AddOnDecoratedVariant StyleVariant
   {
      get => GetValue(StyleVariantProperty);
      set => SetValue(StyleVariantProperty, value);
   }
   
   public AddOnDecoratedStatus Status
   {
      get => GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
   }
   
   public bool IsEnableClearButton
   {
      get => GetValue(IsEnableClearButtonProperty);
      set => SetValue(IsEnableClearButtonProperty, value);
   }
   
   public bool IsEnableRevealButton
   {
      get => GetValue(IsEnableRevealButtonProperty);
      set => SetValue(IsEnableRevealButtonProperty, value);
   }
   
   #endregion

   #region 内部属性定义
   
   internal static readonly DirectProperty<LineEdit, bool> IsEffectiveShowClearButtonProperty =
      AvaloniaProperty.RegisterDirect<LineEdit, bool>(nameof(IsEffectiveShowClearButton),
                                                      o => o.IsEffectiveShowClearButton,
                                                      (o, v) => o.IsEffectiveShowClearButton = v);
   
   private bool _isEffectiveShowClearButton;
   internal bool IsEffectiveShowClearButton
   {
      get => _isEffectiveShowClearButton;
      set => SetAndRaise(IsEffectiveShowClearButtonProperty, ref _isEffectiveShowClearButton, value);
   }
   
   #endregion
   
   static LineEdit()
   {
      AffectsRender<LineEdit>(BorderBrushProperty, BackgroundProperty);
      // AffectsMeasure<LineEdit>(LeftAddOnProperty, RightAddOnProperty);
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      SetupEffectiveShowClearButton();
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      
      if (change.Property == AcceptsReturnProperty ||
          change.Property == IsReadOnlyProperty ||
          change.Property == TextProperty ||
          change.Property == IsEnableClearButtonProperty) {
         SetupEffectiveShowClearButton();
      }
      
      if (change.Property == StatusProperty) {
         UpdatePseudoClasses();
      }

      if (change.Property == InnerLeftContentProperty || 
          change.Property == InnerRightContentProperty ||
          change.Property == LeftAddOnProperty ||
          change.Property == RightAddOnProperty) {
         if (change.OldValue is Control oldControl) {
            UIStructureUtils.SetTemplateParent(oldControl, null);
         }

         if (change.NewValue is Control newControl) {
            UIStructureUtils.SetTemplateParent(newControl, this);
         }
      }
   }
   
   // protected override void OnPointerPressed(PointerPressedEventArgs e)
   // {
   //    if (_lineEditKernelDecorator is null) {
   //       return;
   //    }
   //    var targetRect = new Rect(_lineEditKernelDecorator.DesiredSize);
   //    if (!targetRect.Contains(e.GetPosition(_lineEditKernelDecorator))) {
   //       return;
   //    }
   //    base.OnPointerPressed(e);
   // }

   private void SetupEffectiveShowClearButton()
   {
      if (!IsEnableClearButton) {
         IsEffectiveShowClearButton = false;
         return;
      }

      IsEffectiveShowClearButton = !IsReadOnly && !AcceptsReturn && !string.IsNullOrEmpty(Text);
   }
   
   private void UpdatePseudoClasses()
   {
      PseudoClasses.Set(ErrorPC, Status == AddOnDecoratedStatus.Error);
      PseudoClasses.Set(WarningPC, Status == AddOnDecoratedStatus.Warning);
   }
}