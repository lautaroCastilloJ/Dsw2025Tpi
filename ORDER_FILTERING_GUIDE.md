# ?? Sistema de Búsqueda y Filtrado de Órdenes - Guía Completa

## ?? Resumen de Cambios

Se ha mejorado el sistema de filtrado de órdenes para permitir búsquedas más específicas y flexibles mediante parámetros individuales o combinados.

---

## ? Archivos Modificados

### 1. **Application Layer**
- ? `FilterOrder.cs` - Agregado parámetro `OrderId`
- ? `OrderService.cs` - Mejorada lógica de filtrado con prioridad de parámetros

### 2. **API Layer**
- ? `OrdersController.cs` - Agregado parámetro `orderId` en endpoint `/my-orders`

---

## ?? Parámetros de Filtrado Disponibles

### Para **Clientes** (Endpoint: `GET /api/orders/my-orders`)

| Parámetro | Tipo | Descripción | Ejemplo |
|-----------|------|-------------|---------|
| `orderId` | `Guid?` | Buscar una orden específica por ID (exacta) | `?orderId=3fa85f64-5717-4562-b3fc-2c963f66afa6` |
| `status` | `string?` | Filtrar por estado de la orden | `?status=Pending` |
| `search` | `string?` | Búsqueda general (ID, direcciones, notas) | `?search=Calle%20Falsa` |
| `pageNumber` | `int?` | Número de página (default: 1) | `?pageNumber=2` |
| `pageSize` | `int?` | Tamaño de página (default: 10, max: 100) | `?pageSize=20` |

### Para **Administradores** (Endpoint: `GET /api/orders/admin`)

| Parámetro | Tipo | Descripción | Ejemplo |
|-----------|------|-------------|---------|
| `orderId` | `Guid?` | Buscar una orden específica por ID (exacta) | `?orderId=3fa85f64-5717-4562-b3fc-2c963f66afa6` |
| `customerId` | `Guid?` | Filtrar órdenes de un cliente específico (exacta) | `?customerId=7b8c9d0e-1234-5678-abcd-ef1234567890` |
| `customerName` | `string?` | Buscar por nombre de cliente (parcial) | `?customerName=Juan` |
| `status` | `string?` | Filtrar por estado de la orden | `?status=Shipped` |
| `search` | `string?` | Búsqueda general (ID, cliente, direcciones, notas) | `?search=Buenos%20Aires` |
| `pageNumber` | `int?` | Número de página (default: 1) | `?pageNumber=1` |
| `pageSize` | `int?` | Tamaño de página (default: 10, max: 100) | `?pageSize=50` |

---

## ?? Estados de Orden Válidos

Los siguientes estados están disponibles para filtrado:

- `Pending` - Pendiente
- `Processing` - En proceso
- `Shipped` - Enviada
- `Delivered` - Entregada
- `Cancelled` - Cancelada

---

## ?? Ejemplos de Uso

### **Escenario 1: Cliente busca una orden específica por ID**

```http
GET /api/orders/my-orders?orderId=3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer {token-cliente}
```

**Respuesta:**
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-01-15T10:30:00",
      "status": "Shipped",
      "totalAmount": 1299.99,
      "customerName": "Juan Pérez"
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

---

### **Escenario 2: Cliente busca sus órdenes por estado**

```http
GET /api/orders/my-orders?status=Pending&pageSize=5
Authorization: Bearer {token-cliente}
```

**Casos de uso:**
- Ver todas mis órdenes pendientes
- Ver órdenes entregadas
- Ver órdenes canceladas

---

### **Escenario 3: Cliente busca por dirección de envío**

```http
GET /api/orders/my-orders?search=Calle Falsa 123
Authorization: Bearer {token-cliente}
```

**El término de búsqueda se aplica a:**
- ID de la orden (parcial)
- Dirección de envío
- Dirección de facturación
- Notas de la orden

---

### **Escenario 4: Admin busca órdenes de un cliente específico**

```http
GET /api/orders/admin?customerId=7b8c9d0e-1234-5678-abcd-ef1234567890
Authorization: Bearer {token-admin}
```

**Respuesta:** Todas las órdenes del cliente especificado

---

### **Escenario 5: Admin busca órdenes por nombre de cliente**

```http
GET /api/orders/admin?customerName=Juan
Authorization: Bearer {token-admin}
```

**Respuesta:** Todas las órdenes de clientes cuyo nombre contenga "Juan"

---

### **Escenario 6: Admin busca una orden específica por ID**

```http
GET /api/orders/admin?orderId=3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer {token-admin}
```

**Respuesta:** La orden con ese ID específico

---

### **Escenario 7: Búsqueda general del admin**

```http
GET /api/orders/admin?search=Buenos Aires
Authorization: Bearer {token-admin}
```

**El término de búsqueda se aplica a:**
- ID de la orden
- Nombre del cliente
- Dirección de envío
- Dirección de facturación
- Notas de la orden

---

