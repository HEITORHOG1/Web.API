using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceHybrid.Shared
{
    public class AppTheme
    {
        public static MudTheme DefaultTheme => new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#ea1d2c",
                PrimaryDarken = "#c1121f",
                Secondary = "#3f3f3f",
                AppbarBackground = "#ea1d2c",
                Background = "#f5f5f5",
                Surface = "#ffffff",
                DrawerBackground = "#ffffff",
                DrawerText = "rgba(0,0,0, 0.7)",
                Success = "#4caf50",
                Error = "#ea1d2c",
            },
            Typography = new Typography
            {
                Default = new Default
                {
                    FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                    FontSize = "0.875rem",
                    FontWeight = 400,
                    LineHeight = 1.43,
                    LetterSpacing = ".01071em"
                },
                H6 = new H6
                {
                    FontWeight = 600,
                }
            }
        };
    }
}
