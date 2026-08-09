const db = require('../config/database');

const VALID_CLOTHING_STYLES = ['casual', 'formal', 'deportivo'];
const VALID_COLD_SENSITIVITIES = ['friolento', 'normal', 'caluroso'];

const DEFAULT_PREFERENCES = {
  defaultClothingStyle: 'casual',
  coldSensitivity: 'normal'
};

const getPreferences = (req, res) => {
  const { userId } = req.params;

  if (!userId) {
    return res.status(400).json({ error: 'ID de usuario requerido' });
  }

  db.get(
    'SELECT default_clothing_style, cold_sensitivity FROM user_preferences WHERE user_id = ?',
    [userId],
    (err, row) => {
      if (err) {
        console.error('Error al obtener preferencias:', err);
        return res.status(500).json({ error: 'Error al obtener preferencias' });
      }

      if (!row) {
        return res.json(DEFAULT_PREFERENCES);
      }

      res.json({
        defaultClothingStyle: row.default_clothing_style,
        coldSensitivity: row.cold_sensitivity
      });
    }
  );
};

const updatePreferences = (req, res) => {
  const { userId } = req.params;
  const { defaultClothingStyle, coldSensitivity } = req.body;

  if (!userId) {
    return res.status(400).json({ error: 'ID de usuario requerido' });
  }

  const style = defaultClothingStyle || DEFAULT_PREFERENCES.defaultClothingStyle;
  const sensitivity = coldSensitivity || DEFAULT_PREFERENCES.coldSensitivity;

  if (!VALID_CLOTHING_STYLES.includes(style)) {
    return res.status(400).json({
      error: `Estilo de vestimenta inválido. Opciones: ${VALID_CLOTHING_STYLES.join(', ')}`
    });
  }

  if (!VALID_COLD_SENSITIVITIES.includes(sensitivity)) {
    return res.status(400).json({
      error: `Sensibilidad al frío inválida. Opciones: ${VALID_COLD_SENSITIVITIES.join(', ')}`
    });
  }

  // sqlite3 no soporta INSERT ... ON CONFLICT de forma consistente en todas las
  // versiones; se verifica primero si existe la fila (mismo patron que saveHistory
  // en historyController.js) y se decide INSERT o UPDATE.
  db.get(
    'SELECT user_id FROM user_preferences WHERE user_id = ?',
    [userId],
    (err, row) => {
      if (err) {
        console.error('Error al verificar preferencias:', err);
        return res.status(500).json({ error: 'Error al guardar preferencias' });
      }

      const onSaved = (err) => {
        if (err) {
          console.error('Error al guardar preferencias:', err);
          return res.status(500).json({ error: 'Error al guardar preferencias' });
        }

        res.json({
          message: 'Preferencias actualizadas',
          defaultClothingStyle: style,
          coldSensitivity: sensitivity
        });
      };

      if (row) {
        db.run(
          'UPDATE user_preferences SET default_clothing_style = ?, cold_sensitivity = ? WHERE user_id = ?',
          [style, sensitivity, userId],
          onSaved
        );
      } else {
        db.run(
          'INSERT INTO user_preferences (user_id, default_clothing_style, cold_sensitivity) VALUES (?, ?, ?)',
          [userId, style, sensitivity],
          onSaved
        );
      }
    }
  );
};

module.exports = { getPreferences, updatePreferences };
