namespace CollectorApp.Models;

public record ScanResult(
    string RawValue,
    string BarcodeFormat,
    DateTime ScannedAt
);
