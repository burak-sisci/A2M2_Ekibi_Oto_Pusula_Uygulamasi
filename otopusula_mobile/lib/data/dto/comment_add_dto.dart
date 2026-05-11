class CommentAddDto {
  final String content;

  const CommentAddDto({required this.content});

  Map<String, dynamic> toJson() => {'icerik': content};
}
