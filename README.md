# ReservasApp - Backend API 🍽️
Este repositorio contiene el Backend para la plataforma ReservasApp, un sistema de gestión de reservas para restaurantes que conecta a comensales, dueños de negocios y administradores. El sistema gestiona el flujo completo desde el registro del restaurante, su aprobación administrativa, la configuración de mesas y la asignación inteligente de reservas.

# 1. Video Demostrativo 🎥
Instrucciones para el profesor: Haga clic en la imagen a continuación para ver la demostración del funcionamiento de los endpoints principales y la lógica algorítmica.

[![Video Demo](https://i9.ytimg.com/vi/mtgIDBOWozs/mqdefault.jpg?sqp=CNz688kG-oaymwEmCMACELQB8quKqQMa8AEB-AH-CYAC0AWKAgwIABABGGUgZShlMA8%3D&rs=AOn4CLAvB8M0mWEMlTdhmZd6GDY9ahZ15w&retry=3)](https://youtu.be/mtgIDBOWozs)


# 2. Descripción del Proyecto 📄
El propósito de este proyecto es proveer una API RESTful robusta y segura para automatizar la gestión de reservas. Soluciona el problema de la asignación manual de mesas mediante un algoritmo de "Mejor Ajuste" (Best Fit) y asegura que solo restaurantes verificados puedan operar en la plataforma.

### Principales Funcionalidades:
* **Seguridad y Roles**: Autenticación vía JWT con roles diferenciados (Super Admin, Restaurante, Cliente).
* **Flujo de Aprobación**: Los restaurantes se registran pero no pueden operar hasta ser aprobados por un Administrador.
* **Gestión de Inventario**: Los restaurantes pueden crear y gestionar su inventario de mesas.
* **Algoritmo de Asignación**: Al aceptar una reserva, el sistema asigna automáticamente la mesa más adecuada según la capacidad requerida y la disponibilidad, optimizando el espacio.
* **Panel Administrativo**: Control total para aprobar, pausar o eliminar restaurantes del sistema.

# 3. Endpoints Implementados Destacados 🚀
A continuación, se describen 5 endpoints clave que demuestran la complejidad técnica del proyecto:

### `POST /api/auth/registro-restaurante`

* **Relevancia**: Inicia el flujo de negocio creando simultáneamente un usuario dueño y una ficha de restaurante en estado "Pendiente", disparando la solicitud de aprobación.

### `GET /api/admin/solicitudes`

* **Relevancia**: Endpoint exclusivo para el Super Admin que filtra y proyecta los datos de los restaurantes que requieren atención inmediata.

### `POST /api/admin/aprobar/{id}`

* **Relevancia**: Ya que el usuario Admin sea ingresada se puede relizar la autorizacion del usuario restaurante.

### `POST /api/user/reservar`

* **Relevancia**: Maneja la transacción principal del cliente, validando la existencia del restaurante y creando el registro inicial vinculado al usuario.

### `PUT /api/restaurant/gestionar-reserva`

* **Relevancia**: Contiene la lógica principal. Si el dueño acepta una reserva sin elegir mesa manualmente, el sistema ejecuta un algoritmo para buscar la mesa libre con la capacidad mínima necesaria ("Best Fit") para la fecha y hora solicitada.

# 4. Instrucciones de Ejecución ⚙️
Sigue estos pasos para ejecutar el proyecto en tu entorno local.

### Requerimientos del Sistema
* .NET SDK 8.0
* Docker Desktop (Para la base de datos MySQL)
* Postman (Para pruebas)

### Paso 1: Clonar y Restaurar

```bash
git clone <URL_DEL_REPOSITORIO>
cd reservasApp
dotnet restore
```

### Paso 2: Configuración Inicial (Base de Datos)
El proyecto utiliza Docker para levantar la base de datos MySQL.
Asegúrate de que Docker Desktop esté corriendo.
Ejecuta el siguiente comando en la raíz del proyecto (donde está el docker-compose.yml):

```bash
docker-compose up -d
```

Esto levantará un contenedor MySQL con la contraseña configurada como YES.

### Paso 3: Aplicar Migraciones
Para crear las tablas en la base de datos, ejecuta:

```bash
dotnet ef database update
```

### Paso 4: Iniciar el Servidor

```bash
dotnet run
```

La API estará disponible en http://localhost:5160 (o el puerto indicado en tu consola).

### Consideraciones Especiales
* **Usuario Admin**: No existe registro público para Administradores. Se debe insertar manualmente en la base de datos o usar el script SQL incluido en la carpeta `docs/` (si aplica).
* **Cadena de Conexión**: La configuración ya está lista en `appsettings.json` para conectar con el contenedor Docker local:
  ```
  Server=localhost;Port=3306;Database=ReservAppDb;User=root;Password=YES;
  ```

# 5. Colección de Postman 📬
El archivo de exportación de la colección de Postman (`ReservasApp.postman_collection.json`) se encuentra ubicado en la raíz de este repositorio.

### Incluye ejemplos de:
* Registro y Login (con obtención de Token Bearer).
* Flujo completo de Admin (Ver solicitudes, Aprobar).
* Flujo de Restaurante (Crear mesas, Aceptar reservas algorítmicamente).
* Flujo de Usuario (Crear reserva).
