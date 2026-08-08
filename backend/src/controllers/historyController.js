const db = require('../config/database');

const saveHistory = (req, res) => {
  const { userId, city, latitude, longitude, temperature, humidity, wind_speed, weather_code } = req.body;

  if (!userId || !city) {
    return res.status(400).json({ error: 'Datos incompletos' });
  }

  // Verificar si ya existe una búsqueda reciente de la misma ciudad (últimas 24 horas)
  db.get(
    `SELECT id FROM weather_history 
     WHERE user_id = ? AND city = ? 
     AND datetime(query_time) > datetime('now', '-24 hours')
     ORDER BY query_time DESC 
     LIMIT 1`,
    [userId, city],
    (err, row) => {
      if (err) {
        console.error('Error al verificar historial:', err);
        return res.status(500).json({ error: 'Error al verificar historial' });
      }

      if (row) {
        return res.status(409).json({ 
          error: 'Ya guardaste esta ciudad recientemente. Intenta con otra ciudad.' 
        });
      }

      // Si no existe, guardar la nueva búsqueda
      db.run(
        `INSERT INTO weather_history 
         (user_id, city, latitude, longitude, temperature, humidity, wind_speed, weather_code) 
         VALUES (?, ?, ?, ?, ?, ?, ?, ?)`,
        [userId, city, latitude, longitude, temperature, humidity, wind_speed, weather_code],
        function (err) {
          if (err) {
            console.error('Error al guardar en historial:', err);
            return res.status(500).json({ error: 'Error al guardar en historial' });
          }

          res.status(201).json({
            message: 'Consulta guardada en historial',
            id: this.lastID
          });
        }
      );
    }
  );
};

const getRecentHistory = (req, res) => {
  const { userId } = req.params;

  if (!userId) {
    return res.status(400).json({ error: 'ID de usuario requerido' });
  }

  db.all(
    `SELECT * FROM weather_history 
     WHERE user_id = ? 
     ORDER BY query_time DESC
     LIMIT 3`,
    [userId],
    (err, rows) => {
      if (err) {
        console.error('Error al obtener historial:', err);
        return res.status(500).json({ error: 'Error al obtener historial' });
      }

      res.json(rows);
    }
  );
};

const getAllHistory = (req, res) => {
  const { userId } = req.params;

  if (!userId) {
    return res.status(400).json({ error: 'ID de usuario requerido' });
  }

  db.all(
    `SELECT * FROM weather_history 
     WHERE user_id = ? 
     ORDER BY query_time DESC`,
    [userId],
    (err, rows) => {
      if (err) {
        console.error('Error al obtener historial:', err);
        return res.status(500).json({ error: 'Error al obtener historial' });
      }

      res.json(rows);
    }
  );
};

module.exports = { saveHistory, getRecentHistory, getAllHistory };