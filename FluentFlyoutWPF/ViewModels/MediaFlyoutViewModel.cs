using CommunityToolkit.Mvvm.ComponentModel;
using FluentFlyout.Classes;

namespace FluentFlyout.ViewModels;

public class MediaFlyoutViewModel : ObservableObject
{
    public UserSettings Settings { get; } = SettingsManager.Current;
    
    
}