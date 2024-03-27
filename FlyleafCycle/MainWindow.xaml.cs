using System.Diagnostics;
using System.Windows;
using FlyleafLib;
using Vortice.DXGI.Debug;
using Vortice.DXGI;

namespace FlyleafCycle;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        if (!Engine.IsLoaded)
        {
            Engine.Start(new()
            {
                LogLevel = LogLevel.Trace,
                LogOutput = $"{AppDomain.CurrentDomain.BaseDirectory}\\flyleaf.log",
                FFmpegPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\FFmpeg"
            });
        }

        Task.Run(async () =>
        {
            while (true)
            {
#if DEBUG
                Debug.WriteLine($"========= Live objects ==============");
                try
                {
                    if (DXGI.DXGIGetDebugInterface1(out IDXGIDebug1 dxgiDebug).Success)
                    {
                        //dxgiDebug.ReportLiveObjects(DXGI.DebugAll, ReportLiveObjectFlags.Summary | ReportLiveObjectFlags.IgnoreInternal);
                        dxgiDebug.ReportLiveObjects(DXGI.DebugAll, ReportLiveObjectFlags.Summary);
                        dxgiDebug.Dispose();
                    }

                }
                catch { }
                Debug.WriteLine($"=====================================");
#endif
                // Force GC collection to observe a clear linear trend of handle leaks.
                GC.Collect();
                GC.WaitForPendingFinalizers();
                await Task.Delay(5000);
            }
        });
    }

    private void OnStopCycling(object sender, RoutedEventArgs e)
    {
        foreach (var cell in Players.Children)
        {
            if (cell is FlyleafPlayer flyleafPlayerCell)
            {
                flyleafPlayerCell.StopCycling();
            }
        }
    }

    private void OnStartCycling(object sender, RoutedEventArgs e)
    {
        foreach (var cell in Players.Children)
        {
            if (cell is FlyleafPlayer flyleafPlayerCell)
            {
                flyleafPlayerCell.StartCycling();
            }
        }
    }
}