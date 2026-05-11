class CommentUpdateDto {
  final String content;

  const CommentUpdateDto({required this.content});

  Map<String, dynamic> toJson() => {'icerik': content};
}
