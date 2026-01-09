import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 10,           // usuarios simultáneos
  duration: '10s',   // duración de la prueba
};

export default function () {
  const res = http.get('http://localhost:3000/api/users');

  // Verificación de cabeceras
  check(res, {
    'status es 200': (r) => r.status === 200,
    'tiene content-type': (r) =>
      r.headers['Content-Type'] !== undefined,
  });

  sleep(1);
}
import sleep;
