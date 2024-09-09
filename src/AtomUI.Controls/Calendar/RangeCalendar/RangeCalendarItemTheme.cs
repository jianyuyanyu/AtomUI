﻿using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RangeCalendarItemTheme : BaseControlTheme
{
   public const string ItemFramePart = "PART_ItemFrame";
   public const string ItemRootLayoutPart = "PART_ItemRootLayout";
   public const string MonthViewPart = "PART_MonthView";
   public const string PrimaryMonthViewPart = "PART_PrimaryMonthView";
   public const string SecondaryMonthViewPart = "PART_SecondaryMonthView";
   public const string YearViewPart = "PART_YearView";
   public const string HeaderLayoutPart = "PART_HeaderLayout";
   
   public const string PreviousButtonPart = "PART_PreviousButton";
   public const string PreviousMonthButtonPart = "PART_PreviousMonthButton";
   public const string HeaderButtonPart = "PART_HeaderButton";
   public const string NextMonthButtonPart = "PART_NextMonthButton";
   public const string NextButtonPart = "PART_NextButton";
   
   public RangeCalendarItemTheme()
      : base(typeof(RangeCalendarItem)) { }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<RangeCalendarItem>((calendarItem, scope) =>
      {
         var frame = new Border()
         {
            Name = ItemFramePart
         };

         var rootLayout = new DockPanel()
         {
            Name = ItemRootLayoutPart,
            LastChildFill = true
         };
         BuildHeader(rootLayout, scope);
         BuildContentView(rootLayout, scope);

         BuildDayTitleTemplate(calendarItem);
         frame.Child = rootLayout;
         
         return frame;
      });
   }

   private void BuildDayTitleTemplate(RangeCalendarItem calendarItem)
   {
      calendarItem.DayTitleTemplate = new DayTitleTemplate();
   }

   protected virtual void BuildHeader(DockPanel layout, INameScope scope)
   {
      var headerLayout = new Grid()
      {
         Name = HeaderLayoutPart,
         ColumnDefinitions = new ColumnDefinitions()
         {
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Auto),
         }
      };

      var previousButton = BuildPreviousButton();
      previousButton.RegisterInNameScope(scope);
      Grid.SetColumn(previousButton, 0);
      headerLayout.Children.Add(previousButton);

      var previousMonthButton = BuildPreviousMonthButton();
      previousMonthButton.RegisterInNameScope(scope);
      Grid.SetColumn(previousMonthButton, 1);
      headerLayout.Children.Add(previousMonthButton);
      
      var headerButton = new HeadTextButton()
      {
         Name = HeaderButtonPart
      };
      Grid.SetColumn(headerButton, 2);
      headerButton.RegisterInNameScope(scope);
      headerLayout.Children.Add(headerButton);

      var nextMonthButton = BuildNextMonthButton();
      Grid.SetColumn(nextMonthButton, 3);
      nextMonthButton.RegisterInNameScope(scope);
      headerLayout.Children.Add(nextMonthButton);

      var nextButton = BuildNextButton();
      
      Grid.SetColumn(nextButton, 4);
      nextButton.RegisterInNameScope(scope);
      headerLayout.Children.Add(nextButton);
      
      DockPanel.SetDock(headerLayout, Dock.Top);
      layout.Children.Add(headerLayout);
   }

   protected virtual IconButton BuildPreviousButton()
   {
      var previousButtonIcon = new PathIcon()
      {
         Kind = "DoubleLeftOutlined"
      };
      TokenResourceBinder.CreateGlobalTokenBinding(previousButtonIcon, PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorTextDescription);
      TokenResourceBinder.CreateGlobalTokenBinding(previousButtonIcon, PathIcon.ActiveFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      TokenResourceBinder.CreateGlobalTokenBinding(previousButtonIcon, PathIcon.SelectedFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      
      var previousButton = new IconButton()
      {
         Name = PreviousButtonPart,
         Icon = previousButtonIcon
      };

      TokenResourceBinder.CreateGlobalTokenBinding(previousButton, IconButton.IconWidthProperty, GlobalTokenResourceKey.IconSizeSM);
      TokenResourceBinder.CreateGlobalTokenBinding(previousButton, IconButton.IconHeightProperty, GlobalTokenResourceKey.IconSizeSM);
      return previousButton;
   }

   protected virtual IconButton BuildPreviousMonthButton()
   {
      var previousMonthButtonIcon = new PathIcon()
      {
         Kind = "LeftOutlined"
      };
      TokenResourceBinder.CreateGlobalTokenBinding(previousMonthButtonIcon, PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorTextDescription);
      TokenResourceBinder.CreateGlobalTokenBinding(previousMonthButtonIcon, PathIcon.ActiveFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      TokenResourceBinder.CreateGlobalTokenBinding(previousMonthButtonIcon, PathIcon.SelectedFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      
      var previousMonthButton = new IconButton()
      {
         Name = PreviousMonthButtonPart,
         Icon = previousMonthButtonIcon
      };

      TokenResourceBinder.CreateGlobalTokenBinding(previousMonthButton, IconButton.IconWidthProperty, GlobalTokenResourceKey.IconSizeSM);
      TokenResourceBinder.CreateGlobalTokenBinding(previousMonthButton, IconButton.IconHeightProperty, GlobalTokenResourceKey.IconSizeSM);
      return previousMonthButton;
   }
   
   protected virtual IconButton BuildNextButton()
   {
      var nextButtonIcon = new PathIcon()
      {
         Kind = "DoubleRightOutlined"
      };
      TokenResourceBinder.CreateGlobalTokenBinding(nextButtonIcon, PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorTextDescription);
      TokenResourceBinder.CreateGlobalTokenBinding(nextButtonIcon, PathIcon.ActiveFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      TokenResourceBinder.CreateGlobalTokenBinding(nextButtonIcon, PathIcon.SelectedFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      
      var nextButton = new IconButton()
      {
         Name = NextButtonPart,
         Icon = nextButtonIcon
      };
      TokenResourceBinder.CreateGlobalTokenBinding(nextButton, IconButton.IconWidthProperty, GlobalTokenResourceKey.IconSizeSM);
      TokenResourceBinder.CreateGlobalTokenBinding(nextButton, IconButton.IconHeightProperty, GlobalTokenResourceKey.IconSizeSM);
      return nextButton;
   }

   protected virtual IconButton BuildNextMonthButton()
   {
      var nextMonthButtonIcon = new PathIcon()
      {
         Kind = "RightOutlined"
      };
      TokenResourceBinder.CreateGlobalTokenBinding(nextMonthButtonIcon, PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorTextDescription);
      TokenResourceBinder.CreateGlobalTokenBinding(nextMonthButtonIcon, PathIcon.ActiveFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      TokenResourceBinder.CreateGlobalTokenBinding(nextMonthButtonIcon, PathIcon.SelectedFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      var nextMonthButton = new IconButton()
      {
         Name = NextMonthButtonPart,
         Icon = nextMonthButtonIcon
      };
      TokenResourceBinder.CreateGlobalTokenBinding(nextMonthButton, IconButton.IconWidthProperty, GlobalTokenResourceKey.IconSizeSM);
      TokenResourceBinder.CreateGlobalTokenBinding(nextMonthButton, IconButton.IconHeightProperty, GlobalTokenResourceKey.IconSizeSM);
      return nextMonthButton;
   }

   private void BuildContentView(DockPanel layout, INameScope scope)
   {
      var monthView = new UniformGrid()
      {
         Name = MonthViewPart,
         Columns = 2,
         IsVisible = false
      };

      var primaryMonthView = BuildMonthViewItem(PrimaryMonthViewPart);
      primaryMonthView.RegisterInNameScope(scope);
      monthView.Children.Add(primaryMonthView);
      
      var secondaryMonthView = BuildMonthViewItem(SecondaryMonthViewPart);
      secondaryMonthView.RegisterInNameScope(scope);
      monthView.Children.Add(secondaryMonthView);

      BindUtils.RelayBind(monthView, UniformGrid.IsVisibleProperty, primaryMonthView, Grid.IsVisibleProperty);
      BindUtils.RelayBind(monthView, UniformGrid.IsVisibleProperty, secondaryMonthView, Grid.IsVisibleProperty);
    
      monthView.RegisterInNameScope(scope);
      layout.Children.Add(monthView);

      var yearView = new Grid()
      {
         Name = YearViewPart,
         IsVisible = false,
         RowDefinitions = new RowDefinitions()
         {
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
         },
         ColumnDefinitions = new ColumnDefinitions()
         {
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
         }
      };
      yearView.RegisterInNameScope(scope);
      layout.Children.Add(yearView);
   }

   private Grid BuildMonthViewItem(string name)
   {
      var dayTitleRowDef = new RowDefinition();
      TokenResourceBinder.CreateTokenBinding(dayTitleRowDef, RowDefinition.HeightProperty, CalendarTokenResourceKey.DayTitleHeight);
      var monthView = new Grid()
      {
         Name = name,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         IsVisible = false,
         RowDefinitions = new RowDefinitions()
         {
            dayTitleRowDef,
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
         },
         ColumnDefinitions = new ColumnDefinitions()
         {
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
         }
      };
      return monthView;
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());

      var headerLayoutStyle = new Style(selector => selector.Nesting().Template().Name(HeaderLayoutPart));
      headerLayoutStyle.Add(Grid.MarginProperty, CalendarTokenResourceKey.HeaderMargin);
      
      commonStyle.Add(headerLayoutStyle);
      
      Add(commonStyle);
   }
}