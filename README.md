# Trabajo Práctico Integrador
## Desarrollo de Software
### Backend

#  Guía de uso de la API - Proyecto Dsw2025Tpi 

Esta guía explica cómo probar cada uno de los endpoints de la API desde Swagger.

---

##  Productos

###  POST /api/products
Crea un producto nuevo.

 Ejemplo de entrada (JSON):
json
{
  "sku": "SKU-123",
  "internalCode": "INT-456",
  "name": "Producto de Prueba",
  "description": "Este es un producto de ejemplo",
  "currentUnitPrice": 299.99,
  "stockQuantity": 100
}


 Devuelve el producto creado (con ID generado).

---

###  GET /api/products
Lista todos los productos activos.

 No requiere parámetros.

 Respuesta ejemplo:
json
[
  {
    "id": "c5a587f2-1c2a-4f22-90de-9fdab53be120",
    "sku": "SKU-123",
    "internalCode": "INT-456",
    "name": "Producto de Prueba",
    "description": "Este es un producto de ejemplo",
    "currentUnitPrice": 299.99,
    "stockQuantity": 100,
    "isActive": true
  }
]


---

###  GET /api/products/{id}
Busca un producto por ID.

 Parámetro de ruta:
- id: GUID del producto

 Devuelve el producto si existe, 404 si no.

---

###  PUT /api/products/{id}
Actualiza un producto existente.

 Ejemplo de entrada:
json
{
  "sku": "SKU-999",
  "internalCode": "INT-888",
  "name": "Producto Actualizado",
  "description": "Nueva descripción",
  "currentUnitPrice": 250.00,
  "stockQuantity": 80
}


---

###  PATCH /api/products/{id}
Inhabilita un producto (soft delete).

 Parámetro de ruta:
- id: GUID del producto

Devuelve 204 No Content si fue exitoso.

---

##  Órdenes

###  POST /api/orders
Crea una orden con uno o varios productos.

 Ejemplo de entrada:
json
{
  "customerId": "b3fc31ef-9d25-4e7e-9c7f-53f6f11e1234",
  "shippingAddress": "Calle Falsa 123",
  "billingAddress": "Calle Falsa 123",
  "notes": "Por favor entregar por la mañana",
  "orderItems": [
    {
      "productId": "c5a587f2-1c2a-4f22-90de-9fdab53be120",
      "quantity": 2
    }
  ]
}


 Devuelve la orden creada.

---

### 🛠 PATCH /api/orders/{id}/status
Actualiza el estado de una orden.

 Ejemplo de entrada:
json
{
  "newStatus": "Delivered"
}

 Estados posibles:
- Pending
- Processing
- Shipped
- Delivered
- Cancelled

 Devuelve 200 si fue exitoso, 400 si no es válido.

---

##  Tecnologías utilizadas

- .NET 8
- ASP.NET Core
- Entity Framework Core
- AutoMapper
- FluentValidation
- SQL Server
- Swagger / Swashbuckle
- Patrón Domain-Driven Design (DDD)

---

##  Integrantes

- Castillo, Lautaro - 50527
- Figueroa, Cesar Bernabé - 57150
- Svaldi, Luisina - 53873
