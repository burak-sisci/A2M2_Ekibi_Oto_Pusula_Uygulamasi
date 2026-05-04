// Rota yolları sabitleri. GoRouter instance'ı app.dart'ta kurulur;
// böylece core/ katmanı view/pages/'e bağımlı kalmaz.
class AppRoutes {
  AppRoutes._();

  static const splash = '/splash';
  static const login = '/login';
  static const register = '/register';
  static const home = '/home';

  // İlanlar
  static const carDetail = '/cars/:id';
  static const carCreate = '/cars/new';
  static const pricePredict = '/cars/predict';
  static const carComments = '/cars/:id/comments';

  static String carDetailPath(String id) => '/cars/$id';
  static String carCommentsPath(String id) => '/cars/$id/comments';

  // Listeler
  static const lists = '/lists';
  static const listDetail = '/lists/:id';

  static String listDetailPath(String id) => '/lists/$id';

  // Profil
  static const profile = '/profile';
  static const editProfile = '/profile/edit';
}
