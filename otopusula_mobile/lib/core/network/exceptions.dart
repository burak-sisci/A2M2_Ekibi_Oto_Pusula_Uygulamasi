sealed class ApiException implements Exception {
  final String message;
  const ApiException(this.message);

  @override
  String toString() => message;
}

final class BadRequestException extends ApiException {
  const BadRequestException([super.message = 'Geçersiz istek.']);
}

final class UnauthorizedException extends ApiException {
  const UnauthorizedException([super.message = 'Yetkisiz erişim.']);
}

final class ForbiddenException extends ApiException {
  const ForbiddenException([super.message = 'Bu işlem için yetkiniz yok.']);
}

final class NotFoundException extends ApiException {
  const NotFoundException([super.message = 'İçerik bulunamadı.']);
}

final class ConflictException extends ApiException {
  const ConflictException([super.message = 'Bu kayıt zaten mevcut.']);
}

final class ServerException extends ApiException {
  const ServerException([super.message = 'Sunucu hatası.']);
}

final class NetworkException extends ApiException {
  const NetworkException([super.message = 'İnternet bağlantısı yok.']);
}
