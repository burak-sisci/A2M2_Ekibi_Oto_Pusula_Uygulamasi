class Comment {
  final String id;
  final String userId;
  final String carId;
  final String content;
  final DateTime createdAt;

  const Comment({
    required this.id,
    required this.userId,
    required this.carId,
    required this.content,
    required this.createdAt,
  });

  factory Comment.fromJson(Map<String, dynamic> json) => Comment(
        id: json['_id'] as String,
        userId: json['userId'] as String,
        carId: json['carId'] as String,
        // Backend'de alan adı 'content'; eski Postman örneğindeki 'text' hatalıydı
        content: (json['content'] ?? json['text']) as String? ?? '',
        createdAt: DateTime.parse(json['createdAt'] as String),
      );

  Map<String, dynamic> toJson() => {
        '_id': id,
        'userId': userId,
        'carId': carId,
        'content': content,
        'createdAt': createdAt.toIso8601String(),
      };

  Comment copyWith({
    String? id,
    String? userId,
    String? carId,
    String? content,
    DateTime? createdAt,
  }) =>
      Comment(
        id: id ?? this.id,
        userId: userId ?? this.userId,
        carId: carId ?? this.carId,
        content: content ?? this.content,
        createdAt: createdAt ?? this.createdAt,
      );
}
