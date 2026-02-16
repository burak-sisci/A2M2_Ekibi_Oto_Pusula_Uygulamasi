const Favorite = require('../models/Favorite');

// @desc    Kullanıcının favorilerini getir
// @route   GET /api/favorites
// @access  Private
const getFavorites = async (req, res) => {
    try {
        const favorites = await Favorite.find({ userId: req.user._id })
            .populate({
                path: 'listingId',
                populate: { path: 'userId', select: 'name email' },
            });
        res.json(favorites);
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

// @desc    Favoriye ekle
// @route   POST /api/favorites/:listingId
// @access  Private
const addFavorite = async (req, res) => {
    try {
        const existingFav = await Favorite.findOne({
            userId: req.user._id,
            listingId: req.params.listingId,
        });

        if (existingFav) {
            return res.status(400).json({ message: 'Bu ilan zaten favorilerinizde' });
        }

        const favorite = await Favorite.create({
            userId: req.user._id,
            listingId: req.params.listingId,
        });

        res.status(201).json(favorite);
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

// @desc    Favoriden sil
// @route   DELETE /api/favorites/:listingId
// @access  Private
const removeFavorite = async (req, res) => {
    try {
        const favorite = await Favorite.findOneAndDelete({
            userId: req.user._id,
            listingId: req.params.listingId,
        });

        if (!favorite) {
            return res.status(404).json({ message: 'Favori bulunamadı' });
        }

        res.json({ message: 'Favori başarıyla silindi' });
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

module.exports = { getFavorites, addFavorite, removeFavorite };
