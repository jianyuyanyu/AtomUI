using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Demo.Desktop.ShowCase;

public class ShowCaseItem : ContentControl
{
   private bool _initialized = false;
   public string Title { get; set; } = string.Empty;
   public string Description { get; set; } = string.Empty;

   private void SetupUi()
   {
      var mainLayout = new StackPanel();
      var showCaseTitle = new AtomUI.Controls.Separator()
      {
         Title = Title,
         TitlePosition = AtomUI.Controls.SeparatorTitlePosition.Left,
         FontWeight = FontWeight.Bold,
      };

      if (Content is Control contentControl) {
         LogicalChildren.Remove(contentControl);
         mainLayout.Children.Add(contentControl);
      }
      
      mainLayout.Children.Add(new Border
      {
         Height = 10,
         Background = Brushes.Transparent
      });
      mainLayout.Children.Add(showCaseTitle);
      mainLayout.Children.Add(new TextBlock()
      {
         Text = Description,
         TextWrapping = TextWrapping.Wrap,
         Margin = new Thickness(0, 10, 0, 0)
      });
      
      var outerBorder = new Border()
      {
         BorderBrush = new SolidColorBrush(new Color(10, 5, 5, 5)),
         BorderThickness = new Thickness(1),
         Padding = new Thickness(20),
         Child = mainLayout,
         CornerRadius = new CornerRadius(8)
      };
      
      Content = outerBorder;
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      if (!_initialized) {
         SetupUi();
         _initialized = true;
      }
   }
}