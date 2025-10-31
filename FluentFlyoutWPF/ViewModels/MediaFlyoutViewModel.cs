using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentFlyout.Classes;
using FluentFlyout.Controls;
using System.Runtime.InteropServices;

namespace FluentFlyout.ViewModels;

public partial class MediaFlyoutViewModel : ObservableObject
{
    [DllImport("user32.dll")]
    public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

    [ObservableProperty]
    public partial UserSettings Settings { get; set; } = SettingsManager.Current;

    [RelayCommand]
    private async Task Back()
    {
        if (FluentMediaFlyout.MediaManager.GetFocusedSession() == null)
            return;

        await FluentMediaFlyout.MediaManager.GetFocusedSession().ControlSession.TrySkipPreviousAsync();
    }

    [RelayCommand]
    private void PlayPause()
    {
        keybd_event(0xB3, 0, 0, IntPtr.Zero);
    }

    [RelayCommand]
    private async Task Forward()
    {
        if (FluentMediaFlyout.MediaManager.GetFocusedSession() == null)
            return;

        await FluentMediaFlyout.MediaManager.GetFocusedSession().ControlSession.TrySkipNextAsync();
    }
}