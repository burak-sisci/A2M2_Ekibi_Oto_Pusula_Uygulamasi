import 'package:dio/dio.dart';
import 'exceptions.dart';

class ErrorInterceptor extends Interceptor {
  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    final mapped = _mapError(err);
    // Dio interceptor zincirini kırmamak için handler.reject() kullanılmalı.
    // throw kullanmak DioException [unknown] hatasına neden olur.
    handler.reject(
      DioException(
        requestOptions: err.requestOptions,
        response: err.response,
        type: err.type,
        error: mapped,
        message: mapped.message,
      ),
    );
  }

  ApiException _mapError(DioException err) {
    if (err.type == DioExceptionType.connectionError ||
        err.type == DioExceptionType.receiveTimeout ||
        err.type == DioExceptionType.sendTimeout ||
        err.type == DioExceptionType.connectionTimeout) {
      return const NetworkException();
    }

    final statusCode = err.response?.statusCode;
    final backendMsg = _extractMessage(err.response?.data);

    switch (statusCode) {
      case 400:
        return BadRequestException(backendMsg ?? 'Geçersiz istek.');
      case 401:
        return UnauthorizedException(backendMsg ?? 'Yetkisiz erişim.');
      case 403:
        return ForbiddenException(backendMsg ?? 'Bu işlem için yetkiniz yok.');
      case 404:
        return NotFoundException(backendMsg ?? 'İçerik bulunamadı.');
      case 409:
        return ConflictException(backendMsg ?? 'Bu kayıt zaten mevcut.');
      default:
        if (statusCode != null && statusCode >= 500) {
          return const ServerException();
        }
        return const NetworkException();
    }
  }

  String? _extractMessage(dynamic data) {
    if (data is Map<String, dynamic>) {
      final msg = data['message'] ?? data['mesaj'] ?? data['error'];
      if (msg is String && msg.isNotEmpty) return msg;
    }
    return null;
  }
}
