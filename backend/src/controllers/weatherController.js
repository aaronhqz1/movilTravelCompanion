const https = require('node:https');

// Base de datos de ciudades principales del mundo con coordenadas
// Fuente: Capitales y ciudades principales de cada continente
const worldCities = [
  // América
  { name: 'San José', country: 'Costa Rica', lat: 9.9281, lon: -84.0907 },
  { name: 'New York', country: 'United States', lat: 40.7128, lon: -74.0060 },
  { name: 'Los Angeles', country: 'United States', lat: 34.0522, lon: -118.2437 },
  { name: 'Chicago', country: 'United States', lat: 41.8781, lon: -87.6298 },
  { name: 'Miami', country: 'United States', lat: 25.7617, lon: -80.1918 },
  { name: 'Toronto', country: 'Canada', lat: 43.6532, lon: -79.3832 },
  { name: 'Vancouver', country: 'Canada', lat: 49.2827, lon: -123.1207 },
  { name: 'Mexico City', country: 'Mexico', lat: 19.4326, lon: -99.1332 },
  { name: 'Guadalajara', country: 'Mexico', lat: 20.6597, lon: -103.3496 },
  { name: 'Buenos Aires', country: 'Argentina', lat: -34.6037, lon: -58.3816 },
  { name: 'São Paulo', country: 'Brazil', lat: -23.5505, lon: -46.6333 },
  { name: 'Rio de Janeiro', country: 'Brazil', lat: -22.9068, lon: -43.1729 },
  { name: 'Lima', country: 'Peru', lat: -12.0464, lon: -77.0428 },
  { name: 'Santiago', country: 'Chile', lat: -33.4489, lon: -70.6693 },
  { name: 'Bogotá', country: 'Colombia', lat: 4.7110, lon: -74.0721 },
  { name: 'Caracas', country: 'Venezuela', lat: 10.4806, lon: -66.9036 },
  { name: 'Quito', country: 'Ecuador', lat: -0.1807, lon: -78.4678 },
  { name: 'La Paz', country: 'Bolivia', lat: -16.5000, lon: -68.1500 },
  
  // Europa
  { name: 'London', country: 'United Kingdom', lat: 51.5074, lon: -0.1278 },
  { name: 'Paris', country: 'France', lat: 48.8566, lon: 2.3522 },
  { name: 'Berlin', country: 'Germany', lat: 52.5200, lon: 13.4050 },
  { name: 'Madrid', country: 'Spain', lat: 40.4168, lon: -3.7038 },
  { name: 'Barcelona', country: 'Spain', lat: 41.3851, lon: 2.1734 },
  { name: 'Rome', country: 'Italy', lat: 41.9028, lon: 12.4964 },
  { name: 'Milan', country: 'Italy', lat: 45.4642, lon: 9.1900 },
  { name: 'Amsterdam', country: 'Netherlands', lat: 52.3676, lon: 4.9041 },
  { name: 'Vienna', country: 'Austria', lat: 48.2082, lon: 16.3738 },
  { name: 'Zurich', country: 'Switzerland', lat: 47.3769, lon: 8.5417 },
  { name: 'Stockholm', country: 'Sweden', lat: 59.3293, lon: 18.0686 },
  { name: 'Oslo', country: 'Norway', lat: 59.9139, lon: 10.7522 },
  { name: 'Copenhagen', country: 'Denmark', lat: 55.6761, lon: 12.5683 },
  { name: 'Helsinki', country: 'Finland', lat: 60.1695, lon: 24.9354 },
  { name: 'Warsaw', country: 'Poland', lat: 52.2297, lon: 21.0122 },
  { name: 'Prague', country: 'Czech Republic', lat: 50.0755, lon: 14.4378 },
  { name: 'Athens', country: 'Greece', lat: 37.9838, lon: 23.7275 },
  { name: 'Lisbon', country: 'Portugal', lat: 38.7223, lon: -9.1393 },
  { name: 'Dublin', country: 'Ireland', lat: 53.3498, lon: -6.2603 },
  { name: 'Brussels', country: 'Belgium', lat: 50.8503, lon: 4.3517 },
  { name: 'Moscow', country: 'Russia', lat: 55.7558, lon: 37.6173 },
  { name: 'Istanbul', country: 'Turkey', lat: 41.0082, lon: 28.9784 },
  
  // Asia
  { name: 'Tokyo', country: 'Japan', lat: 35.6762, lon: 139.6503 },
  { name: 'Osaka', country: 'Japan', lat: 34.6937, lon: 135.5023 },
  { name: 'Beijing', country: 'China', lat: 39.9042, lon: 116.4074 },
  { name: 'Shanghai', country: 'China', lat: 31.2304, lon: 121.4737 },
  { name: 'Seoul', country: 'South Korea', lat: 37.5665, lon: 126.9780 },
  { name: 'Bangkok', country: 'Thailand', lat: 13.7563, lon: 100.5018 },
  { name: 'Singapore', country: 'Singapore', lat: 1.3521, lon: 103.8198 },
  { name: 'Dubai', country: 'United Arab Emirates', lat: 25.2048, lon: 55.2708 },
  { name: 'Mumbai', country: 'India', lat: 19.0760, lon: 72.8777 },
  { name: 'Delhi', country: 'India', lat: 28.7041, lon: 77.1025 },
  { name: 'Bangalore', country: 'India', lat: 12.9716, lon: 77.5946 },
  { name: 'Karachi', country: 'Pakistan', lat: 24.8607, lon: 67.0011 },
  { name: 'Manila', country: 'Philippines', lat: 14.5995, lon: 120.9842 },
  { name: 'Jakarta', country: 'Indonesia', lat: -6.2088, lon: 106.8456 },
  { name: 'Ho Chi Minh City', country: 'Vietnam', lat: 10.8231, lon: 106.6297 },
  { name: 'Hanoi', country: 'Vietnam', lat: 21.0285, lon: 105.8542 },
  { name: 'Kuala Lumpur', country: 'Malaysia', lat: 3.1390, lon: 101.6869 },
  { name: 'Tel Aviv', country: 'Israel', lat: 32.0853, lon: 34.7818 },
  { name: 'Riyadh', country: 'Saudi Arabia', lat: 24.7136, lon: 46.6753 },
  
  // África
  { name: 'Cairo', country: 'Egypt', lat: 30.0444, lon: 31.2357 },
  { name: 'Nairobi', country: 'Kenya', lat: -1.2921, lon: 36.8219 },
  { name: 'Cape Town', country: 'South Africa', lat: -33.9249, lon: 18.4241 },
  { name: 'Johannesburg', country: 'South Africa', lat: -26.2041, lon: 28.0473 },
  { name: 'Lagos', country: 'Nigeria', lat: 6.5244, lon: 3.3792 },
  { name: 'Casablanca', country: 'Morocco', lat: 33.5731, lon: -7.5898 },
  { name: 'Accra', country: 'Ghana', lat: 5.6037, lon: -0.1870 },
  { name: 'Addis Ababa', country: 'Ethiopia', lat: 9.0320, lon: 38.7469 },
  
  // Oceanía
  { name: 'Sydney', country: 'Australia', lat: -33.8688, lon: 151.2093 },
  { name: 'Melbourne', country: 'Australia', lat: -37.8136, lon: 144.9631 },
  { name: 'Brisbane', country: 'Australia', lat: -27.4698, lon: 153.0251 },
  { name: 'Perth', country: 'Australia', lat: -31.9505, lon: 115.8605 },
  { name: 'Auckland', country: 'New Zealand', lat: -36.8485, lon: 174.7633 },
  { name: 'Wellington', country: 'New Zealand', lat: -41.2865, lon: 174.7762 }
];

