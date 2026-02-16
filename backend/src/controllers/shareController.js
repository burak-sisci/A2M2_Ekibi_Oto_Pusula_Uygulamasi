const Listing = require('../models/Listing');

// @desc    Paylaşım bilgisi oluştur
// @route   GET /api/share/:listingId
// @access  Public
const getShareInfo = async (req, res) => {
    try {
        const listing = await Listing.findById(req.params.listingId);
        if (!listing) {
            return res.status(404).json({ message: 'İlan bulunamadı' });
        }

        // Frontend URL'sine göre paylaşım linki oluştur
        const baseUrl = process.env.FRONTEND_URL || 'http://localhost:5173';
        const shareUrl = `${baseUrl}/listings/${listing._id}`;

        res.json({
            title: `${listing.brand} ${listing.model} - ${listing.year}`,
            description: `${listing.brand} ${listing.model} ${listing.year} | ${listing.km} km | ${listing.fuelType} | ${listing.price.toLocaleString('tr-TR')} TL`,
            url: shareUrl,
        });
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

module.exports = { getShareInfo };
