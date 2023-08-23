using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AXAMLColorGenerator.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace AXAMLColorGenerator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel) return;

        bool ok = viewModel.SaveColorShades(MyColorPicker.Color);
        if (!ok)
        {
            await MessageBoxManager.GetMessageBoxStandard("", "Invalid number, try again.",
                ButtonEnum.Ok).ShowAsync();
            return;
        }
        
        string axaml = viewModel.AxamlContent;
        
        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;
        
        IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save .axaml file",
            SuggestedFileName = viewModel.ColorName + ".axaml",
        });

        if (file is null) return;
        
        await using Stream stream = await file.OpenWriteAsync();
        await using StreamWriter streamWriter = new StreamWriter(stream);
        await streamWriter.WriteLineAsync(axaml);

        if (streamWriter.WriteLineAsync().IsCompleted)
        {
            await MessageBoxManager.GetMessageBoxStandard("Information", "The file has been saved successfully.", ButtonEnum.Ok).ShowAsync();
        }
    }
}

