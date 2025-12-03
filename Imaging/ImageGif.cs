/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     Imaging
* FILE:        ImageGifThreaded.cs
* PURPOSE:     Extends Image Control to play GIFs asynchronously on its own thread
* PROGRAMMER:  Peter Geinitz (Wayfarer)
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imaging;

/// <inheritdoc cref="Image" />
/// <summary>
/// Extension for Image to play GIFs asynchronously on a background thread
/// </summary>
public sealed class ImageGif : Image, IDisposable
{
    #region Dependency Properties

    public static readonly DependencyProperty FrameIndexProperty =
        DependencyProperty.Register(nameof(FrameIndex), typeof(int), typeof(ImageGif),
            new UIPropertyMetadata(0, ChangingFrameIndex));

    public static readonly DependencyProperty AutoStartProperty =
        DependencyProperty.Register(nameof(AutoStart), typeof(bool), typeof(ImageGif),
            new UIPropertyMetadata(false, AutoStartPropertyChanged));

    public static readonly DependencyProperty GifSourceProperty =
        DependencyProperty.Register(nameof(GifSource), typeof(string), typeof(ImageGif),
            new UIPropertyMetadata(string.Empty, GifSourcePropertyChanged));

    #endregion

    #region Private Fields

    private List<ImageSource> _imageList;
    private List<int> _frameDelays; // in milliseconds
    private bool _isDisposed;
    private bool _isInitialized;

    private CancellationTokenSource _animationCts;
    private Task _animationTask;

    #endregion

    #region Constructor

    static ImageGif()
    {
        VisibilityProperty.OverrideMetadata(typeof(ImageGif),
            new FrameworkPropertyMetadata(VisibilityPropertyChanged));
    }

    #endregion

    #region Public Properties

    public int FrameIndex
    {
        get => (int)GetValue(FrameIndexProperty);
        set => SetValue(FrameIndexProperty, value);
    }

    public bool AutoStart
    {
        get => (bool)GetValue(AutoStartProperty);
        set => SetValue(AutoStartProperty, value);
    }

    public string GifSource
    {
        get => (string)GetValue(GifSourceProperty);
        set => SetValue(GifSourceProperty, value);
    }

    #endregion

    #region Events

    public event EventHandler ImageLoaded;

    #endregion

    #region Private Methods

    private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if ((Visibility)e.NewValue == Visibility.Visible)
        {
            ((ImageGif)sender).StartAnimation();
        }
        else
        {
            ((ImageGif)sender).StopAnimation();
        }
    }

    private static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
    {
        if (obj is not ImageGif gifImage)
            return;

        int newIndex = (int)ev.NewValue;
        if (gifImage._imageList != null && newIndex >= 0 && newIndex < gifImage._imageList.Count)
        {
            gifImage.Source = gifImage._imageList[newIndex];
        }
    }

    private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        (sender as ImageGif)?.StartAnimation();
    }

    private static async void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var gif = sender as ImageGif;
        if (gif == null) return;

        await gif.InitializeAsync(); // load frames async

        if (gif.AutoStart)
            gif.StartAnimation();
    }

    /// <summary>
    /// Loads GIF frames asynchronously off the UI thread
    /// </summary>
    /// <summary>
    /// Loads GIF frames asynchronously off the UI thread
    /// </summary>
    private async Task InitializeAsync()
    {
        string gifSource = await Application.Current.Dispatcher.InvokeAsync(() => GifSource);

        if (!File.Exists(gifSource))
            return;

        try
        {
            // Load frames in background thread
            var (frames, delays) = await Task.Run(() => ImageGifHandler.LoadGifWithDelaysAsync(gifSource));

            // Freeze frames to make them thread-safe
            _imageList = new List<ImageSource>();
            foreach (var frame in frames)
            {
                if (frame.CanFreeze)
                    frame.Freeze(); // important!
                _imageList.Add(frame);
            }

            _frameDelays = delays;

            if (_imageList.Count == 0)
                return;

            _isInitialized = true;

            // Set the first frame immediately on the UI thread
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                Source = _imageList[0];
                ImageLoaded?.Invoke(this, EventArgs.Empty);
            }));
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);
        }
    }

    #endregion

    #region Public Methods

    public void StartAnimation()
    {
        if (!_isInitialized || _imageList == null || _imageList.Count == 0)
            return;

        StopAnimation(); // stop existing animation

        _animationCts = new CancellationTokenSource();
        var token = _animationCts.Token;

        _animationTask = Task.Run(async () =>
        {
            int frameIndex = 0;
            while (!token.IsCancellationRequested)
            {
                int delay = _frameDelays[frameIndex];

                // Update UI on the dispatcher thread
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    Source = _imageList[frameIndex];
                    FrameIndex = frameIndex;
                }));

                frameIndex = (frameIndex + 1) % _imageList.Count;

                await Task.Delay(delay, token);
            }
        }, token);
    }

    public void StopAnimation()
    {
        if (_animationCts != null)
        {
            _animationCts.Cancel();
            _animationCts = null;
        }
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
        {
            StopAnimation();
            _imageList?.Clear();
        }

        _isDisposed = true;
    }

    ~ImageGif()
    {
        Dispose(false);
    }

    #endregion
}
