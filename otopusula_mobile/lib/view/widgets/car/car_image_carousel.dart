import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import '../../../core/constants/app_constants.dart';
import '../../../core/theme/app_colors.dart';

class CarImageCarousel extends StatefulWidget {
  final List<String> imageUrls;

  const CarImageCarousel({super.key, required this.imageUrls});

  @override
  State<CarImageCarousel> createState() => _CarImageCarouselState();
}

class _CarImageCarouselState extends State<CarImageCarousel> {
  int _currentIndex = 0;
  final _controller = PageController();

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (widget.imageUrls.isEmpty) {
      return _placeholder(MediaQuery.of(context).size.width, 240);
    }
    return Stack(
      alignment: Alignment.bottomCenter,
      children: [
        SizedBox(
          height: 240,
          child: PageView.builder(
            controller: _controller,
            itemCount: widget.imageUrls.length,
            onPageChanged: (i) => setState(() => _currentIndex = i),
            itemBuilder: (_, index) => CachedNetworkImage(
              imageUrl: widget.imageUrls[index],
              fit: BoxFit.cover,
              placeholder: (_, __) => _placeholder(double.infinity, 240),
              errorWidget: (_, __, ___) => _placeholder(double.infinity, 240),
            ),
          ),
        ),
        if (widget.imageUrls.length > 1)
          Padding(
            padding: const EdgeInsets.only(bottom: AppConstants.space8),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: List.generate(
                widget.imageUrls.length,
                (i) => AnimatedContainer(
                  duration: const Duration(milliseconds: 200),
                  margin: const EdgeInsets.symmetric(horizontal: 3),
                  width: _currentIndex == i ? 16 : 6,
                  height: 6,
                  decoration: BoxDecoration(
                    color: _currentIndex == i ? AppColors.primary : Colors.white54,
                    borderRadius: BorderRadius.circular(AppConstants.radiusPill),
                  ),
                ),
              ),
            ),
          ),
      ],
    );
  }

  Widget _placeholder(double width, double height) => Container(
        width: width,
        height: height,
        color: AppColors.surfaceMuted,
        child: const Icon(Icons.directions_car, color: AppColors.border, size: 48),
      );
}
