class ApiEndpoints {
  ApiEndpoints._();

  // Auth
  static const authRegister = '/auth/register';
  static const authLogin = '/auth/login';
  static const authLogout = '/auth/logout';
  static const authForgotPassword = '/auth/forgot-password';
  static const authResetPassword = '/auth/reset-password';

  // Users
  static String user(String userId) => '/users/$userId';
  static String userLists(String userId) => '/users/$userId/lists';
  static String userComments(String userId) => '/users/$userId/comments';

  // Cars
  static const cars = '/cars';
  static const predictPrice = '/api/Prediction/predict';
  static String car(String carId) => '/cars/$carId';
  static String carShare(String carId) => '/cars/$carId/share';
  static String carComments(String carId) => '/cars/$carId/comments';

  // Comments
  static String comment(String commentId) => '/comments/$commentId';
  static String commentLike(String commentId) => '/comments/$commentId/like';

  // Lists
  static const lists = '/lists';
  static String list(String listId) => '/lists/$listId';
  static String listCars(String listId) => '/lists/$listId/cars';
  static String listCar(String listId, String carId) => '/lists/$listId/cars/$carId';
}