### **Escenario 8: Filtros combinados**

```http
GET /api/orders/admin?customerId=7b8c9d0e-1234-5678-abcd-ef1234567890&status=Shipped&pageSize=20
Authorization: Bearer {token-admin}
```

**Respuesta:** Órdenes enviadas de un cliente específico, con 20 resultados por página

---

## ?? Lógica de Prioridad de Filtros

La búsqueda sigue una **lógica de prioridad** para optimizar el rendimiento:

### 1?? **Filtros Exactos (Mayor prioridad)**
- `orderId` - Búsqueda exacta por ID de orden
- `customerId` - Búsqueda exacta por ID de cliente

### 2?? **Filtros de Estado**
- `status` - Filtra por estado de la orden

### 3?? **Filtros Parciales**
- `customerName` - Búsqueda parcial en nombre de cliente (LIKE)

### 4?? **Búsqueda General (Menor prioridad)**
- `search` - Se ignora si ya se usó `orderId`

### Ejemplo de Prioridad:
```http
# Este request busca la orden específica, ignora el parámetro search
GET /api/orders/admin?orderId=123&search=test
```

---

## ?? Seguridad y Permisos

### **Clientes (`/api/orders/my-orders`)**
? Solo ven sus propias órdenes  
? El `customerId` se toma automáticamente del token JWT  
? No pueden filtrar por `customerId` ni `customerName`  

### **Administradores (`/api/orders/admin`)**
? Ven todas las órdenes del sistema  
? Pueden filtrar por cualquier combinación de parámetros  
? Pueden buscar órdenes de cualquier cliente  

---

## ?? Validaciones

### Parámetros de Paginación
- `pageNumber` debe ser > 0 (default: 1)
- `pageSize` debe ser > 0 y ? 100 (default: 10)

### Estados
- El `status` debe ser uno de los valores válidos (case-insensitive)
- Valores inválidos retornan error 400

### Búsqueda
- `customerName` máximo 100 caracteres
- `search` máximo 250 caracteres

---

## ?? Testing con Swagger

### 1. Cliente buscando sus órdenes
```
GET /api/orders/my-orders
Headers:
  Authorization: Bearer {token-cliente}
Query Parameters:
  orderId: (opcional)
  status: Pending
  search: (opcional)
  pageNumber: 1
  pageSize: 10
```

### 2. Admin buscando por cliente
```
GET /api/orders/admin
Headers:
  Authorization: Bearer {token-admin}
Query Parameters:
  customerId: 7b8c9d0e-1234-5678-abcd-ef1234567890
  status: (opcional)
  pageNumber: 1
  pageSize: 20
```

### 3. Admin buscando por nombre
```
GET /api/orders/admin
Headers:
  Authorization: Bearer {token-admin}
Query Parameters:
  customerName: Juan
  status: Delivered
  pageNumber: 1
  pageSize: 10
```

---

## ?? Respuesta del API

Todas las búsquedas retornan el mismo formato paginado:

```json
{
  "items": [
    {
      "id": "guid",
      "date": "2024-01-15T10:30:00",
      "status": "Shipped",
      "totalAmount": 1299.99,
      "customerName": "Juan Pérez"
    }
  ],
  "totalCount": 42,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

## ?? Casos de Uso Comunes

### Para Clientes:

1. **Ver todas mis órdenes**: `GET /my-orders`
2. **Ver mis órdenes pendientes**: `GET /my-orders?status=Pending`
3. **Buscar mi orden por ID**: `GET /my-orders?orderId={guid}`
4. **Buscar por dirección**: `GET /my-orders?search=mi direccion`

### Para Administradores:

1. **Ver todas las órdenes**: `GET /admin`
2. **Ver órdenes de un cliente**: `GET /admin?customerId={guid}`
3. **Buscar clientes por nombre**: `GET /admin?customerName=Juan`
4. **Ver órdenes por estado**: `GET /admin?status=Processing`
5. **Buscar una orden específica**: `GET /admin?orderId={guid}`
6. **Búsqueda combinada**: `GET /admin?customerName=Juan&status=Shipped`

---

## ? Checklist de Implementación

- [x] Agregado parámetro `OrderId` en `FilterOrder`
- [x] Actualizada lógica de filtrado en `OrderService`
- [x] Agregado parámetro `orderId` en endpoint `/my-orders`
- [x] Implementada lógica de prioridad de filtros
- [x] Validaciones existentes mantienen compatibilidad
- [x] Compilación exitosa
- [x] Documentación completa

---

## ?? ¡Listo para Usar!

El sistema de búsqueda y filtrado está completamente implementado y listo para usar. Los usuarios ahora pueden:

? Buscar órdenes por ID exacto  
? Filtrar por cliente específico (ID o nombre)  
? Filtrar por estado  
? Realizar búsquedas generales  
? Combinar múltiples filtros  
? Usar paginación eficiente  

**Nota:** Los filtros son opcionales y pueden combinarse según las necesidades del usuario.
