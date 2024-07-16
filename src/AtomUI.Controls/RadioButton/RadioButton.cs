using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

public partial class RadioButton : AvaloniaRadioButton, 
                                   ICustomHitTest,
                                   IWaveAdornerInfoProvider, 
                                   IControlCustomStyle
{
   private bool _initialized = false;
   private IPen? _cachedPen;
   private ControlStyleState _styleState;
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   
   static RadioButton()
   {
      AffectsRender<RadioButton>(
         RadioBorderBrushProperty,
         RadioInnerBackgroundProperty,
         RadioBackgroundProperty,
         RadioBorderThicknessProperty,
         RadioDotEffectSizeProperty);
   }

   public RadioButton()
   {
      _controlTokenBinder = new ControlTokenBinder(this, RadioButtonToken.ID);
      _customStyle = this;
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      _customStyle.ApplyRenderScalingAwareStyleConfig();
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width + RadioSize + PaddingInline;
      var targetHeight = Math.Max(size.Height, RadioSize);
      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var arrangeRect = RadioTextRect();

      var visualChildren = VisualChildren;
      var visualCount = visualChildren.Count;

      for (var i = 0; i < visualCount; i++) {
         Visual visual = visualChildren[i];
         if (visual is Layoutable layoutable) {
            layoutable.Arrange(arrangeRect);
         }
      }

      return finalSize;
   }

   public sealed override void Render(DrawingContext context)
   {
      var radioRect = RadioRect();
      var penWidth = RadioBorderThickness.Top;
      PenUtils.TryModifyOrCreate(ref _cachedPen, RadioBorderBrush, RadioBorderThickness.Top);
      context.DrawEllipse(RadioBackground, _cachedPen, radioRect.Deflate(penWidth / 2));
      if (IsChecked.HasValue && IsChecked.Value) {
         var dotDiameter = RadioDotEffectSize / 2;
         context.DrawEllipse(RadioInnerBackground, null, radioRect.Center, dotDiameter, dotDiameter);
      }
      
   }

   public bool HitTest(Point point)
   {
      return true;
   }
   
   #region IControlCustomStyle 实现
   void IControlCustomStyle.SetupUi()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.SetupTransitions();
      
      RadioDotEffectSize = CalculateDotSize(IsEnabled, IsChecked.HasValue && IsChecked.Value);
   }
   
   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(RadioBorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Style,
                                            new RenderScaleAwareThicknessConfigure(this));
      BindUtils.CreateTokenBinding(this, RadioBorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Template);
   }
   
   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, DotSizeValueTokenProperty, RadioButtonResourceKey.DotSize);
      BindUtils.CreateTokenBinding(this, DotPaddingValueTokenProperty, RadioButtonResourceKey.DotPadding);
   }

   void IControlCustomStyle.CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }

      if (IsChecked.HasValue && IsChecked.Value) {
         _styleState |= ControlStyleState.On;
      } else {
         _styleState |= ControlStyleState.Off;
      }
   }

   private double CalculateDotSize(bool isEnabled, bool isChecked)
   {
      double targetValue;
      if (isChecked) {
         if (isEnabled) {
            targetValue = _dotSizeValueToken;
         } else {
            targetValue = RadioSize - _dotPaddingValueToken * 2;
         }
      } else {
         targetValue = _dotSizeValueToken * 0.6;
      }
      return targetValue;
   }

   void IControlCustomStyle.SetupTransitions()
   {
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(RadioBorderBrushProperty),
         AnimationUtils.CreateTransition<DoubleTransition>(RadioDotEffectSizeProperty),
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(RadioBackgroundProperty, GlobalResourceKey.MotionDurationFast)
      };
   }

   // Measure 之后才有值
   private Rect RadioRect()
   {
      var offsetY = (DesiredSize.Height - Margin.Top - Margin.Bottom - RadioSize) / 2;
      return new Rect(0d, offsetY, RadioSize, RadioSize);
   }

   private Rect RadioTextRect()
   {
      var offsetX = RadioSize + PaddingInline;
      return new Rect(offsetX, 0d, DesiredSize.Width - offsetX, DesiredSize.Height);
   }
   
   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsPointerOverProperty ||
          e.Property == IsCheckedProperty ||
          e.Property == IsEnabledProperty) {
         _customStyle.CollectStyleState();
         if (VisualRoot is not null) {
            RadioDotEffectSize = CalculateDotSize(IsEnabled, IsChecked.HasValue && IsChecked.Value);
         }
         if (e.Property == IsCheckedProperty && 
             _styleState.HasFlag(ControlStyleState.Enabled) &&
             _styleState.HasFlag(ControlStyleState.On)) {
            WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.CircleWave);
         }
      }
   }

   public Rect WaveGeometry()
   {
      return RadioRect();
   }
   
   public CornerRadius WaveBorderRadius()
   {
      return new CornerRadius(RadioSize / 2);
   }
   #endregion
}