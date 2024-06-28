﻿using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public enum FlyoutTriggerType
{
   Hover,
   Click,
   Focus
}

public class FlyoutHost : Control
{
   public static readonly StyledProperty<Control?> AnchorTargetProperty =
      AvaloniaProperty.Register<FlyoutHost, Control?>(nameof(AnchorTarget));

   /// <summary>
   /// Defines the <see cref="Flyout"/> property
   /// </summary>
   public static readonly StyledProperty<PopupFlyoutBase?> FlyoutProperty =
      AvaloniaProperty.Register<FlyoutHost, PopupFlyoutBase?>(nameof(Flyout));

   /// <summary>
   /// 触发方式
   /// </summary>
   public static readonly StyledProperty<FlyoutTriggerType> TriggerProperty =
      AvaloniaProperty.Register<FlyoutHost, FlyoutTriggerType>(nameof(Trigger), FlyoutTriggerType.Click);
   
   /// <summary>
   /// 是否显示指示箭头
   /// </summary>
   public static readonly StyledProperty<bool> IsShowArrowProperty =
      AvaloniaProperty.Register<FlyoutHost, bool>(nameof(IsShowArrow), true);

   /// <summary>
   /// 箭头是否始终指向中心
   /// </summary>
   public static readonly StyledProperty<bool> IsPointAtCenterProperty =
      AvaloniaProperty.Register<FlyoutHost, bool>(nameof(IsPointAtCenter), false);

   /// <summary>
   /// Defines the ToolTip.Placement property.
   /// </summary>
   public static readonly StyledProperty<PlacementMode> PlacementProperty =
      AvaloniaProperty.Register<FlyoutHost, PlacementMode>(
         nameof(Placement), defaultValue: PlacementMode.Top);

   /// <summary>
   /// 距离 anchor 的边距，根据垂直和水平进行设置
   /// </summary>
   public static readonly StyledProperty<double> MarginToAnchorProperty =
      AvaloniaProperty.Register<FlyoutHost, double>(nameof(MarginToAnchor), 0);

   public static readonly StyledProperty<int> ShowDelayProperty =
      AvaloniaProperty.Register<FlyoutHost, int>(nameof(ShowDelay), 400);

   public static readonly StyledProperty<int> BetweenShowDelayProperty =
      AvaloniaProperty.Register<FlyoutHost, int>(nameof(BetweenShowDelay), 100);

   public static readonly StyledProperty<bool> ShowOnDisabledProperty =
      AvaloniaProperty.Register<FlyoutHost, bool>(nameof(ShowOnDisabled), defaultValue: false, inherits: true);

   /// <summary>
   /// 装饰的目标控件
   /// </summary>
   [Content]
   public Control? AnchorTarget
   {
      get => GetValue(AnchorTargetProperty);
      set => SetValue(AnchorTargetProperty, value);
   }

   /// <summary>
   /// Gets or sets the Flyout that should be shown with this button.
   /// </summary>
   public PopupFlyoutBase? Flyout
   {
      get => GetValue(FlyoutProperty);
      set => SetValue(FlyoutProperty, value);
   }

   public FlyoutTriggerType Trigger
   {
      get => GetValue(TriggerProperty);
      set => SetValue(TriggerProperty, value);
   }

   public bool IsShowArrow
   {
      get => GetValue(IsShowArrowProperty);
      set => SetValue(IsShowArrowProperty, value);
   }

   public bool IsPointAtCenter
   {
      get => GetValue(IsPointAtCenterProperty);
      set => SetValue(IsPointAtCenterProperty, value);
   }

   public PlacementMode Placement
   {
      get => GetValue(PlacementProperty);
      set => SetValue(PlacementProperty, value);
   }

   public double MarginToAnchor
   {
      get => GetValue(MarginToAnchorProperty);
      set => SetValue(MarginToAnchorProperty, value);
   }

   public int ShowDelay
   {
      get => GetValue(ShowDelayProperty);
      set => SetValue(ShowDelayProperty, value);
   }

   public int BetweenShowDelay
   {
      get => GetValue(BetweenShowDelayProperty);
      set => SetValue(BetweenShowDelayProperty, value);
   }

   public bool ShowOnDisabled
   {
      get => GetValue(ShowOnDisabledProperty);
      set => SetValue(ShowOnDisabledProperty, value);
   }

   private bool _initialized = false;
   private CompositeDisposable _compositeDisposable;

   public FlyoutHost()
   {
      _compositeDisposable = new CompositeDisposable();
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         if (AnchorTarget is not null) {
            ((ISetLogicalParent)AnchorTarget).SetParent(this);
            VisualChildren.Add(AnchorTarget);
         }
         _initialized = true;
      }
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      SetupTriggerHandler();
      SetupFlyoutProperties();
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      _compositeDisposable?.Dispose();
   }

   private void SetupFlyoutProperties()
   {
      
   }

   private void SetupTriggerHandler()
   {
      if (AnchorTarget is null) {
         return;
      }

      if (Trigger == FlyoutTriggerType.Hover) {
         _compositeDisposable.Add(IsPointerOverProperty.Changed.Subscribe(args =>
         {
            if (args.Sender == AnchorTarget) {
               HandleAnchorTargetHover(args);
            }
         }));
      } else if (Trigger == FlyoutTriggerType.Focus) {
         _compositeDisposable.Add(IsFocusedProperty.Changed.Subscribe(args =>
         {
            if (args.Sender == AnchorTarget) {
               HandleAnchorTargetFocus(args);
            }
         }));
      } else if (Trigger == FlyoutTriggerType.Click) {
         var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
         _compositeDisposable.Add(inputManager.Process.Subscribe(HandleAnchorTargetClick));
      }
   }

   private void HandleAnchorTargetHover(AvaloniaPropertyChangedEventArgs<bool> e)
   {
      if (Flyout is not null) {
         if (e.GetNewValue<bool>()) {
            ShowFlyout();
         } else {
            HideFlyout();
         }
      }
   }

   private void HandleAnchorTargetFocus(AvaloniaPropertyChangedEventArgs<bool> e)
   {
      if (Flyout is not null) {
         if (e.GetNewValue<bool>()) {
            ShowFlyout();
         } else {
            HideFlyout();
         }
      }
   }

   private void HandleAnchorTargetClick(RawInputEventArgs args)
   {
      if (args is RawPointerEventArgs pointerEventArgs) {
         if (AnchorTarget is not null) {
            var pos = AnchorTarget.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(AnchorTarget)!);
            if (!pos.HasValue) {
               return;
            }

            var bounds = new Rect(pos.Value, AnchorTarget.Bounds.Size);
            if (bounds.Contains(pointerEventArgs.Position)) {
               if (Flyout is not null) {
                  if (Flyout.IsOpen) {
                     HideFlyout();
                  } else {
                     ShowFlyout();
                  }
               }
            }
         }
      }
   }

   public void ShowFlyout()
   {
      if (Flyout is null || AnchorTarget is null) {
         return;
      }
      
      Flyout.ShowAt(AnchorTarget);
   }

   public void HideFlyout()
   {
      if (Flyout is null) {
         return;
      }
      Flyout.Hide();
   }
}