using CommunityToolkit.Mvvm.ComponentModel;

namespace FluentFlyout.Classes.Settings;

/**
 * User Settings data model.
 */
public partial class UserSettings : ObservableObject
{
    /// <summary>
    /// Use a compact layout
    /// </summary>
    [ObservableProperty] public partial bool CompactLayout { get; set; }

    /// <summary>
    /// Flyout position on screen
    /// </summary>
    [ObservableProperty] public partial int Position { get; set; }

    /// <summary>
    /// Scale for flyout animation speed
    /// </summary>
    [ObservableProperty] public partial int FlyoutAnimationSpeed { get; set; }

    /// <summary>
    /// Show player information in the flyout
    /// </summary>
    [ObservableProperty] public partial bool PlayerInfoEnabled { get; set; }

    /// <summary>
    /// Enable repeat button
    /// </summary>
    [ObservableProperty] public partial bool RepeatEnabled { get; set; }

    /// <summary>
    /// Enable shuffle button
    /// </summary>
    [ObservableProperty] public partial bool ShuffleEnabled { get; set; }

    /// <summary>
    /// Start minimized to tray when Windows starts
    /// </summary>
    [ObservableProperty] public partial bool Startup { get; set; }

    /// <summary>
    /// Flyout display duration (milliseconds)
    /// </summary>
    [ObservableProperty] public partial int Duration { get; set; }

    /// <summary>
    /// Enable the 'Next Up' flyout (experimental)
    /// </summary>
    [ObservableProperty] public partial bool NextUpEnabled { get; set; }

    /// <summary>
    /// 'Next Up' flyout display duration (milliseconds)
    /// </summary>
    [ObservableProperty] public partial int NextUpDuration { get; set; }

    /// <summary>
    /// Tray icon left-click behavior
    /// </summary>
    [ObservableProperty] public partial int NIconLeftClick { get; set; }

    /// <summary>
    /// Center the title and artist text
    /// </summary>
    [ObservableProperty] public partial bool CenterTitleArtist { get; set; }

    /// <summary>
    /// Animation easing style index
    /// </summary>
    [ObservableProperty] public partial int FlyoutAnimationEasingStyle { get; set; }

    /// <summary>
    /// Enable lock keys flyout (shows Caps/Num/Scroll status)
    /// </summary>
    [ObservableProperty] public partial bool LockKeysEnabled { get; set; }

    /// <summary>
    /// Lock keys flyout display duration (milliseconds)
    /// </summary>
    [ObservableProperty] public partial int LockKeysDuration { get; set; }

    /// <summary>
    /// App theme
    /// </summary>
    [ObservableProperty] public partial int AppTheme { get; set; }

    /// <summary>
    /// Enable media flyout
    /// </summary>
    [ObservableProperty] public partial bool MediaFlyoutEnabled { get; set; }

    /// <summary>
    /// Use symbol-style tray icon
    /// </summary>
    [ObservableProperty] public partial bool NIconSymbol { get; set; }

    /// <summary>
    /// Disable flyout when a DirectX exclusive fullscreen app is detected
    /// </summary>
    [ObservableProperty] public partial bool DisableIfFullscreen { get; set; }

    /// <summary>
    /// Use bold symbol and font in the lock keys flyout
    /// </summary>
    [ObservableProperty] public partial bool LockKeysBoldUi { get; set; }

    /// <summary>
    /// Determines if the user has updated to a new version
    /// </summary>
    [ObservableProperty]
    public partial string LastKnownVersion { get; set; }

    /// <summary>
    /// Show seekbar if the player supports it
    /// </summary>
    [ObservableProperty] public partial bool SeekbarEnabled { get; set; }

    /// <summary>
    /// Pause other media sessions when focusing a new one
    /// </summary>
    [ObservableProperty]
    public partial bool PauseOtherSessionsEnabled { get; set; }

    /// <summary>
    /// Show LockKeys flyout when the Insert key is pressed
    /// </summary>
    [ObservableProperty]
    public partial bool LockKeysInsertEnabled { get; set; }

    /// <summary>
    /// Preset for media flyout background blur styles
    /// </summary>
    [ObservableProperty]
    public partial int MediaFlyoutBackgroundBlur { get; set; }

    /// <summary>
    /// Enable acrylic blur effect on the flyout window
    /// </summary>
    [ObservableProperty]
    public partial bool MediaFlyoutAcrylicWindowEnabled { get; set; }

    /// <summary>
    /// User's preferred app language (e.g., "system" for system default)
    /// </summary>
    [ObservableProperty]
    public partial string AppLanguage { get; set; }


    public UserSettings()
    {
        CompactLayout = false;
        Position = 0;
        FlyoutAnimationSpeed = 2;
        PlayerInfoEnabled = true;
        RepeatEnabled = false;
        ShuffleEnabled = false;
        Startup = true;
        Duration = 3000;
        NextUpEnabled = false;
        NextUpDuration = 2000;
        NIconLeftClick = 0;
        CenterTitleArtist = false;
        FlyoutAnimationEasingStyle = 2;
        LockKeysEnabled = true;
        LockKeysDuration = 2000;
        AppTheme = 0;
        MediaFlyoutEnabled = true;
        NIconSymbol = false;
        DisableIfFullscreen = true;
        LockKeysBoldUi = true;
        LastKnownVersion = "";
        SeekbarEnabled = false;
        PauseOtherSessionsEnabled = false;
        LockKeysInsertEnabled = true;
        MediaFlyoutBackgroundBlur = 0;
        MediaFlyoutAcrylicWindowEnabled = true;
        AppLanguage = "system";
    }
}