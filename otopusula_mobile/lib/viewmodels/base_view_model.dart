import 'package:flutter/foundation.dart';

enum ViewState { idle, loading, success, error }

abstract class BaseViewModel extends ChangeNotifier {
  ViewState _state = ViewState.idle;
  String? _errorMessage;

  ViewState get state => _state;
  String? get errorMessage => _errorMessage;
  bool get isLoading => _state == ViewState.loading;
  bool get hasError => _state == ViewState.error;
  bool get isSuccess => _state == ViewState.success;

  @protected
  void setState(ViewState s, {String? error}) {
    _state = s;
    _errorMessage = error;
    notifyListeners();
  }

  @protected
  void setLoading() => setState(ViewState.loading);

  @protected
  void setSuccess() => setState(ViewState.success);

  @protected
  void setError(String message) => setState(ViewState.error, error: message);

  @protected
  void setIdle() => setState(ViewState.idle);
}
