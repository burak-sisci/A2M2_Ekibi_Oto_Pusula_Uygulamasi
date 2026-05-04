import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import '../../../lib/data/repositories/car_repository.dart';
import '../../../lib/viewmodels/base_view_model.dart';
import '../../../lib/viewmodels/car/car_list_view_model.dart';
import '../../helpers/mocks.dart';
import '../../helpers/test_data.dart';

void main() {
  late MockCarRepository mockRepo;
  late CarListViewModel vm;

  setUpAll(() {
    registerFallbackValue(const CarFilterParams());
  });

  setUp(() {
    mockRepo = MockCarRepository();
    vm = CarListViewModel(carRepository: mockRepo);
  });

  tearDown(() => vm.dispose());

  test('başarılı yükleme → state=success, cars dolu', () async {
    when(() => mockRepo.listCars(any()))
        .thenAnswer((_) async => [TestData.car]);
    await vm.load();
    expect(vm.state, ViewState.success);
    expect(vm.cars.length, 1);
    expect(vm.cars.first.brand, 'Toyota');
  });

  test('hata → state=error', () async {
    when(() => mockRepo.listCars(any()))
        .thenThrow(Exception('network'));
    await vm.load();
    expect(vm.state, ViewState.error);
    expect(vm.errorMessage, isNotNull);
  });

  test('yeniden yükleme listeyi sıfırlar', () async {
    when(() => mockRepo.listCars(any()))
        .thenAnswer((_) async => [TestData.car]);
    await vm.load();
    await vm.load();
    expect(vm.cars.length, 1);
  });
}
