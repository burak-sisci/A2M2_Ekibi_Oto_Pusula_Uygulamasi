import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import 'core/constants/app_constants.dart';
import 'core/network/api_client.dart';
import 'core/push/fcm_service.dart';
import 'core/router/app_router.dart';
import 'core/storage/token_storage.dart';
import 'core/theme/app_theme.dart';

import 'data/repositories/ai_repository.dart';
import 'data/repositories/auth_repository.dart';
import 'data/repositories/car_repository.dart';
import 'data/repositories/comment_repository.dart';
import 'data/repositories/device_repository.dart';
import 'data/repositories/list_repository.dart';
import 'data/repositories/user_repository.dart';

import 'viewmodels/auth/auth_session_view_model.dart';

import 'view/pages/auth/forgot_password_page.dart';
import 'view/pages/auth/login_page.dart';
import 'view/pages/auth/register_page.dart';
import 'view/pages/auth/reset_password_page.dart';
import 'view/pages/car/car_create_page.dart';
import 'view/pages/car/car_detail_page.dart';
import 'view/pages/car/car_list_page.dart';
import 'view/pages/car/price_predict_page.dart';
import 'view/pages/comment/comments_page.dart';
import 'view/pages/home/home_page.dart';
import 'view/pages/list/list_detail_page.dart';
import 'view/pages/list/lists_page.dart';
import 'view/pages/profile/edit_profile_page.dart';
import 'view/pages/profile/profile_page.dart';
import 'view/pages/splash_page.dart';

class App extends StatefulWidget {
  const App({super.key});

  @override
  State<App> createState() => _AppState();
}

class _AppState extends State<App> {
  late final TokenStorage _tokenStorage;
  late final ApiClient _apiClient;
  late final AuthRepository _authRepository;
  late final UserRepository _userRepository;
  late final CarRepository _carRepository;
  late final AiRepository _aiRepository;
  late final ListRepository _listRepository;
  late final CommentRepository _commentRepository;
  late final DeviceRepository _deviceRepository;
  late final FcmService _fcmService;
  late final AuthSessionViewModel _authSession;
  late final GoRouter _router;

  @override
  void initState() {
    super.initState();
    _tokenStorage = TokenStorage();
    _apiClient = ApiClient(tokenStorage: _tokenStorage);
    _authRepository = AuthRepository(apiClient: _apiClient);
    _userRepository = UserRepository(apiClient: _apiClient);
    _carRepository = CarRepository(apiClient: _apiClient);
    _aiRepository = AiRepository(apiClient: _apiClient);
    _listRepository = ListRepository(apiClient: _apiClient);
    _commentRepository = CommentRepository(apiClient: _apiClient);
    _deviceRepository = DeviceRepository(apiClient: _apiClient);
    _fcmService = FcmService(deviceRepository: _deviceRepository);
    _authSession = AuthSessionViewModel(
      authRepository: _authRepository,
      tokenStorage: _tokenStorage,
      fcmService: _fcmService,
    );
    // Interceptor refresh başarısız olunca session'ı temizlesin
    _apiClient.authInterceptor.onSessionExpired = _authSession.handleSessionExpired;
    _router = _buildRouter();
    _authSession.init();
  }

  GoRouter _buildRouter() {
    return GoRouter(
      initialLocation: AppRoutes.splash,
      refreshListenable: _authSession,
      redirect: (context, state) {
        final status = _authSession.status;
        final path = state.matchedLocation;

        if (status == AuthStatus.initializing) return AppRoutes.splash;

        if (status == AuthStatus.unauthenticated) {
          if (path == AppRoutes.login ||
              path == AppRoutes.register ||
              path == AppRoutes.forgotPassword ||
              path == AppRoutes.resetPassword) {
            return null;
          }
          return AppRoutes.login;
        }

        if (status == AuthStatus.authenticated) {
          if (path == AppRoutes.splash ||
              path == AppRoutes.login ||
              path == AppRoutes.register) {
            return AppRoutes.home;
          }
        }
        return null;
      },
      routes: [
        GoRoute(
          path: AppRoutes.splash,
          builder: (_, __) => const SplashPage(),
        ),
        GoRoute(
          path: AppRoutes.login,
          builder: (_, __) => const LoginPage(),
        ),
        GoRoute(
          path: AppRoutes.register,
          builder: (_, __) => const RegisterPage(),
        ),
        GoRoute(
          path: AppRoutes.home,
          builder: (_, __) => const HomePage(),
          routes: [
            GoRoute(
              path: AppConstants.carsSegment,
              builder: (_, __) => const CarListPage(),
            ),
          ],
        ),
        GoRoute(
          path: AppRoutes.carCreate,
          builder: (_, __) => const CarCreatePage(),
        ),
        // Spesifik rotalar (:id parametreli rotadan önce tanımlanmalı
        GoRoute(
          path: AppRoutes.pricePredict,
          builder: (_, __) => const PricePredictPage(),
        ),
        GoRoute(
          path: AppRoutes.carDetail,
          builder: (_, state) => CarDetailPage(carId: state.pathParameters['id']!),
        ),
        GoRoute(
          path: AppRoutes.carComments,
          builder: (_, state) => CommentsPage(carId: state.pathParameters['id']!),
        ),
        GoRoute(
          path: AppRoutes.lists,
          builder: (_, __) => const ListsPage(),
        ),
        GoRoute(
          path: AppRoutes.listDetail,
          builder: (_, state) => ListDetailPage(listId: state.pathParameters['id']!),
        ),
        GoRoute(
          path: AppRoutes.profile,
          builder: (_, __) => const ProfilePage(),
        ),
        GoRoute(
          path: AppRoutes.editProfile,
          builder: (_, __) => const EditProfilePage(),
        ),
        GoRoute(
          path: AppRoutes.forgotPassword,
          builder: (_, __) => const ForgotPasswordPage(),
        ),
        GoRoute(
          path: AppRoutes.resetPassword,
          builder: (_, state) => ResetPasswordPage(
            initialToken: state.uri.queryParameters['token'],
          ),
        ),
      ],
    );
  }

  @override
  void dispose() {
    _router.dispose();
    _authSession.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        Provider<AuthRepository>.value(value: _authRepository),
        Provider<UserRepository>.value(value: _userRepository),
        Provider<CarRepository>.value(value: _carRepository),
        Provider<AiRepository>.value(value: _aiRepository),
        Provider<ListRepository>.value(value: _listRepository),
        Provider<CommentRepository>.value(value: _commentRepository),
        ChangeNotifierProvider<AuthSessionViewModel>.value(value: _authSession),
      ],
      child: MaterialApp.router(
        title: 'OtoPusula',
        debugShowCheckedModeBanner: false,
        theme: AppTheme.light,
        darkTheme: AppTheme.dark,
        themeMode: ThemeMode.system,
        routerConfig: _router,
      ),
    );
  }
}
