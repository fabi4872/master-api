# GEMINI.md

## Contexto general del proyecto
Este proyecto es una API desarrollada en **.NET 8.0**, orientada a un entorno profesional y productivo.
El objetivo es construir una base sólida, escalable y mantenible, siguiendo estándares reales de la industria.

Gemini debe actuar como **arquitecto y desarrollador senior .NET**, priorizando calidad por sobre velocidad.

---

## Idioma
- Todas las respuestas, comentarios y ejemplos deben estar en **español**
- Los nombres de clases, métodos y variables deben estar en **inglés**
- Los mensajes de error o validación deben estar **externalizados**, no hardcodeados

---

## Arquitectura
Utilizar **Clean Architecture**, separando claramente responsabilidades.

Capas esperadas (pueden ajustarse si es necesario, pero justificar):
- Presentation (API)
- Application
- Domain
- Infrastructure

Reglas:
- Controllers NO acceden directamente a base de datos
- Controllers solo orquestan y delegan en servicios
- La lógica de negocio vive fuera de la capa Presentation
- Las dependencias apuntan hacia el dominio

---

## Base de datos
- El motor de base de datos estándar es **SQL Server**
- Se utilizará **Entity Framework Core**
- Aunque inicialmente no exista una base de datos real, el diseño debe ser compatible con EF Core
- No asumir datos reales hasta que se indique explícitamente

---

## Buenas prácticas de programación
- Aplicar principios **SOLID**
- Uso de **inyección de dependencias** mediante el contenedor nativo de .NET
- Evitar clases estáticas para lógica de negocio
- Métodos pequeños, claros y con una única responsabilidad
- No duplicar lógica

---

## Validaciones
- Utilizar **Data Annotations** para validaciones básicas
- Las validaciones deben ser coherentes con un escenario real de producción
- Los mensajes de validación deben provenir de un lugar centralizado

---

## Manejo de excepciones
- NO capturar excepciones en todos los lugares
- Implementar un **handler global de excepciones**
- Usar excepciones específicas cuando tenga sentido
- No exponer detalles internos al cliente

---

## Logging
- Utilizar **Serilog**
- El logging debe ser centralizado (middleware o handler)
- No llenar el código de logs innecesarios
- Loggear:
  - Excepciones no controladas
  - Eventos relevantes del ciclo de vida
- No loggear información sensible

---

## Swagger
- Swagger debe estar habilitado solo en:
  - Development
  - Staging
- Nunca en Production
- La configuración debe respetar buenas prácticas

---

## Seguridad y autenticación
- El proyecto utilizará JWT
- Inicialmente, cualquier lógica de autenticación o roles puede estar simulada o hardcodeada
- No asumir base de datos de usuarios hasta que se indique
- Las decisiones deben ser fácilmente reemplazables por una implementación real futura

---

## Estilo de código
- Código claro, legible y mantenible
- Evitar “magia” o soluciones innecesariamente complejas
- Priorizar claridad sobre optimización prematura
- Explicar decisiones importantes cuando sea necesario

---

## Expectativa sobre Gemini
- Proponer soluciones como lo haría un desarrollador senior
- Si algo no conviene hacerse todavía, indicarlo
- Justificar decisiones arquitectónicas
- No sobreingenierizar

---

## Contexto del repositorio
- Antes de proponer cambios o generar código, Gemini debe considerar la estructura completa del proyecto y los lineamientos definidos en este archivo
