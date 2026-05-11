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
        // Backend returns camelCase Turkish: kisaUrl / orijinalUrl / gecerlilikSonu
        shortUrl: (json['kisaUrl'] ?? json['shortUrl'] ?? '') as String,
        originalUrl: (json['orijinalUrl'] ?? json['originalUrl'] ?? '') as String,
        expiresAt: json['gecerlilikSonu'] != null
            ? DateTime.parse(json['gecerlilikSonu'] as String)
            : json['expiresAt'] != null
                ? DateTime.parse(json['expiresAt'] as String)
                : DateTime.now().add(const Duration(days: 90)),
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
