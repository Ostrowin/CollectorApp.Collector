using Android.Content;
using CollectorApp.Helpers;
using CollectorApp.Models;
using CommunityToolkit.Mvvm.Messaging;

namespace CollectorApp.Platforms.Android;

public class DataWedgeBroadcastReceiver : BroadcastReceiver
{
    private readonly IMessenger _messenger;

    public DataWedgeBroadcastReceiver(IMessenger messenger)
    {
        _messenger = messenger;
    }
    public override void OnReceive(Context? context, Intent? intent)
    {
        AppLogger.Info($"OnReceive fired! Action: {intent?.Action}");

        if (intent is null)
        {
            AppLogger.Warning("OnReceive - intent is null");
            return;
        }

        var value = intent.GetStringExtra("com.symbol.datawedge.data_string");
        var format = intent.GetStringExtra("com.symbol.datawedge.label_type");

        AppLogger.Info($"Barcode value: {value} | Format: {format}");

        if (string.IsNullOrEmpty(value))
        {
            AppLogger.Warning("OnReceive - value is empty");
            return;
        }

        var cleanFormat = format?.Replace("LABEL-TYPE-", "") ?? "UNKNOWN";

        MainThread.BeginInvokeOnMainThread(() =>
        {
            _messenger.Send(new BarcodeScannedMessage(value, cleanFormat));
        });
    }
}
