using System.Windows;
using System.Windows.Controls;
using FlyleafLib;
using FlyleafLib.MediaPlayer;

namespace FlyleafCycle;

public partial class FlyleafPlayer : UserControl
{
    public static readonly DependencyProperty IdProperty = DependencyProperty.Register(
        nameof(Id), typeof(string), typeof(FlyleafPlayer));

    public string Id
    {
        get { return (string)GetValue(IdProperty); }
        set { SetValue(IdProperty, value); }
    }

    private CancellationTokenSource? _cancel;
    
    public FlyleafPlayer()
    {
        InitializeComponent();
    }

    public void StartCycling() 
    {
        _cancel?.Cancel();
        _cancel?.Dispose();
        _cancel = new CancellationTokenSource();

        var cancel = _cancel.Token;
        Task.Run(async () =>
        {
            try
            {
                while (!cancel.IsCancellationRequested)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadPlayer($"file://{AppDomain.CurrentDomain.BaseDirectory}/Media/clip2.mp4");
                    });
                    await Task.Delay(20000, cancel);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadPlayer(string.Empty);
                    });
                    await Task.Delay(5000, cancel);
                }
            }
            catch
            {

            }
        }, cancel);
    }
    
    public void StopCycling()
    {
        _cancel?.Cancel();
        _cancel?.Dispose();
        _cancel = null;
        LoadPlayer(string.Empty);
    }

    private void LoadPlayer(string uri)
    {
        FlyleafPlayer1.Player?.Stop();
        FlyleafPlayer1.Player?.Dispose();

        if (!string.IsNullOrEmpty(uri))
        {
            Config config = new()
            {
                Player = new()
                {
                    AutoPlay = true,
                    Usage = Usage.AVS
                },
                Audio = new()
                {
                    Enabled = false,
                },
                Subtitles = new()
                {
                    Enabled = false
                }
            };

            FlyleafPlayer1.Player = new Player(config);
            FlyleafPlayer1.Player.Open(uri);
        }
    }
}