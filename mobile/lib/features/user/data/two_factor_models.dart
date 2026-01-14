class TotpSetupData {
  final String otpauthUri;
  final String manualKeyBase32;

  TotpSetupData({
    required this.otpauthUri,
    required this.manualKeyBase32,
  });

  factory TotpSetupData.fromJson(Map<String, dynamic> json) {
    return TotpSetupData(
      otpauthUri: json['otpauthUri'] as String,
      manualKeyBase32: json['manualKeyBase32'] as String,
    );
  }
}