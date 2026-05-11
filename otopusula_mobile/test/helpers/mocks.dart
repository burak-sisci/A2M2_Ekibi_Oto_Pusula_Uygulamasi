import 'package:mocktail/mocktail.dart';
import 'package:otopusula_mobile/data/repositories/ai_repository.dart';
import 'package:otopusula_mobile/data/repositories/auth_repository.dart';
import 'package:otopusula_mobile/data/repositories/car_repository.dart';
import 'package:otopusula_mobile/data/repositories/comment_repository.dart';
import 'package:otopusula_mobile/data/repositories/list_repository.dart';
import 'package:otopusula_mobile/data/repositories/user_repository.dart';
import 'package:otopusula_mobile/core/storage/token_storage.dart';

class MockAuthRepository extends Mock implements AuthRepository {}
class MockUserRepository extends Mock implements UserRepository {}
class MockCarRepository extends Mock implements CarRepository {}
class MockAiRepository extends Mock implements AiRepository {}
class MockListRepository extends Mock implements ListRepository {}
class MockCommentRepository extends Mock implements CommentRepository {}
class MockTokenStorage extends Mock implements TokenStorage {}
