const jwt = require('jsonwebtoken');
const User = require('../models/User');

// Token oluşturma yardımcı fonksiyonu
const generateToken = (id) => {
    return jwt.sign({ id }, process.env.JWT_SECRET, { expiresIn: '30d' });
};

// @desc    Kullanıcı kaydı
// @route   POST /api/auth/register
// @access  Public
const register = async (req, res) => {
    try {
        const { name, email, password } = req.body;

        const userExists = await User.findOne({ email });
        if (userExists) {
            return res.status(400).json({ message: 'Bu e-posta adresi zaten kayıtlı' });
        }

        const user = await User.create({ name, email, password });

        res.status(201).json({
            _id: user._id,
            name: user.name,
            email: user.email,
            token: generateToken(user._id),
        });
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

// @desc    Kullanıcı girişi
// @route   POST /api/auth/login
// @access  Public
const login = async (req, res) => {
    try {
        const { email, password } = req.body;

        const user = await User.findOne({ email }).select('+password');
        if (!user) {
            return res.status(401).json({ message: 'Geçersiz e-posta veya şifre' });
        }

        const isMatch = await user.matchPassword(password);
        if (!isMatch) {
            return res.status(401).json({ message: 'Geçersiz e-posta veya şifre' });
        }

        res.json({
            _id: user._id,
            name: user.name,
            email: user.email,
            token: generateToken(user._id),
        });
    } catch (error) {
        res.status(500).json({ message: 'Sunucu hatası', error: error.message });
    }
};

// @desc    Kullanıcı çıkışı
// @route   POST /api/auth/logout
// @access  Private
const logout = async (req, res) => {
    // JWT tabanlı auth olduğu için sunucu tarafında yapılacak bir şey yok
    // Client tarafında token silinecek
    res.json({ message: 'Çıkış başarılı' });
};

// @desc    Kullanıcı profili
// @route   GET /api/auth/me
// @access  Private
const getMe = async (req, res) => {
    res.json({
        _id: req.user._id,
        name: req.user.name,
        email: req.user.email,
    });
};

module.exports = { register, login, logout, getMe };
