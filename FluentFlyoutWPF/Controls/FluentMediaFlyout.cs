using FluentFlyout.Classes;
using FluentFlyout.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Control;
using Windows.Storage.Streams;
using WindowsMediaController;
using static FluentFlyout.MainWindow;
using static WindowsMediaController.MediaManager;
using MediaManager = WindowsMediaController.MediaManager;

namespace FluentFlyout.Controls
{
    public class FluentMediaFlyout : Control
    {
        public static readonly DependencyProperty CenterTitleArtistProperty =
            DependencyProperty.Register(
                nameof(CenterTitleArtist),
                typeof(bool),
                typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Center the title and artist text
        /// </summary>
        public bool CenterTitleArtist
        {
            get => (bool)GetValue(CenterTitleArtistProperty);
            set => SetValue(CenterTitleArtistProperty, value);
        }


        public static readonly DependencyProperty SeekbarEnabledProperty =
            DependencyProperty.Register(
                nameof(SeekbarEnabled),
                typeof(bool),
                typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                    OnSeekbarEnabledPropertyChanged));

        /// <summary>
        /// Show seekbar if the player supports it
        /// </summary>
        public bool SeekbarEnabled
        {
            get => (bool)GetValue(SeekbarEnabledProperty);
            set => SetValue(SeekbarEnabledProperty, value);
        }


        private static void OnSeekbarEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FluentMediaFlyout)d;
            if (e.NewValue == e.OldValue) return;
            if (e.NewValue is true)
            {
                control.PositionTimer.Change(0, SeekbarUpdateInterval);
            }
            else
            {
                control.PositionTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        public static readonly DependencyProperty CompactLayoutProperty =
            DependencyProperty.Register(nameof(CompactLayout), typeof(bool), typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                    OnLayoutPropertyChanged));

        public bool CompactLayout
        {
            get => (bool)GetValue(CompactLayoutProperty);
            set => SetValue(CompactLayoutProperty, value);
        }

