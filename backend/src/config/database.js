const sqlite3 = require('sqlite3').verbose();
const path = require('path');

const dbPath = path.join(__dirname, '../../database.sqlite');

const db = new sqlite3.Database(dbPath, (err) => {
  if (err) {
    console.error('Error al conectar con la base de datos:', err);
  } else {
    console.log('Conectado a la base de datos SQLite');
    initializeDatabase();
  }
});

function initializeDatabase() {
  db.serialize(() => {
    // Tabla de usuarios con ciudad de origen y coordenadas
    db.run(`
      CREATE TABLE IF NOT EXISTS users (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        username TEXT UNIQUE NOT NULL,
        password TEXT NOT NULL,
        home_city TEXT,
        home_latitude REAL,
        home_longitude REAL,
        created_at DATETIME DEFAULT CURRENT_TIMESTAMP
      )
    `, (err) => {
      if (err) {
        console.error('Error al crear tabla users:', err);
      } else {
        console.log('Tabla users lista');
      }
    });

    // Tabla de historial de consultas climaticas
    db.run(`
      CREATE TABLE IF NOT EXISTS weather_history (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        user_id INTEGER NOT NULL,
        city TEXT NOT NULL,
        latitude REAL,
        longitude REAL,
        temperature REAL,
        humidity INTEGER,
        wind_speed REAL,
        weather_code INTEGER,
        query_time DATETIME DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (user_id) REFERENCES users(id)
      )
    `, (err) => {
      if (err) {
        console.error('Error al crear tabla weather_history:', err);
      } else {
        console.log('Tabla weather_history lista');
      }
    });
  });
}

module.exports = db;