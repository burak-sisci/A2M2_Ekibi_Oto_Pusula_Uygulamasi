const express = require('express');
const router = express.Router();
const { getShareInfo } = require('../controllers/shareController');

router.get('/:listingId', getShareInfo);

module.exports = router;
