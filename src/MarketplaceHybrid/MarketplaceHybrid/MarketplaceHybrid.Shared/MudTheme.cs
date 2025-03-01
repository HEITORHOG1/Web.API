using MudBlazor;

namespace MarketplaceHybrid.Shared
{
    public class AppTheme
    {
        public static MudTheme DefaultTheme => new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#ea1d2c",          // iFood Red
                PrimaryDarken = "#c1121f",    // Darker Red for hover states
                Secondary = "#3f3f3f",        // Dark Gray
                SecondaryDarken = "#2d2d2d",  // Darker Gray
                Tertiary = "#4caf50",         // Success Green

                AppbarBackground = "#ea1d2c",
                Background = "#f5f5f5",       // Light Gray background
                Surface = "#ffffff",          // White

                DrawerBackground = "#ffffff",
                DrawerText = "rgba(0,0,0, 0.7)",

                Success = "#4caf50",
                Error = "#ea1d2c",
                Warning = "#ff9800",
                Info = "#2196f3",

                TextPrimary = "rgba(0,0,0, 0.87)",
                TextSecondary = "rgba(0,0,0, 0.6)",

                ActionDefault = "#adadad",
                ActionDisabled = "rgba(0,0,0, 0.26)",
                ActionDisabledBackground = "rgba(0,0,0, 0.12)"
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
                },
                Button = new Button
                {
                    FontWeight = 500,
                    FontSize = "0.875rem",
                    TextTransform = "none"
                },
                Subtitle1 = new Subtitle1
                {
                    FontSize = "1rem",
                    FontWeight = 500,
                    LineHeight = 1.5,
                    LetterSpacing = ".00938em"
                }
            },

            Shadows = new Shadow(),

            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "4px",
                DrawerWidthLeft = "260px",
                DrawerMiniWidthLeft = "56px"
            }
        };
    }
}
