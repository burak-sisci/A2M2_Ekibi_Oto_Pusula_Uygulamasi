class Comment {
  final String id;
  final String userId;
  final String carId;
  final String content;
  final int likeCount;
  final List<String> likedByUsers;
  final DateTime createdAt;
  final DateTime? updatedAt;

  const Comment({
    required this.id,
    required this.userId,
    required this.carId,
    required this.content,
    this.likeCount = 0,
    this.likedByUsers = const [],
    required this.createdAt,
    this.updatedAt,
  });

  // Backend: {id, userId, carId, content, likeCount, likedByUsers, createdAt, updatedAt}
  factory Comment.fromJson(Map<String, dynamic> json) => Comment(
        id: json['id'] as String,
        userId: json['userId'] as String? ?? '',
        carId: json['carId'] as String? ?? '',
        content: (json['content'] ?? json['text']) as String? ?? '',
        likeCount: json['likeCount'] as int? ?? 0,
        likedByUsers:
            (json['likedByUsers'] as List<dynamic>? ?? []).cast<String>(),
        createdAt: DateTime.parse(json['createdAt'] as String),
        updatedAt: json['updatedAt'] != null
            ? DateTime.parse(json['updatedAt'] as String)
            : null,
      );

  Map<String, dynamic> toJson() => {
        'id': id,
        'userId': userId,
        'carId': carId,
        'content': content,
        'likeCount': likeCount,
        'likedByUsers': likedByUsers,
        'createdAt': createdAt.toIso8601String(),
        if (updatedAt != null) 'updatedAt': updatedAt!.toIso8601String(),
      };

  Comment copyWith({
    String? id,
    String? userId,
    String? carId,
    String? content,
    int? likeCount,
    List<String>? likedByUsers,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) =>
      Comment(
        id: id ?? this.id,
        userId: userId ?? this.userId,
        carId: carId ?? this.carId,
        content: content ?? this.content,
        likeCount: likeCount ?? this.likeCount,
        likedByUsers: likedByUsers ?? this.likedByUsers,
        createdAt: createdAt ?? this.createdAt,
        updatedAt: updatedAt ?? this.updatedAt,
      );
}
