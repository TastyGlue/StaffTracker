namespace Dismissal_Appointment.Components.Pages.Abstract;

public static class Themes
{
    public static readonly MudTheme DefaultTheme = new()
    {
        PaletteLight = new()
        {
            // Primary blue theme - Professional and trustworthy
            Primary = "#1976D2",
            PrimaryDarken = "#1565C0",
            PrimaryLighten = "#42A5F5",

            // Secondary colors - Subtle gray for balance
            Secondary = "#455A64",
            Tertiary = "#0288D1",

            // Background colors - Clean and bright
            Background = "#FFFFFF",
            BackgroundGray = "#F5F7FA",
            Surface = "#F7F7F7",

            // AppBar and Drawer - Slight contrast
            AppbarBackground = "#FAFBFC",
            DrawerBackground = "#FFFFFF",

            // Text colors - High readability
            TextPrimary = "#212121",
            TextSecondary = "#616161",
            TextDisabled = "#9E9E9E",

            // Action colors
            ActionDefault = "#616161",
            ActionDisabled = "#BDBDBD",
            ActionDisabledBackground = "#F5F5F5",

            // Divider - Subtle separation
            Divider = "#E0E0E0",
            DividerLight = "#EEEEEE",

            // Status colors - Clear and distinguishable
            Success = "#2E7D32",
            Warning = "#F57C00",
            Error = "#D32F2F",
            Info = "#0288D1",

            // Hover states
            HoverOpacity = 0.08,

            // Lines and borders
            LinesDefault = "#E0E0E0",
            LinesInputs = "#BDBDBD",

            TableStriped = "#F5F5F5",
            TableHover = "#e0edfb"
        },

        LayoutProperties = new()
        {
            DefaultBorderRadius = "8px",
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "260px",
            DrawerMiniWidthLeft = "56px",
            DrawerMiniWidthRight = "56px",
            AppbarHeight = "48px"
        },

        Typography = new()
        {
            Default = new DefaultTypography()
            {
                FontFamily = ["-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "sans-serif"],
                FontSize = "1rem",
                FontWeight = "400",
                LineHeight = "1.5",
                LetterSpacing = "normal"
            },
            H1 = new H1Typography()
            {
                FontSize = "3rem",
                FontWeight = "700",
                LineHeight = "1.167",
                LetterSpacing = "-0.01em"
            },
            H2 = new H2Typography()
            {
                FontSize = "1.875rem",
                FontWeight = "700",
                LineHeight = "1.2",
                LetterSpacing = "normal"
            },
            H3 = new H3Typography()
            {
                FontSize = "1.5rem",
                FontWeight = "700",
                LineHeight = "1.167",
                LetterSpacing = "0"
            },
            H4 = new H4Typography()
            {
                FontSize = "1.25rem",
                FontWeight = "600",
                LineHeight = "1.235",
                LetterSpacing = "normal"
            },
            H5 = new H5Typography()
            {
                FontSize = "1.125rem",
                FontWeight = "600",
                LineHeight = "1.334",
                LetterSpacing = "0"
            },
            H6 = new H6Typography()
            {
                FontSize = "1rem",
                FontWeight = "600",
                LineHeight = "1.6",
                LetterSpacing = "normal"
            },
            Subtitle1 = new Subtitle1Typography()
            {
                FontSize = "1rem",
                FontWeight = "500",
                LineHeight = "1.75",
                LetterSpacing = "normal"
            },
            Subtitle2 = new Subtitle2Typography()
            {
                FontSize = "0.875rem",
                FontWeight = "500",
                LineHeight = "1.57",
                LetterSpacing = "normal"
            },
            Body1 = new Body1Typography()
            {
                FontSize = "1rem",
                FontWeight = "400",
                LineHeight = "1.5",
                LetterSpacing = "normal"
            },
            Body2 = new Body2Typography()
            {
                FontSize = "0.875rem",
                FontWeight = "400",
                LineHeight = "1.43",
                LetterSpacing = "normal"
            },
            Button = new ButtonTypography()
            {
                FontSize = "0.875rem",
                FontWeight = "500",
                LineHeight = "1.75",
                LetterSpacing = "0.02857em",
                TextTransform = "uppercase"
            },
            Caption = new CaptionTypography()
            {
                FontSize = "0.75rem",
                FontWeight = "400",
                LineHeight = "1.66",
                LetterSpacing = "normal"
            },
            Overline = new OverlineTypography()
            {
                FontSize = "0.75rem",
                FontWeight = "600",
                LineHeight = "2.66",
                LetterSpacing = "0.08333em",
                TextTransform = "uppercase"
            }
        },

        ZIndex = new()
        {
            Drawer = 1100,
            AppBar = 1300,
            Dialog = 1400,
            Popover = 1200,
            Snackbar = 1500,
            Tooltip = 1600
        },

        Shadows = new()
        {
            Elevation =
            [
                "none",
                "0 1px 3px rgba(0, 0, 0, 0.08)",
                "0 2px 4px rgba(0, 0, 0, 0.08)",
                "0 3px 6px rgba(0, 0, 0, 0.09)",
                "0 4px 8px rgba(0, 0, 0, 0.10)",
                "0 6px 12px rgba(0, 0, 0, 0.10)",
                "0 8px 16px rgba(0, 0, 0, 0.11)",
                "0 10px 20px rgba(0, 0, 0, 0.11)",
                "0 12px 24px rgba(0, 0, 0, 0.12)",
                "0 14px 28px rgba(0, 0, 0, 0.12)",
                "0 16px 32px rgba(0, 0, 0, 0.13)",
                "0 18px 36px rgba(0, 0, 0, 0.13)",
                "0 20px 40px rgba(0, 0, 0, 0.14)",
                "0 22px 44px rgba(0, 0, 0, 0.14)",
                "0 24px 48px rgba(0, 0, 0, 0.15)",
                "0 26px 52px rgba(0, 0, 0, 0.15)",
                "0 28px 56px rgba(0, 0, 0, 0.16)",
                "0 30px 60px rgba(0, 0, 0, 0.16)",
                "0 32px 64px rgba(0, 0, 0, 0.17)",
                "0 34px 68px rgba(0, 0, 0, 0.17)",
                "0 36px 72px rgba(0, 0, 0, 0.18)",
                "0 38px 76px rgba(0, 0, 0, 0.18)",
                "0 40px 80px rgba(0, 0, 0, 0.19)",
                "0 42px 84px rgba(0, 0, 0, 0.19)",
                "0 44px 88px rgba(0, 0, 0, 0.20)",
                "0 8px 16px rgba(0, 0, 0, 0.11)"
            ]
        }
    };
}