// Función para geocodificar una ciudad
function geocodeCity(cityName) {
  return new Promise((resolve, reject) => {
    const url = `https://geocoding-api.open-meteo.com/v1/search?name=${encodeURIComponent(cityName)}&count=1&language=es&format=json`;
    
    https.get(url, (res) => {
      let data = '';
      
      res.on('data', (chunk) => {
        data += chunk;
      });
      
      res.on('end', () => {
        try {
          const result = JSON.parse(data);
          if (result.results && result.results.length > 0) {
            const location = result.results[0];
            resolve({
              city: `${location.name}, ${location.country}`,
              latitude: location.latitude,
              longitude: location.longitude
            });
          } else {
            reject(new Error('Ciudad no encontrada'));
          }
        } catch (error) {
          reject(error);
        }
      });
    }).on('error', (err) => {
      reject(err);
    });
  });
}

// Función para obtener clima de coordenadas
function getWeatherByCoordinates(latitude, longitude) {
  return new Promise((resolve, reject) => {
    const url = `https://api.open-meteo.com/v1/forecast?latitude=${latitude}&longitude=${longitude}&current=temperature_2m,relative_humidity_2m,wind_speed_10m,weather_code&hourly=temperature_2m,weather_code&timezone=auto&forecast_days=1`;
    
    https.get(url, (res) => {
      let data = '';
      
      res.on('data', (chunk) => {
        data += chunk;
      });
      
      res.on('end', () => {
        try {
          const weatherData = JSON.parse(data);
          
          const response = {
            temperature: weatherData.current.temperature_2m,
            humidity: weatherData.current.relative_humidity_2m,
            wind_speed: weatherData.current.wind_speed_10m,
            weather_code: weatherData.current.weather_code,
            hourly_forecast: weatherData.hourly.time.map((time, index) => ({
              time: time,
              temperature: weatherData.hourly.temperature_2m[index],
              weather_code: weatherData.hourly.weather_code[index]
            })).slice(0, 24)
          };
          
          resolve(response);
        } catch (error) {
          reject(error);
        }
      });
    }).on('error', (err) => {
      reject(err);
    });
  });
}

