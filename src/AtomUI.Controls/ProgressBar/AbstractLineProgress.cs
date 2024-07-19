using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Layout;

namespace AtomUI.Controls;

public enum LinePercentAlignment
{
   Start,
   Center,
   End
}

internal class SizeTypeThresholdValue
{
   internal double NormalStateValue { get; set; } 
   internal double InnerStateValue { get; set; }
}

[PseudoClasses(VerticalPC, HorizontalPC)]
public abstract partial class AbstractLineProgress : AbstractProgressBar
{
   public const string VerticalPC = ":vertical";
   public const string HorizontalPC = ":horizontal";
   
   /// <summary>
   /// Defines the <see cref="Orientation"/> property.
   /// </summary>
   public static readonly StyledProperty<Orientation> OrientationProperty =
      AvaloniaProperty.Register<AbstractProgressBar, Orientation>(nameof(Orientation), Orientation.Horizontal);
   
   /// <summary>
   /// Gets or sets the orientation of the <see cref="ProgressBar"/>.
   /// </summary>
   public Orientation Orientation
   {
      get => GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
   }
   
   internal Dictionary<SizeType, SizeTypeThresholdValue> _sizeTypeThresholdValue;
   protected Size _extraInfoSize = Size.Infinity;
   protected Rect _grooveRect;

   public AbstractLineProgress()
   {
      _sizeTypeThresholdValue = new Dictionary<SizeType, SizeTypeThresholdValue>();
   }
   
   // 根据当前的状态进行计算
   protected virtual Size CalculateExtraInfoSize(double fontSize)
   {
      if (Status == ProgressStatus.Exception || MathUtils.AreClose(Value, Maximum)) {
         // 只要图标
         if (EffectiveSizeType == SizeType.Large || EffectiveSizeType == SizeType.Middle) {
            return new Size(_lineInfoIconSizeToken, _lineInfoIconSizeToken);
         } 
         return new Size(_lineInfoIconSizeSMToken, _lineInfoIconSizeSMToken);
      }
      var textSize = TextUtils.CalculateTextSize(string.Format(ProgressTextFormat, Value), FontFamily, fontSize);
      return textSize;
   }

   protected override void NotifyUiStructureReady()
   {
      base.NotifyUiStructureReady();
      var calculateEffectiveSize = false;
      double sizeValue = 0;
      if (Orientation == Orientation.Horizontal && !double.IsNaN(Height)) {
         sizeValue = Height;
         calculateEffectiveSize = true;
      } else if (Orientation == Orientation.Vertical && !double.IsNaN(Width)) {
         sizeValue = Width;
         calculateEffectiveSize = true;
      }

      if (calculateEffectiveSize) {
         EffectiveSizeType = CalculateEffectiveSizeType(sizeValue);
      }
    
      _extraInfoSize = CalculateExtraInfoSize(FontSize);
      SetupAlignment();
      NotifyOrientationChanged();
   }
   
   protected virtual void SetupAlignment()
   {
      HorizontalAlignment = HorizontalAlignment.Left;
      VerticalAlignment = VerticalAlignment.Top;
   }
   
   private void UpdatePseudoClasses()
   {
      PseudoClasses.Set(VerticalPC, Orientation == Orientation.Vertical);
      PseudoClasses.Set(HorizontalPC, Orientation == Orientation.Horizontal);
   }

   protected override void NotifyPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.NotifyPropertyChanged(e);
      if (e.Property == HeightProperty || e.Property == WidthProperty) {
         SetupAlignment();
      }
      if (_initialized) {
         if ((e.Property == WidthProperty && Orientation == Orientation.Vertical) ||
             (e.Property == HeightProperty && Orientation == Orientation.Horizontal)) {
            EffectiveSizeType = CalculateEffectiveSizeType(e.GetNewValue<double>());
            CalculateStrokeThickness();
         } else if (e.Property == EffectiveSizeTypeProperty) {
            _extraInfoSize = CalculateExtraInfoSize(FontSize);
         } else if (e.Property == OrientationProperty) {
            NotifyOrientationChanged();
         }
      }
   }

   protected override void NotifyApplyFixedStyleConfig()
   {
      base.NotifyApplyFixedStyleConfig();
      BindUtils.CreateTokenBinding(this, LineProgressPaddingTokenProperty, ProgressBarResourceKey.LineProgressPadding);
      BindUtils.CreateTokenBinding(this, LineExtraInfoMarginTokenProperty, ProgressBarResourceKey.LineExtraInfoMargin);
      BindUtils.CreateTokenBinding(this, FontSizeTokenProperty, GlobalResourceKey.FontSize);
      BindUtils.CreateTokenBinding(this, FontSizeSMTokenProperty, GlobalResourceKey.FontSizeSM);
      
      BindUtils.CreateTokenBinding(this, LineInfoIconSizeTokenProperty, ProgressBarResourceKey.LineInfoIconSize);
      BindUtils.CreateTokenBinding(this, LineInfoIconSizeSMTokenProperty, ProgressBarResourceKey.LineInfoIconSizeSM);
   }

   protected override void NotifyTemplateApplied(INameScope scope)
   {
      _exceptionCompletedIcon = scope.Find<PathIcon>(AbstractProgressBarTheme.ExceptionCompletedIconPart);
      _successCompletedIcon = scope.Find<PathIcon>(AbstractProgressBarTheme.SuccessCompletedIconPart);
      base.NotifyTemplateApplied(scope);
   }
   
   protected virtual void NotifyOrientationChanged()
   {
      UpdatePseudoClasses();
   }
   
   private bool _lastCompletedStatus = false;
   protected override void NotifyHandleExtraInfoVisibility()
   {
      base.NotifyHandleExtraInfoVisibility();
      var currentStatus = false;
      if (MathUtils.AreClose(Value, Maximum)) {
         currentStatus = true;
        
      } else {
         currentStatus = false;
      }

      if (currentStatus != _lastCompletedStatus) {
         _lastCompletedStatus = currentStatus;
         _extraInfoSize = CalculateExtraInfoSize(FontSize);
      }
   }
}