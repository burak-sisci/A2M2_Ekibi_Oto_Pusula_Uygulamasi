import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import '../../../lib/data/dto/user_login_dto.dart';
import '../../../lib/viewmodels/auth/login_view_model.dart';
import '../../../lib/viewmodels/base_view_model.dart';
import '../../helpers/mocks.dart';

void main() {
  late MockAuthRepository mockRepo;
  late LoginViewModel vm;

  setUp(() {
    mockRepo = MockAuthRepository();
    vm = LoginViewModel(authRepository: mockRepo);
  });

  tearDown(() => vm.dispose());

  test('başarılı giriş → state=success, loginSuccess=true', () async {
    when(() => mockRepo.login(any())).thenAnswer(
      (_) async => {'token': 'tok123', '_id': 'user123'},
    );
    await vm.login('test@example.com', 'Sifre123!');
    expect(vm.state, ViewState.success);
    expect(vm.loginSuccess, isTrue);
    expect(vm.token, 'tok123');
    expect(vm.userId, 'user123');
  });

  test('hata durumu → state=error', () async {
    when(() => mockRepo.login(any())).thenThrow(Exception('401'));
    await vm.login('yanlis@example.com', 'Yanlis1!');
    expect(vm.state, ViewState.error);
    expect(vm.loginSuccess, isFalse);
    expect(vm.errorMessage, isNotNull);
  });

  test('giriş öncesi state=idle', () {
    expect(vm.state, ViewState.idle);
  });
}
