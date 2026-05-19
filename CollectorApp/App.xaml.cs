using CommunityToolkit.Mvvm.Messaging;
using CollectorApp.Helpers;
using CollectorApp.Services.Interfaces;
using CollectorApp.Models;




#if ANDROID
using Android.Content;
using CollectorApp.Platforms.Android;
#endif

namespace CollectorApp;

public partial class App : Application
{
#if ANDROID
    private DataWedgeBroadcastReceiver? _receiver;
#endif
    private readonly IMessenger _messenger;
    private readonly IScannerService _scannerService;

    public App(IMessenger messenger, AppShell appShell, IScannerService scannerService)
    {
        InitializeComponent();
        _messenger = messenger;
        MainPage = appShell;
        scannerService.Initialize();

        _messenger.Register<SessionExpiredMessage>(this, async (r, m) =>
        {
            AppLogger.Info("Session expired message received, navigating to login page");
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.GoToAsync("//LoginPage");
            });
        });
    }

    protected override void OnStart()
    {
        AppLogger.Info("App.OnStart");
        base.OnStart();
    #if ANDROID
        RegisterDataWedgeReceiver();
    #endif
    }

    protected override void OnSleep()
    {
        AppLogger.Info("App.OnSleep");
        base.OnSleep();
    #if ANDROID
        UnregisterDataWedgeReceiver();
    #endif
    }

    protected override void OnResume()
    {
        AppLogger.Info("App.OnResume");
        base.OnResume();
    #if ANDROID
        RegisterDataWedgeReceiver();
    #endif
    }

#if ANDROID
    private void RegisterDataWedgeReceiver()
    {
        AppLogger.Info("DataWedge receiver registered");
        if (_receiver is not null)
            return;

        _receiver = new DataWedgeBroadcastReceiver(_messenger);

        var filter = new IntentFilter();
        filter.AddAction("com.insertsf.collector.SCAN");
        filter.AddAction("com.symbol.datawedge.api.RESULT_ACTION");
        filter.AddCategory(Intent.CategoryDefault);

        Android.App.Application.Context.RegisterReceiver(
            _receiver,
            filter,
            ReceiverFlags.Exported);
    }

    private void UnregisterDataWedgeReceiver()
    {
        AppLogger.Info("DataWedge receiver unregistered");
        if (_receiver is null)
            return;

        Android.App.Application.Context.UnregisterReceiver(_receiver);
        _receiver = null;
    }
#endif
}