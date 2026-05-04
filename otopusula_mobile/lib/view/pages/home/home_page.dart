import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../core/router/app_router.dart';
import '../car/car_list_page.dart';
import '../list/lists_page.dart';
import '../car/price_predict_page.dart';
import '../profile/profile_page.dart';

// Alt sekme kabuğu: İlanlar / Listelerim / Fiyat Tahmini / Profil (developer.md §8)
class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  int _currentIndex = 0;

  static const _tabs = [
    CarListPage(),
    ListsPage(),
    PricePredictPage(),
    ProfilePage(),
  ];

  static const _labels = ['İlanlar', 'Listelerim', 'Fiyat Tahmini', 'Profil'];

  static const _icons = [
    Icons.directions_car_outlined,
    Icons.bookmark_border,
    Icons.price_change_outlined,
    Icons.person_outline,
  ];

  static const _activeIcons = [
    Icons.directions_car,
    Icons.bookmark,
    Icons.price_change,
    Icons.person,
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: IndexedStack(index: _currentIndex, children: _tabs),
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _currentIndex,
        onTap: (i) => setState(() => _currentIndex = i),
        items: List.generate(
          _tabs.length,
          (i) => BottomNavigationBarItem(
            icon: Icon(_icons[i]),
            activeIcon: Icon(_activeIcons[i]),
            label: _labels[i],
          ),
        ),
      ),
      floatingActionButton: _currentIndex == 0
          ? FloatingActionButton(
              onPressed: () => context.push(AppRoutes.carCreate),
              tooltip: 'Yeni İlan',
              child: const Icon(Icons.add),
            )
          : null,
    );
  }
}
