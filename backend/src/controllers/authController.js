const bcrypt = require('bcrypt');
const https = require('node:https');
const db = require('../config/database');

// Función para validar seguridad de contraseña
function validatePasswordStrength(password) {
  const minLength = 8;
  const hasUpperCase = /[A-Z]/.test(password);
  const hasLowerCase = /[a-z]/.test(password);
  const hasNumbers = /\d/.test(password);
  const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);

  if (password.length < minLength) {
    return { valid: false, message: 'La contraseña debe tener al menos 8 caracteres' };
  }
  if (!hasUpperCase) {
    return { valid: false, message: 'La contraseña debe contener al menos una letra mayúscula' };
  }
  if (!hasLowerCase) {
    return { valid: false, message: 'La contraseña debe contener al menos una letra minúscula' };
  }
  if (!hasNumbers) {
    return { valid: false, message: 'La contraseña debe contener al menos un número' };
  }
  if (!hasSpecialChar) {
    return { valid: false, message: 'La contraseña debe contener al menos un carácter especial (!@#$%^&*...)' };
  }

  return { valid: true };
}

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
              city: location.name,
              latitude: location.latitude,
              longitude: location.longitude,
              country: location.country
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

const register = async (req, res) => {
  const { username, password } = req.body;

  if (!username || !password) {
    return res.status(400).json({ error: 'Usuario y contraseña son requeridos' });
  }

  if (username.length < 3) {
    return res.status(400).json({ error: 'El usuario debe tener al menos 3 caracteres' });
  }

  // Validar fortaleza de la contraseña
  const passwordValidation = validatePasswordStrength(password);
  if (!passwordValidation.valid) {
    return res.status(400).json({ error: passwordValidation.message });
  }

  try {
    // Usar factor de costo 12 para mayor seguridad (10 es el default)
    const hashedPassword = await bcrypt.hash(password, 12);

    db.run(
      'INSERT INTO users (username, password) VALUES (?, ?)',
      [username, hashedPassword],
      function (err) {
        if (err) {
          if (err.message.includes('UNIQUE constraint failed')) {
            return res.status(400).json({ error: 'El usuario ya existe' });
          }
          return res.status(500).json({ error: 'Error al registrar usuario' });
        }

        res.status(201).json({
          message: 'Usuario registrado exitosamente',
          userId: this.lastID,
          username: username
        });
      }
    );
  } catch (error) {
    res.status(500).json({ error: 'Error en el servidor' });
  }
};

const login = async (req, res) => {
  const { username, password } = req.body;

  if (!username || !password) {
    return res.status(400).json({ error: 'Usuario y contraseña son requeridos' });
  }

  db.get(
    'SELECT * FROM users WHERE username = ?',
    [username],
    async (err, user) => {
      if (err) {
        return res.status(500).json({ error: 'Error en el servidor' });
      }

      if (!user) {
        return res.status(401).json({ error: 'Usuario o contraseña incorrectos' });
      }

      try {
        const isValidPassword = await bcrypt.compare(password, user.password);

        if (!isValidPassword) {
          return res.status(401).json({ error: 'Usuario o contraseña incorrectos' });
        }

        res.json({
          message: 'Inicio de sesión exitoso',
          userId: user.id,
          username: user.username,
          homeCity: user.home_city,
          homeLatitude: user.home_latitude,
          homeLongitude: user.home_longitude
        });
      } catch (error) {
        res.status(500).json({ error: 'Error en el servidor' });
      }
    }
  );
};

const updateHomeCity = async (req, res) => {
  const { userId } = req.params;
  const { homeCity } = req.body;

  if (!homeCity) {
    return res.status(400).json({ error: 'Ciudad de origen es requerida' });
  }

  try {
    const location = await geocodeCity(homeCity);

    db.run(
      'UPDATE users SET home_city = ?, home_latitude = ?, home_longitude = ? WHERE id = ?',
      [location.city, location.latitude, location.longitude, userId],
      function (err) {
        if (err) {
          return res.status(500).json({ error: 'Error al actualizar ciudad' });
        }

        if (this.changes === 0) {
          return res.status(404).json({ error: 'Usuario no encontrado' });
        }

        res.json({
          message: 'Ciudad de origen actualizada',
          homeCity: location.city,
          homeLatitude: location.latitude,
          homeLongitude: location.longitude
        });
      }
    );
  } catch (error) {
    if (error.message === 'Ciudad no encontrada') {
      return res.status(404).json({ error: 'Ciudad no encontrada. Intente con otro nombre' });
    }
    res.status(500).json({ error: 'Error en el servidor' });
  }
};

module.exports = { register, login, updateHomeCity };