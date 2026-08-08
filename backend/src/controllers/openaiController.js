const OpenAI = require('openai');

// Inicializar cliente de OpenAI
// IMPORTANTE: Necesitas establecer la variable de entorno OPENAI_API_KEY
const openai = new OpenAI({
  apiKey: process.env.OPENAI_API_KEY || 'tu-api-key-aqui'
});

// Función para interpretar código de clima WMO a descripción
function getWeatherDescription(code) {
  const descriptions = {
    0: 'Despejado',
    1: 'Mayormente despejado',
    2: 'Parcialmente nublado',
    3: 'Nublado',
    45: 'Niebla',
    48: 'Niebla con escarcha',
    51: 'Llovizna ligera',
    53: 'Llovizna moderada',
    55: 'Llovizna intensa',
    61: 'Lluvia ligera',
    63: 'Lluvia moderada',
    65: 'Lluvia intensa',
    71: 'Nevada ligera',
    73: 'Nevada moderada',
    75: 'Nevada intensa',
    80: 'Chubascos ligeros',
    81: 'Chubascos moderados',
    82: 'Chubascos violentos',
    85: 'Chubascos de nieve ligeros',
    86: 'Chubascos de nieve intensos',
    95: 'Tormenta',
    96: 'Tormenta con granizo ligero',
    99: 'Tormenta con granizo intenso'
  };
  
  return descriptions[code] || 'Desconocido';
}

const getClothingRecommendation = async (req, res) => {
  const { city, temperature, weatherCode, humidity, windSpeed, clothingStyle } = req.body;

  // Validar datos requeridos
  if (!city || temperature === undefined || weatherCode === undefined) {
    return res.status(400).json({ 
      error: 'Faltan datos requeridos: city, temperature, weatherCode' 
    });
  }

  // Validar estilo de vestimenta
  const validStyles = ['formal', 'casual', 'athletic'];
  const style = clothingStyle || 'casual';
  
  if (!validStyles.includes(style.toLowerCase())) {
    return res.status(400).json({ 
      error: 'Estilo de vestimenta inválido. Opciones: formal, casual, athletic' 
    });
  }

  try {
    const weatherDescription = getWeatherDescription(weatherCode);
    
    // Crear prompt para OpenAI
    const prompt = `Eres un asistente experto en moda y clima. 

Datos del clima en ${city}:
- Temperatura: ${temperature}°C
- Condición: ${weatherDescription}
- Humedad: ${humidity}%
- Velocidad del viento: ${windSpeed} km/h

El usuario necesita recomendaciones de vestimenta estilo: ${style.toUpperCase()}

Por favor proporciona:
1. Una breve descripción del clima (2-3 líneas)
2. Recomendaciones específicas de prendas para este estilo (5-7 prendas)
3. Accesorios recomendados (3-4 items)
4. Un consejo adicional considerando el clima

Formato de respuesta:
- Sé conciso y específico
- Menciona colores y materiales cuando sea relevante
- Considera la practicidad y comodidad
- Adapta las recomendaciones al estilo solicitado

Para estilo FORMAL: trajes, sacos, vestidos elegantes, zapatos formales
Para estilo CASUAL: jeans, camisetas, sudaderas, tenis
Para estilo ATHLETIC: ropa deportiva, zapatillas running, ropa técnica`;

    // Llamar a OpenAI API
    const completion = await openai.chat.completions.create({
      model: "gpt-3.5-turbo",
      messages: [
        {
          role: "system",
          content: "Eres un asistente experto en moda y clima que da recomendaciones de vestimenta concisas y prácticas en español."
        },
        {
          role: "user",
          content: prompt
        }
      ],
      temperature: 0.7,
      max_tokens: 500
    });

    const recommendation = completion.choices[0].message.content;

    res.json({
      success: true,
      city: city,
      weather: {
        temperature: temperature,
        condition: weatherDescription,
        humidity: humidity,
        windSpeed: windSpeed
      },
      clothingStyle: style,
      recommendation: recommendation,
      timestamp: new Date().toISOString()
    });

  } catch (error) {
    console.error('Error al consultar OpenAI:', error);
    
    // Manejar error de API key
    if (error.code === 'invalid_api_key') {
      return res.status(401).json({ 
        error: 'API Key de OpenAI no configurada o inválida. Configura OPENAI_API_KEY en las variables de entorno.' 
      });
    }
    
    // Manejar error de límite de cuota
    if (error.code === 'insufficient_quota') {
      return res.status(429).json({ 
        error: 'Límite de cuota de OpenAI excedido. Verifica tu cuenta de OpenAI.' 
      });
    }

    res.status(500).json({ 
      error: 'Error al generar recomendación de vestimenta',
      details: error.message 
    });
  }
};

module.exports = { getClothingRecommendation };