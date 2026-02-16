const Listing = require('../models/Listing');

// @desc    Tüm ilanları getir
// @route   GET /api/listings
// @access  Public
const getListings = async (req, res) => {
    try {
        const { brand, fuelType, gearType, minPrice, maxPrice, sort } = req.query;
        const filter = {};

        if (brand) filter.brand = new RegExp(brand, 'i');
        if (fuelType) filter.fuelType = fuelType;
        if (gearType) filter.gearType = gearType;
        if (minPrice || maxPrice) {
            filter.price = {};
            if (minPrice) filter.price.$gte = Number(minPrice);
            if (maxPrice) filter.price.$lte = Number(maxPrice);
        }

        let query = Listing.find(filter).populate('userId', 'name email');

        // Sıralama
        if (sort === 'price_asc') query = query.sort({ price: 1 });
        else if (sort === 'price_desc') query = query.sort({ price: -1 });
        else query = query.sort({ createdAt: -1 });

        const listings = await query;
        res.json(listings);
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

// @desc    Tek ilan getir
// @route   GET /api/listings/:id
// @access  Public
const getListingById = async (req, res) => {
    try {
        const listing = await Listing.findById(req.params.id).populate('userId', 'name email');
        if (!listing) {
            return res.status(404).json({ message: 'İlan bulunamadı' });
        }
        res.json(listing);
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

// @desc    Yeni ilan oluştur
// @route   POST /api/listings
// @access  Private
const createListing = async (req, res) => {
    try {
        const listing = await Listing.create({
            ...req.body,
            userId: req.user._id,
        });
        res.status(201).json(listing);
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

// @desc    İlan güncelle
// @route   PUT /api/listings/:id
// @access  Private (sadece ilan sahibi)
const updateListing = async (req, res) => {
    try {
        const listing = await Listing.findById(req.params.id);
        if (!listing) {
            return res.status(404).json({ message: 'İlan bulunamadı' });
        }
        if (listing.userId.toString() !== req.user._id.toString()) {
            return res.status(403).json({ message: 'Bu işlem için yetkiniz yok' });
        }

        const updated = await Listing.findByIdAndUpdate(req.params.id, req.body, { new: true });
        res.json(updated);
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

// @desc    İlan sil
// @route   DELETE /api/listings/:id
// @access  Private (sadece ilan sahibi)
const deleteListing = async (req, res) => {
    try {
        const listing = await Listing.findById(req.params.id);
        if (!listing) {
            return res.status(404).json({ message: 'İlan bulunamadı' });
        }
        if (listing.userId.toString() !== req.user._id.toString()) {
            return res.status(403).json({ message: 'Bu işlem için yetkiniz yok' });
        }

        await Listing.findByIdAndDelete(req.params.id);
        res.json({ message: 'İlan başarıyla silindi' });
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

module.exports = { getListings, getListingById, createListing, updateListing, deleteListing };
