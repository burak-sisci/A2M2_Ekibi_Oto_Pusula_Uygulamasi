import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/constants/app_strings.dart';
import '../../../data/models/list_model.dart';
import '../../../data/repositories/list_repository.dart';
import '../../../data/repositories/user_repository.dart';
import '../../../viewmodels/auth/auth_session_view_model.dart';
import '../common/loading_indicator.dart';

class AddToListSheet extends StatefulWidget {
  final String carId;

  const AddToListSheet({super.key, required this.carId});

  static Future<void> show(BuildContext context, {required String carId}) =>
      showModalBottomSheet(
        context: context,
        shape: const RoundedRectangleBorder(
          borderRadius: BorderRadius.vertical(
            top: Radius.circular(AppConstants.radiusSheet),
          ),
        ),
        builder: (_) => AddToListSheet(carId: carId),
      );

  @override
  State<AddToListSheet> createState() => _AddToListSheetState();
}

class _AddToListSheetState extends State<AddToListSheet> {
  List<UserList>? _lists;
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadLists();
  }

  Future<void> _loadLists() async {
    final userId = context.read<AuthSessionViewModel>().userId;
    if (userId == null) {
      setState(() {
        _loading = false;
        _error = AppStrings.errorGeneric;
      });
      return;
    }
    try {
      final lists = await context.read<UserRepository>().getUserLists(userId);
      setState(() {
        _lists = lists;
        _loading = false;
      });
    } on Exception {
      setState(() {
        _loading = false;
        _error = AppStrings.errorGeneric;
      });
    }
  }

  Future<void> _addToList(String listId) async {
    try {
      await context.read<ListRepository>().addCarToList(listId, widget.carId);
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('İlan listeye eklendi.')),
        );
        Navigator.of(context).pop();
      }
    } on Exception {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text(AppStrings.errorGeneric)),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(AppConstants.space16),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(AppStrings.addToList,
              style: Theme.of(context).textTheme.titleMedium),
          const SizedBox(height: AppConstants.space16),
          if (_loading) const LoadingIndicator(),
          if (_error != null) Text(_error!),
          if (_lists != null)
            ..._lists!.map(
              (l) => ListTile(
                leading: const Icon(Icons.list_alt_outlined),
                title: Text(l.name),
                trailing: const Icon(Icons.chevron_right),
                onTap: () => _addToList(l.id),
              ),
            ),
          const SizedBox(height: AppConstants.space8),
        ],
      ),
    );
  }
}
