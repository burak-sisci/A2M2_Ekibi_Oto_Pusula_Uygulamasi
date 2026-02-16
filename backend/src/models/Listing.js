const mongoose = require('mongoose');

const listingSchema = new mongoose.Schema({
    userId: {
        type: mongoose.Schema.Types.ObjectId,
        ref: 'User',
        required: true,
    },
    brand: {
        type: String,
        required: [true, 'Marka alanı zorunludur'],
        trim: true,
    },
    model: {
        type: String,
        required: [true, 'Model alanı zorunludur'],
        trim: true,
    },
    year: {
        type: Number,
        required: [true, 'Yıl alanı zorunludur'],
    },
    km: {
        type: Number,
        required: [true, 'Kilometre alanı zorunludur'],
    },
    fuelType: {
        type: String,
        enum: ['Benzin', 'Dizel', 'LPG', 'Elektrik', 'Hibrit'],
        required: [true, 'Yakıt tipi zorunludur'],
    },
    gearType: {
        type: String,
        enum: ['Manuel', 'Otomatik', 'Yarı Otomatik'],
        required: [true, 'Vites tipi zorunludur'],
    },
    price: {
        type: Number,
        required: [true, 'Fiyat alanı zorunludur'],
    },
    description: {
        type: String,
        trim: true,
        default: '',
    },
    images: [{
        type: String,
    }],
}, {
    timestamps: true,
});

module.exports = mongoose.model('Listing', listingSchema);
