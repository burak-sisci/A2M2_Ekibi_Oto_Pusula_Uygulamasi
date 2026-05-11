class ListCreateDto {
  final String name;

  const ListCreateDto({required this.name});

  Map<String, dynamic> toJson() => {'listeAdi': name};
}
