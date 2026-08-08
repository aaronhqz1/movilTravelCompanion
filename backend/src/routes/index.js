const express = require('express');
const { register, login, updateHomeCity } = require('../controllers/authController');
const { getRandomWeather, searchWeather, getWeatherByCoords, getCitiesList, getCitiesStats } = require('../controllers/weatherController');
const { saveHistory, getRecentHistory, getAllHistory } = require('../controllers/historyController');
const { getClothingRecommendation } = require('../controllers/openaiController');

const router = express.Router();

// Rutas de autenticación
router.post('/auth/register', register);
router.post('/auth/login', login);
router.put('/user/:userId/home', updateHomeCity);

// Rutas de clima
router.get('/weather/random', getRandomWeather);
router.get('/weather/search', searchWeather);
router.get('/weather/coordinates', getWeatherByCoords);
router.get('/weather/cities', getCitiesList);
router.get('/weather/stats', getCitiesStats);

// Rutas de historial
router.post('/history', saveHistory);
router.get('/history/:userId/recent', getRecentHistory);
router.get('/history/:userId', getAllHistory);

// Rutas de OpenAI
router.post('/ai/clothing-recommendation', getClothingRecommendation);

module.exports = router;