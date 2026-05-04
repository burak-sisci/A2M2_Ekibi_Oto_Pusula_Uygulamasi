class ListUpdateDto {
  final String name;

  const ListUpdateDto({required this.name});

  Map<String, dynamic> toJson() => {'name': name};
}
