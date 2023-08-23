using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AXAMLColorGenerator.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private Color? _selectedColor = Colors.Coral;
    [ObservableProperty] private string _colorName = string.Empty;
    [ObservableProperty] private string _numberOfShades = "10";
    [ObservableProperty] private string _filePath = string.Empty;
    [ObservableProperty] private string _axamlContent = string.Empty;

    public bool SaveColorShades(Color? colorRightNow)
    {
        // Generate lighter shades of the selected color
        (List<Color> colorShades, bool ok) = GenerateColorShades(colorRightNow.GetValueOrDefault());
        colorShades.Reverse();
        
        // Convert the color shades to .axaml
        AxamlContent = ConvertToAxaml(colorShades);

        return ok;
    }

    private (List<Color>, bool ok )GenerateColorShades(Color? baseColor)
    {
        List<Color> colorShades = new List<Color>();
        bool ok = int.TryParse(NumberOfShades, out int shades);
        if (!ok) return (colorShades, ok);
        
        // Generate shades
        for (int i = 1; i <= shades ; i++)
        {
            double factor = i * (1.0 / shades);
            
            if (baseColor == null) continue;
            
            Color lighterColor = Color.FromArgb(
                baseColor.Value.A,
                (byte)(baseColor.Value.R + (255 - baseColor.Value.R) * factor),
                (byte)(baseColor.Value.G + (255 - baseColor.Value.G) * factor),
                (byte)(baseColor.Value.B + (255 - baseColor.Value.B) * factor)
            );
            colorShades.Add(lighterColor);
        }

        return (colorShades, ok);
    }

    private string ConvertToAxaml(List<Color> colorShades)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<ResourceDictionary xmlns=\"https://github.com/avaloniaui\"");
        sb.AppendLine("                    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");

        for (var i = 0; i < colorShades.Count; i++)
        {
            var color = colorShades[i];
            var hexColor = color.ToHexString();
            sb.AppendLine($"    <Color x:Key=\"{_colorName}-{i * 100}\">{hexColor}</Color>");
        }

        sb.AppendLine("</ResourceDictionary>");

        return sb.ToString();
    }
}

public static class ColorExtensions
{
    public static string ToHexString(this Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}
