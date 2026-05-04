class ShareLink {
  final String shortUrl;
  final String originalUrl;
  final DateTime expiresAt;

  const ShareLink({
    required this.shortUrl,
    required this.originalUrl,
    required this.expiresAt,
  });

  factory ShareLink.fromJson(Map<String, dynamic> json) => ShareLink(
        shortUrl: json['shortUrl'] as String,
        originalUrl: json['originalUrl'] as String,
        expiresAt: DateTime.parse(json['expiresAt'] as String),
      );

  Map<String, dynamic> toJson() => {
        'shortUrl': shortUrl,
        'originalUrl': originalUrl,
        'expiresAt': expiresAt.toIso8601String(),
      };

  ShareLink copyWith({
    String? shortUrl,
    String? originalUrl,
    DateTime? expiresAt,
  }) =>
      ShareLink(
        shortUrl: shortUrl ?? this.shortUrl,
        originalUrl: originalUrl ?? this.originalUrl,
        expiresAt: expiresAt ?? this.expiresAt,
      );
}
