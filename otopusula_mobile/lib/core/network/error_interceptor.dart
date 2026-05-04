import 'package:dio/dio.dart';
import 'exceptions.dart';

class ErrorInterceptor extends Interceptor {
  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    throw _mapError(err);
  }

  ApiException _mapError(DioException err) {
    if (err.type == DioExceptionType.connectionError ||
        err.type == DioExceptionType.receiveTimeout ||
        err.type == DioExceptionType.sendTimeout ||
        err.type == DioExceptionType.connectionTimeout) {
      return const NetworkException();
    }

    final statusCode = err.response?.statusCode;
    switch (statusCode) {
      case 400:
        return const BadRequestException();
      case 401:
        return const UnauthorizedException();
      case 403:
        return const ForbiddenException();
      case 404:
        return const NotFoundException();
      case 409:
        return const ConflictException();
      default:
        if (statusCode != null && statusCode >= 500) {
          return const ServerException();
        }
        return const NetworkException();
    }
  }
}
