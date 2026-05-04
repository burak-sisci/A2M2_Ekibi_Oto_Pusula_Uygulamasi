import '../../core/network/api_client.dart';
import '../../core/constants/api_endpoints.dart';
import '../dto/list_create_dto.dart';
import '../dto/list_update_dto.dart';
import '../models/list_model.dart';

class ListRepository {
  final ApiClient _apiClient;

  ListRepository({required ApiClient apiClient}) : _apiClient = apiClient;

  Future<UserList> createList(ListCreateDto dto) async {
    final response = await _apiClient.dio.post(
      ApiEndpoints.lists,
      data: dto.toJson(),
    );
    return UserList.fromJson(response.data as Map<String, dynamic>);
  }

  Future<UserList> getList(String listId) async {
    final response = await _apiClient.dio.get(ApiEndpoints.list(listId));
    return UserList.fromJson(response.data as Map<String, dynamic>);
  }

  Future<UserList> updateList(String listId, ListUpdateDto dto) async {
    final response = await _apiClient.dio.put(
      ApiEndpoints.list(listId),
      data: dto.toJson(),
    );
    return UserList.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> deleteList(String listId) async {
    await _apiClient.dio.delete(ApiEndpoints.list(listId));
  }

  Future<void> addCarToList(String listId, String carId) async {
    await _apiClient.dio.post(
      ApiEndpoints.listCars(listId),
      data: {'carId': carId},
    );
  }

  Future<void> removeCarFromList(String listId, String carId) async {
    await _apiClient.dio.delete(ApiEndpoints.listCar(listId, carId));
  }
}
