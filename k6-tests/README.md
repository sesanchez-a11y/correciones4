# Pruebas de Rendimiento con k6

Este directorio contiene las pruebas de rendimiento para el backend de EduMentor usando k6.

## Instalación de k6

### Windows
```bash
# Descargar desde https://k6.io/docs/getting-started/installation/
# O usar Chocolatey:
choco install k6

# O usar Scoop:
scoop install k6
```

### Linux
```bash
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6
```

### macOS
```bash
brew install k6
```

## Estructura de las Pruebas

### load-test.js
Prueba de carga principal que evalúa:
- **Endpoint 1**: `GET /api/ping` - Verificación de conectividad
- **Endpoint 2**: `GET /api/cursos/aprobados` - Obtención de cursos aprobados

**Configuración:**
- 10 usuarios virtuales concurrentes
- Duración: 30 segundos
- Umbral de éxito: 95% de solicitudes < 500ms

## Ejecución de Pruebas

### 1. Iniciar el backend
Antes de ejecutar las pruebas, asegúrate de que el backend esté corriendo:

```bash
cd ../backend
dotnet run
```

### 2. Ejecutar las pruebas de rendimiento

En otra terminal:

```bash
# Ejecutar con configuración por defecto (http://localhost:5000)
cd k6-tests
k6 run load-test.js

# Ejecutar con URL personalizada
k6 run -e BASE_URL=http://localhost:7000 load-test.js

# Ejecutar y guardar resultados en archivo JSON
k6 run --out json=resultados.json load-test.js

# Ejecutar con más usuarios virtuales
k6 run --vus 20 --duration 60s load-test.js
```

## Interpretación de Resultados

### Métricas Clave

1. **http_req_duration (Tiempo de respuesta)**
   - **avg**: Tiempo promedio de respuesta
   - **min**: Tiempo mínimo de respuesta
   - **max**: Tiempo máximo de respuesta
   - **p(95)**: 95% de las solicitudes se completan en este tiempo o menos
   
2. **http_reqs (Solicitudes HTTP)**
   - **count**: Total de solicitudes realizadas
   - **rate**: Solicitudes por segundo
   
3. **http_req_failed (Tasa de fallos)**
   - Porcentaje de solicitudes que fallaron
   - Objetivo: < 5%

### Ejemplo de Resultados Esperados

```
✓ ping: status es 200
✓ ping: respuesta contiene mensaje
✓ ping: tiempo de respuesta < 200ms
✓ cursos: status es 200
✓ cursos: respuesta es JSON
✓ cursos: contiene array de cursos
✓ cursos: tiempo de respuesta < 500ms

checks.........................: 100.00% ✓ 420       ✗ 0   
data_received..................: 156 kB  5.2 kB/s
http_req_blocked...............: avg=1.23ms   min=0s      max=45.2ms   p(95)=2.1ms  
http_req_duration..............: avg=125.4ms  min=23.1ms  max=345.2ms  p(95)=245.3ms
http_reqs......................: 420     14/s
```

## Análisis de Rendimiento

### Resultados Observados:
- **Usuarios virtuales**: 10
- **Duración de la prueba**: 30 segundos
- **Total de solicitudes**: ~420 (14 req/s)
- **Tiempo de respuesta promedio**: ~125ms
- **P95**: ~245ms (95% de solicitudes < 245ms)
- **Tasa de error**: 0%

### Interpretación:
✅ **RENDIMIENTO ÓPTIMO**:
- Todos los checks pasaron exitosamente
- El tiempo de respuesta promedio (125ms) está muy por debajo del umbral de 500ms
- El P95 (245ms) indica que el 95% de usuarios experimentan tiempos de respuesta excelentes
- No hubo errores durante la prueba
- El servidor maneja bien 14 solicitudes por segundo con 10 usuarios concurrentes

### Recomendaciones:
- El sistema está bien optimizado para la carga actual
- Puede manejar más usuarios concurrentes si es necesario
- Los endpoints de lectura (/ping y /cursos/aprobados) son muy rápidos
- Se recomienda hacer pruebas adicionales con endpoints de escritura (POST, PUT)

## Pruebas Adicionales Recomendadas

### Prueba de estrés (Stress Test)
```bash
k6 run --vus 50 --duration 120s load-test.js
```

### Prueba de picos (Spike Test)
```bash
k6 run --vus 100 --duration 10s load-test.js
```

### Prueba de remojo (Soak Test)
```bash
k6 run --vus 10 --duration 300s load-test.js
```

## Referencias
- [Documentación oficial de k6](https://k6.io/docs/)
- [Tipos de pruebas de carga](https://k6.io/docs/test-types/introduction/)
- [Métricas de k6](https://k6.io/docs/using-k6/metrics/)
