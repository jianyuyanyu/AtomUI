﻿using AtomUI.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class DashboardProgressTheme : AbstractCircleProgressTheme
{
   public DashboardProgressTheme() : base(typeof(DashboardProgress)) {}
}