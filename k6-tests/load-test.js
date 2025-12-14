import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

/**
 * Pruebas de rendimiento con k6 para EduMentor Backend
 * 
 * Este script realiza pruebas de carga en los endpoints principales:
 * - GET /api/cursos/aprobados (obtener cursos aprobados)
 * - GET /api/ping (verificar conectividad)
 * 
 * Configuración:
 * - 10 usuarios virtuales concurrentes
 * - Duración: 30 segundos
 * - Umbral de éxito: 95% de solicitudes con tiempo < 500ms
 */

// Métrica personalizada para tasa de errores
const errorRate = new Rate('errors');

// Configuración de la prueba
export const options = {
    vus: 10, // 10 usuarios virtuales
    duration: '30s', // Duración de 30 segundos
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% de las solicitudes deben completarse en menos de 500ms
        http_req_failed: ['rate<0.05'], // Menos del 5% de solicitudes pueden fallar
        errors: ['rate<0.05'], // Menos del 5% de errores
    },
};

// URL base del backend
const BASE_URL = __ENV.BASE_URL || 'http://localhost:5000';

export default function () {
    // Test 1: Verificar conectividad del servidor
    {
        const pingRes = http.get(`${BASE_URL}/api/ping`);
        const pingCheck = check(pingRes, {
            'ping: status es 200': (r) => r.status === 200,
            'ping: respuesta contiene mensaje': (r) => r.json('message') !== undefined,
            'ping: tiempo de respuesta < 200ms': (r) => r.timings.duration < 200,
        });
        
        if (!pingCheck) {
            errorRate.add(1);
        } else {
            errorRate.add(0);
        }
    }

    sleep(0.5); // Pausa de 500ms entre solicitudes

    // Test 2: Obtener cursos aprobados
    {
        const cursosRes = http.get(`${BASE_URL}/api/cursos/aprobados`);
        const cursosCheck = check(cursosRes, {
            'cursos: status es 200': (r) => r.status === 200,
            'cursos: respuesta es JSON': (r) => {
                try {
                    const body = JSON.parse(r.body);
                    return body !== null;
                } catch (e) {
                    return false;
                }
            },
            'cursos: contiene array de cursos': (r) => {
                try {
                    const body = JSON.parse(r.body);
                    return body.cursos !== undefined && Array.isArray(body.cursos);
                } catch (e) {
                    return false;
                }
            },
            'cursos: tiempo de respuesta < 500ms': (r) => r.timings.duration < 500,
        });
        
        if (!cursosCheck) {
            errorRate.add(1);
        } else {
            errorRate.add(0);
        }
    }

    sleep(1); // Pausa de 1 segundo entre iteraciones
}

// Función que se ejecuta al finalizar la prueba
export function handleSummary(data) {
    console.log('\n========================================');
    console.log('RESUMEN DE PRUEBAS DE RENDIMIENTO');
    console.log('========================================\n');
    
    return {
        'stdout': textSummary(data, { indent: ' ', enableColors: true }),
        'k6-results.json': JSON.stringify(data),
    };
}

function textSummary(data, options) {
    const indent = options.indent || '';
    const enableColors = options.enableColors || false;
    
    let summary = '';
    
    // Información general
    summary += `${indent}Configuración de la prueba:\n`;
    summary += `${indent}  - Usuarios virtuales: ${data.options.vus}\n`;
    summary += `${indent}  - Duración: ${data.options.duration}\n\n`;
    
    // Métricas HTTP
    if (data.metrics.http_req_duration) {
        summary += `${indent}Tiempo de respuesta HTTP:\n`;
        summary += `${indent}  - Promedio: ${data.metrics.http_req_duration.values.avg.toFixed(2)}ms\n`;
        summary += `${indent}  - Mínimo: ${data.metrics.http_req_duration.values.min.toFixed(2)}ms\n`;
        summary += `${indent}  - Máximo: ${data.metrics.http_req_duration.values.max.toFixed(2)}ms\n`;
        summary += `${indent}  - P95: ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms\n\n`;
    }
    
    // Solicitudes
    if (data.metrics.http_reqs) {
        summary += `${indent}Solicitudes HTTP:\n`;
        summary += `${indent}  - Total: ${data.metrics.http_reqs.values.count}\n`;
        summary += `${indent}  - Tasa: ${data.metrics.http_reqs.values.rate.toFixed(2)}/s\n\n`;
    }
    
    // Errores
    if (data.metrics.http_req_failed) {
        const failRate = (data.metrics.http_req_failed.values.rate * 100).toFixed(2);
        summary += `${indent}Errores:\n`;
        summary += `${indent}  - Tasa de fallo: ${failRate}%\n\n`;
    }
    
    return summary;
}