// Obtener clima de una ciudad aleatoria
const getRandomWeather = async (req, res) => {
  try {
    // Seleccionar ciudad aleatoria de la base de datos
    const randomCity = worldCities[Math.floor(Math.random() * worldCities.length)];
    
    // Obtener clima usando las coordenadas directamente
    const weather = await getWeatherByCoordinates(randomCity.lat, randomCity.lon);
    
    res.json({
      city: `${randomCity.name}, ${randomCity.country}`,
      latitude: randomCity.lat,
      longitude: randomCity.lon,
      ...weather
    });
  } catch (error) {
    res.status(500).json({ error: 'Error al obtener clima aleatorio' });
  }
};

// Buscar clima por nombre de ciudad
const searchWeather = async (req, res) => {
  const { city } = req.query;

  if (!city) {
    return res.status(400).json({ error: 'El nombre de la ciudad es requerido' });
  }

  try {
    const location = await geocodeCity(city);
    const weather = await getWeatherByCoordinates(location.latitude, location.longitude);
    
    res.json({
      city: location.city,
      latitude: location.latitude,
      longitude: location.longitude,
      ...weather
    });
  } catch (error) {
    if (error.message === 'Ciudad no encontrada') {
      return res.status(404).json({ error: 'Ciudad no encontrada' });
    }
    res.status(500).json({ error: 'Error al obtener clima' });
  }
};

// Obtener clima por coordenadas
const getWeatherByCoords = async (req, res) => {
  const { lat, lon } = req.query;

  if (!lat || !lon) {
    return res.status(400).json({ error: 'Latitud y longitud son requeridas' });
  }

  try {
    const weather = await getWeatherByCoordinates(parseFloat(lat), parseFloat(lon));
    res.json(weather);
  } catch (error) {
    res.status(500).json({ error: 'Error al obtener clima' });
  }
};

// Obtener lista de ciudades desde la base de datos local
const getCitiesList = (req, res) => {
  try {
    // Ordenar alfabéticamente por nombre
    const sortedCities = worldCities.sort((a, b) => a.name.localeCompare(b.name));
    
    res.json({
      total: sortedCities.length,
      cities: sortedCities.map(city => ({
        name: city.name,
        country: city.country,
        displayName: `${city.name}, ${city.country}`,
        latitude: city.lat,
        longitude: city.lon
      }))
    });
  } catch (error) {
    res.status(500).json({ error: 'Error al obtener lista de ciudades' });
  }
};

// Obtener estadísticas de ciudades por continente
const getCitiesStats = (req, res) => {
  try {
    const continents = {
      'América': 0,
      'Europa': 0,
      'Asia': 0,
      'África': 0,
      'Oceanía': 0
    };
    
    worldCities.forEach(city => {
      // Clasificar por latitud/longitud aproximada
      if (city.lon < -30) continents['América']++;
      else if (city.lon >= -30 && city.lon < 40) continents['Europa']++;
      else if (city.lon >= 40 && city.lon < 150) continents['Asia']++;
      else if (city.lon >= -30 && city.lon < 60 && city.lat < 40) continents['África']++;
      else continents['Oceanía']++;
    });
    
    res.json({
      totalCities: worldCities.length,
      byContinent: continents
    });
  } catch (error) {
    res.status(500).json({ error: 'Error al obtener estadísticas' });
  }
};

module.exports = { 
  getRandomWeather, 
  searchWeather, 
  getWeatherByCoords,
  getCitiesList,
  getCitiesStats
};