        public static readonly DependencyProperty MediaFlyoutAlwaysDisplayProperty =
            DependencyProperty.Register(nameof(MediaFlyoutAlwaysDisplay), typeof(bool), typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                    OnLayoutPropertyChanged));

        public bool MediaFlyoutAlwaysDisplay
        {
            get => (bool)GetValue(MediaFlyoutAlwaysDisplayProperty);
            set => SetValue(MediaFlyoutAlwaysDisplayProperty, value);
        }

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FluentMediaFlyout)d;
            control.ShowControlClose = control is { CompactLayout: true, MediaFlyoutAlwaysDisplay: true };
            control.ShowTopClose = control is { CompactLayout: false, MediaFlyoutAlwaysDisplay: true };
        }

        public static readonly DependencyProperty ShowControlCloseProperty =
            DependencyProperty.Register(nameof(ShowControlClose), typeof(bool), typeof(FluentMediaFlyout),
                new PropertyMetadata(false));

        public bool ShowControlClose
        {
            get => (bool)GetValue(ShowControlCloseProperty);
            private set => SetValue(ShowControlCloseProperty, value);
        }

        public static readonly DependencyProperty ShowTopCloseProperty =
            DependencyProperty.Register(nameof(ShowTopClose), typeof(bool), typeof(FluentMediaFlyout),
                new PropertyMetadata(false));

        public bool ShowTopClose
        {
            get => (bool)GetValue(ShowTopCloseProperty);
            private set => SetValue(ShowTopCloseProperty, value);
        }

        public bool RepeatEnabled
        {
            get => (bool)GetValue(RepeatEnabledProperty);
            set => SetValue(RepeatEnabledProperty, value);
        }

        public static readonly DependencyProperty RepeatEnabledProperty =
            DependencyProperty.Register(nameof(RepeatEnabled), typeof(bool), typeof(FluentMediaFlyout),
                new PropertyMetadata(false));


        public bool ShuffleEnabled
        {
            get => (bool)GetValue(ShuffleEnabledProperty);
            set
            {
                SetValue(ShuffleEnabledProperty, value);
                Debug.WriteLine(value);
            }
        }

        public static readonly DependencyProperty ShuffleEnabledProperty =
            DependencyProperty.Register(nameof(ShuffleEnabled), typeof(bool), typeof(FluentMediaFlyout),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SeekbarValueProperty =
            DependencyProperty.Register(
                nameof(SeekbarValue),
                typeof(double),
                typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnSeekbarValueChanged));

        public double SeekbarValue
        {
            get => (double)GetValue(SeekbarValueProperty);
            set => SetValue(SeekbarValueProperty, value);
        }

        private static void OnSeekbarValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FluentMediaFlyout control) return;
            var seconds = (double)e.NewValue;
            var ts = TimeSpan.FromSeconds(seconds);
            var format = ts.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss";
            control.SeekbarCurrentDuration = ts.ToString(format);
        }

        public static readonly DependencyPropertyKey SeekbarCurrentDurationPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SeekbarCurrentDuration),
                typeof(string),
                typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata("--:--"));

        public static readonly DependencyProperty SeekbarCurrentDurationProperty =
            SeekbarCurrentDurationPropertyKey.DependencyProperty;

        public string SeekbarCurrentDuration
        {
            get => (string)GetValue(SeekbarCurrentDurationProperty);
            protected set => SetValue(SeekbarCurrentDurationPropertyKey, value);
        }

        public static readonly DependencyProperty SeekbarMaxValueProperty =
            DependencyProperty.Register(
                nameof(SeekbarMaxValue),
                typeof(double),
                typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnSeekbarMaxValueChanged));

        public double SeekbarMaxValue
        {
            get => (double)GetValue(SeekbarMaxValueProperty);
            set => SetValue(SeekbarMaxValueProperty, value);
        }

        private static void OnSeekbarMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FluentMediaFlyout control) return;
            var seconds = (double)e.NewValue;
            var ts = TimeSpan.FromSeconds(seconds);
            var format = ts.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss";
            control.SeekbarMaxDuration = ts.ToString(format);
        }

        public static readonly DependencyPropertyKey SeekbarMaxDurationPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SeekbarMaxDuration),
                typeof(string),
                typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata("--:--"));

        public static readonly DependencyProperty SeekbarMaxDurationProperty =
            SeekbarMaxDurationPropertyKey.DependencyProperty;

        public string SeekbarMaxDuration
        {
            get => (string)GetValue(SeekbarMaxDurationProperty);
            protected set => SetValue(SeekbarMaxDurationPropertyKey, value);
        }

        public static readonly DependencyProperty IsShuffleActiveProperty =
            DependencyProperty.Register(
                nameof(IsShuffleActive),
                typeof(bool),
                typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsShuffleActive
        {
            get => (bool)GetValue(IsShuffleActiveProperty);
            protected set => SetValue(IsShuffleActiveProperty, value);
        }

        public static readonly DependencyProperty SongTitleProperty =
            DependencyProperty.Register(
                nameof(SongTitle),
                typeof(string),
                typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public string SongTitle
        {
            get => (string)GetValue(SongTitleProperty);
            set => SetValue(SongTitleProperty, value);
        }

        public static readonly DependencyProperty SongArtistProperty =
            DependencyProperty.Register(
                nameof(SongArtist),
                typeof(string),
                typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public string SongArtist
        {
            get => (string)GetValue(SongArtistProperty);
            set => SetValue(SongArtistProperty, value);
        }

        public static readonly DependencyProperty SongImageSourceProperty =
            DependencyProperty.Register(
                nameof(SongImageSource),
                typeof(ImageSource),
                typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 当前歌曲封面图像
        /// </summary>
        public ImageSource? SongImageSource
        {
            get => (ImageSource?)GetValue(SongImageSourceProperty);
            set => SetValue(SongImageSourceProperty, value);
        }


        static FluentMediaFlyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FluentMediaFlyout),
                new FrameworkPropertyMetadata(typeof(FluentMediaFlyout)));
        }

        public static readonly MediaManager MediaManager = new();

        private DateTime _lastSelfUpdateTimestamp = DateTime.MinValue;
        private bool _isDragging;

        public readonly Timer PositionTimer;
        private bool _isActive;

        private const int SeekbarUpdateInterval = 300;

        public FluentMediaFlyout()
        {
            MediaManager.Start();
            MediaManager.OnAnyMediaPropertyChanged += InnerOnAnyMediaPropertyChanged;
            MediaManager.OnAnyPlaybackStateChanged += InnerOnPlaybackStateChanged;
            MediaManager.OnAnyTimelinePropertyChanged += InnerOnAnyTimelinePropertyChanged;

            PositionTimer = new Timer(SeekbarUpdateUi, null, Timeout.Infinite, Timeout.Infinite);
            if (SeekbarEnabled && MediaManager.GetFocusedSession() is { } session)
            {
                UpdateSeekbarCurrentDuration(session.ControlSession.GetTimelineProperties().Position);
            }
        }

        private void InnerOnAnyTimelinePropertyChanged(MediaSession mediaSession,
            GlobalSystemMediaTransportControlsSessionTimelineProperties timelineProperties)
        {
            _lastSelfUpdateTimestamp = DateTime.Now;

            if (MediaManager.GetFocusedSession() is not { } session) return;
            Dispatcher.Invoke(() =>
            {
                if (!SeekbarEnabled) return;
                if (!Visible || _isDragging) return;

                UpdateSeekbarCurrentDuration(session.ControlSession.GetTimelineProperties().Position);
                HandlePlayBackState(session.ControlSession.GetPlaybackInfo().PlaybackStatus);
            });
        }

        private void SeekbarUpdateUi(object? sender)
        {
            // if (DateTime.Now.Subtract(_lastSelfUpdateTimestamp).TotalSeconds < 1) return;
            //
            // Dispatcher.Invoke(() =>
            // {
            //     if (!SeekbarEnabled) return;
            //     if (!Visible || _isDragging) return;
            //     if (MediaManager.GetFocusedSession() is not { } session) return;
            //
            //     var timeline = session.ControlSession.GetTimelineProperties();
            //     var pos = timeline.Position + (DateTime.Now - timeline.LastUpdatedTime.DateTime);
            //     if (pos > timeline.EndTime)
            //     {
            //         HandlePlayBackState(GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed);
            //         return;
            //     }
            //
            //     UpdateSeekbarCurrentDuration(pos);
            // });
        }

        private void PauseOtherMediaSessionsIfNeeded(MediaSession mediaSession)
        {
            if (
                SettingsManager.Current.PauseOtherSessionsEnabled
                && mediaSession.ControlSession.GetPlaybackInfo().PlaybackStatus ==
                GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing
            )
            {
                PauseOtherSessions(mediaSession);
            }
        }

        private Task PauseOtherSessions(MediaSession currentMediaSession)
        {
            return Task.WhenAll(
                MediaManager.CurrentMediaSessions.Values.Select(session =>
                {
                    if (
                        session.Id != currentMediaSession.Id &&
                        session.ControlSession.GetPlaybackInfo().PlaybackStatus ==
                        GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing
                    )
                    {
                        return session.ControlSession.TryPauseAsync().AsTask();
                    }

                    return Task.CompletedTask;
                })
            );
        }

        private void InnerOnPlaybackStateChanged(MediaSession mediaSession,
            GlobalSystemMediaTransportControlsSessionPlaybackInfo? playbackInfo = null)
        {
            PauseOtherMediaSessionsIfNeeded(mediaSession);

            var focusedSession = MediaManager.GetFocusedSession();

            if (IsVisible)
            {
                HandlePlayBackState(focusedSession.ControlSession.GetPlaybackInfo().PlaybackStatus);
            }
        }

        private async void InnerOnAnyMediaPropertyChanged(MediaSession mediaSession,
            GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties)
        {
            if (MediaManager.GetFocusedSession() == null) return;

            PauseOtherMediaSessionsIfNeeded(mediaSession);

            if (!IsVisible) return;
            var focusedSession = MediaManager.GetFocusedSession();
            HandlePlayBackState(focusedSession.ControlSession.GetPlaybackInfo().PlaybackStatus);
            UpdateSongInfo(await mediaSession.ControlSession.TryGetMediaPropertiesAsync());
        }

        public async void UpdateUi()
        {
            if (MediaManager.GetFocusedSession() is { } session)
            {
                UpdateSongInfo(await session.ControlSession.TryGetMediaPropertiesAsync());
            }
        }

        public void UpdateSongInfo(GlobalSystemMediaTransportControlsSessionMediaProperties songInfo)
        {
            Dispatcher.Invoke(() =>
            {
                SongTitle = songInfo.Title;
                SongArtist = songInfo.Artist;
                SongImageSource = Helper.GetThumbnail(songInfo.Thumbnail);
                if (!SeekbarEnabled) return;
                var timeline = MediaManager.GetFocusedSession().ControlSession.GetTimelineProperties();
                if (timeline.MaxSeekTime.TotalSeconds >= 1.0)
                    SeekbarMaxValue = timeline.MaxSeekTime.TotalSeconds;
            });
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_Seekbar") is Slider slider)
            {
                slider.PreviewMouseLeftButtonUp += Seekbar_OnPreviewMouseLeftButtonUp;
                slider.PreviewMouseLeftButtonDown += Seekbar_OnPreviewMouseLeftButtonDown;
            }
        }
        private void Seekbar_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging) return;
            _isDragging = true;
        }

        private async void Seekbar_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MediaManager.GetFocusedSession() is { } session)
            {
                var seekPosition = TimeSpan.FromSeconds(SeekbarValue);
                if (seekPosition == TimeSpan.Zero) seekPosition = TimeSpan.FromSeconds(1);
                await session.ControlSession.TryChangePlaybackPositionAsync(seekPosition.Ticks);
            }

            _isDragging = false;
        }


        private void UpdateSeekbarCurrentDuration(TimeSpan pos)
        {
            if (_isDragging) return;
            SeekbarValue = pos.TotalSeconds;
        }

        private void HandlePlayBackState(GlobalSystemMediaTransportControlsSessionPlaybackStatus? status)
        {
        }


        private async void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MediaManager.GetFocusedSession() == null)
                return;

            await MediaManager.GetFocusedSession().ControlSession.TrySkipPreviousAsync();
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            // keybd_event(0xB3, 0, 0, IntPtr.Zero);

            if (MediaManager.GetFocusedSession() == null)
                return;

            // if (mediaManager.GetFocusedSession().ControlSession.GetPlaybackInfo().Controls.IsPauseEnabled)
            //     SymbolPlayPause.Dispatcher.Invoke(() => SymbolPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Pause16);
            // else
            //     SymbolPlayPause.Dispatcher.Invoke(() => SymbolPlayPause.Symbol = Wpf.Ui.Controls.SymbolRegular.Play16);
        }

        private async void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (MediaManager.GetFocusedSession() == null)
                return;

            await MediaManager.GetFocusedSession().ControlSession.TrySkipNextAsync();
        }

        public static readonly DependencyProperty ShuffleCommandProperty =
            DependencyProperty.Register(nameof(ShuffleCommand), typeof(ICommand), typeof(FluentMediaFlyout));

        public ICommand ShuffleCommand
        {
            get => (ICommand)GetValue(ShuffleCommandProperty);
            set => SetValue(ShuffleCommandProperty, value);
        }

        public static readonly DependencyProperty RepeatCommandProperty =
            DependencyProperty.Register(nameof(RepeatCommand), typeof(ICommand), typeof(FluentMediaFlyout));

        public ICommand RepeatCommand
        {
            get => (ICommand)GetValue(RepeatCommandProperty);
            set => SetValue(RepeatCommandProperty, value);
        }

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(FluentMediaFlyout));

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }


        public static readonly DependencyProperty BackCommandProperty =
            DependencyProperty.Register(nameof(BackCommand), typeof(ICommand), typeof(FluentMediaFlyout));

        public ICommand BackCommand
        {
            get => (ICommand)GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }
        public static readonly DependencyProperty ForwardCommandProperty =
            DependencyProperty.Register(nameof(ForwardCommand), typeof(ICommand), typeof(FluentMediaFlyout));

        public ICommand ForwardCommand
        {
            get => (ICommand)GetValue(ForwardCommandProperty);
            set => SetValue(ForwardCommandProperty, value);
        }


        public static readonly DependencyProperty PlayPauseCommandProperty =
            DependencyProperty.Register(nameof(PlayPauseCommand), typeof(ICommand), typeof(FluentMediaFlyout));

        public ICommand PlayPauseCommand
        {
            get => (ICommand)GetValue(PlayPauseCommandProperty);
            set => SetValue(PlayPauseCommandProperty, value);
        }

        public bool Visible { get; protected set; }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
            UpdateUi();
        }
    }
}