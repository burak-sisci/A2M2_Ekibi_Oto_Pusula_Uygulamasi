const express = require('express');
const router = express.Router();
const {
    getListings,
    getListingById,
    createListing,
    updateListing,
    deleteListing,
} = require('../controllers/listingController');
const { protect } = require('../middleware/authMiddleware');
const { cacheMiddleware, invalidateCache } = require('../middleware/cacheMiddleware');

// Public — cache'li
router.get('/', cacheMiddleware(300), getListings);           // 5 dk cache
router.get('/:id', cacheMiddleware(120), getListingById);     // 2 dk cache

// Protected — cache invalidation ile
router.post('/', protect, invalidateCache('cache:/api/listings*'), createListing);
router.put('/:id', protect, invalidateCache('cache:/api/listings*'), updateListing);
router.delete('/:id', protect, invalidateCache('cache:/api/listings*'), deleteListing);

module.exports = router;
