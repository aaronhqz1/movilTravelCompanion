const express = require('express');
const cors = require('cors');
require('dotenv').config();

const routes = require('./routes');

const app = express();
const PORT = process.env.PORT || 3000;

// Middlewares
app.use(cors());
app.use(express.json());

// Rutas
app.use('/api', routes);

// Ruta de prueba
app.get('/', (req, res) => {
  res.json({ 
    message: 'API de Clima funcionando correctamente',
    version: '2.0.0',
    api: 'Open-Meteo (gratuita)',
    features: {
      weather: true,
      openai: process.env.OPENAI_API_KEY ? true : false
    }
  });
});

// Manejo de errores
app.use((err, req, res, next) => {
  console.error(err.stack);
  res.status(500).json({ error: 'Algo salió mal en el servidor' });
});

// Iniciar servidor
app.listen(PORT, () => {
  console.log(`Servidor corriendo en http://localhost:${PORT}`);
  console.log('Base de datos SQLite inicializada');
  console.log(`OpenAI: ${process.env.OPENAI_API_KEY ? 'Configurado ✓' : 'No configurado ✗'}`);
});