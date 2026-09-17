USE NeptunoDB;
GO

-- =============================================
-- 1. CRUD DE PRODUCTOS (con eliminación lógica)
-- =============================================
IF OBJECT_ID('SP_Productos_Crud', 'P') IS NOT NULL
    DROP PROCEDURE SP_Productos_Crud;
GO

CREATE PROCEDURE SP_Productos_Crud
    @Opcion CHAR(1),
    @ProductoID INT = NULL,
    @NombreProducto NVARCHAR(60) = NULL,
    @ProveedorID INT = NULL,
    @CategoriaID INT = NULL,
    @CantidadPorUnidad NVARCHAR(30) = NULL,
    @PrecioUnidad DECIMAL(10,2) = NULL,
    @UnidadesEnExistencia SMALLINT = NULL,
    @UnidadesEnPedido SMALLINT = NULL,
    @NivelDeReorden SMALLINT = NULL,
    @Descontinuado BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Opcion = 'C'
    BEGIN
        INSERT INTO Productos (NombreProducto, ProveedorID, CategoriaID, 
                               CantidadPorUnidad, PrecioUnidad, UnidadesEnExistencia, 
                               UnidadesEnPedido, NivelDeReorden, Descontinuado, Activo)
        VALUES (@NombreProducto, @ProveedorID, @CategoriaID, @CantidadPorUnidad,
                @PrecioUnidad, @UnidadesEnExistencia, @UnidadesEnPedido,
                @NivelDeReorden, @Descontinuado, 1)
    END
    ELSE IF @Opcion = 'R'
    BEGIN
        IF @ProductoID IS NULL
            SELECT p.*, c.NombreCategoria, pr.CompaniaNombre
            FROM Productos p
            LEFT JOIN Categorias c ON p.CategoriaID = c.CategoriaID
            LEFT JOIN Proveedores pr ON p.ProveedorID = pr.ProveedorID
            WHERE p.Activo = 1
            ORDER BY p.NombreProducto
        ELSE
            SELECT p.*, c.NombreCategoria, pr.CompaniaNombre
            FROM Productos p
            LEFT JOIN Categorias c ON p.CategoriaID = c.CategoriaID
            LEFT JOIN Proveedores pr ON p.ProveedorID = pr.ProveedorID
            WHERE p.Activo = 1 AND p.ProductoID = @ProductoID
    END
    ELSE IF @Opcion = 'U'
    BEGIN
        UPDATE Productos 
        SET NombreProducto = @NombreProducto,
            ProveedorID = @ProveedorID,
            CategoriaID = @CategoriaID,
            CantidadPorUnidad = @CantidadPorUnidad,
            PrecioUnidad = @PrecioUnidad,
            UnidadesEnExistencia = @UnidadesEnExistencia,
            UnidadesEnPedido = @UnidadesEnPedido,
            NivelDeReorden = @NivelDeReorden,
            Descontinuado = @Descontinuado
        WHERE ProductoID = @ProductoID AND Activo = 1
    END
    ELSE IF @Opcion = 'D'
    BEGIN
        -- ELIMINACIÓN LÓGICA: Solo marca como inactivo
        UPDATE Productos SET Activo = 0 WHERE ProductoID = @ProductoID
    END
END
GO

-- =============================================
-- 2. CRUD DE CATEGORÍAS (con eliminación lógica)
-- =============================================
IF OBJECT_ID('SP_Categorias_Crud', 'P') IS NOT NULL
    DROP PROCEDURE SP_Categorias_Crud;
GO

CREATE PROCEDURE SP_Categorias_Crud
    @Opcion CHAR(1),
    @CategoriaID INT = NULL,
    @NombreCategoria NVARCHAR(30) = NULL,
    @Descripcion NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Opcion = 'C'
        INSERT INTO Categorias (NombreCategoria, Descripcion, Activo)
        VALUES (@NombreCategoria, @Descripcion, 1)
    ELSE IF @Opcion = 'R'
    BEGIN
        IF @CategoriaID IS NULL
            SELECT * FROM Categorias WHERE Activo = 1 ORDER BY NombreCategoria
        ELSE
            SELECT * FROM Categorias WHERE Activo = 1 AND CategoriaID = @CategoriaID
    END
    ELSE IF @Opcion = 'U'
        UPDATE Categorias 
        SET NombreCategoria = @NombreCategoria, Descripcion = @Descripcion
        WHERE CategoriaID = @CategoriaID AND Activo = 1
    ELSE IF @Opcion = 'D'
    BEGIN
        -- ELIMINACIÓN LÓGICA
        UPDATE Categorias SET Activo = 0 WHERE CategoriaID = @CategoriaID
    END
END
GO

-- =============================================
-- 3. CRUD DE PROVEEDORES (con eliminación lógica)
-- =============================================
IF OBJECT_ID('SP_Proveedores_Crud', 'P') IS NOT NULL
    DROP PROCEDURE SP_Proveedores_Crud;
GO

CREATE PROCEDURE SP_Proveedores_Crud
    @Opcion CHAR(1),
    @ProveedorID INT = NULL,
    @CompaniaNombre NVARCHAR(60) = NULL,
    @NombreContacto NVARCHAR(40) = NULL,
    @CargoContacto NVARCHAR(40) = NULL,
    @Direccion NVARCHAR(80) = NULL,
    @Ciudad NVARCHAR(30) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
    @Pais NVARCHAR(30) = NULL,
    @Telefono NVARCHAR(24) = NULL,
    @Fax NVARCHAR(24) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Opcion = 'C'
        INSERT INTO Proveedores (CompaniaNombre, NombreContacto, CargoContacto, 
                                 Direccion, Ciudad, CodigoPostal, Pais, Telefono, Fax, Activo)
        VALUES (@CompaniaNombre, @NombreContacto, @CargoContacto, @Direccion,
                @Ciudad, @CodigoPostal, @Pais, @Telefono, @Fax, 1)
    ELSE IF @Opcion = 'R'
    BEGIN
        IF @ProveedorID IS NULL
            SELECT * FROM Proveedores WHERE Activo = 1 ORDER BY CompaniaNombre
        ELSE
            SELECT * FROM Proveedores WHERE Activo = 1 AND ProveedorID = @ProveedorID
    END
    ELSE IF @Opcion = 'U'
        UPDATE Proveedores
        SET CompaniaNombre = @CompaniaNombre, NombreContacto = @NombreContacto,
            CargoContacto = @CargoContacto, Direccion = @Direccion,
            Ciudad = @Ciudad, CodigoPostal = @CodigoPostal, Pais = @Pais,
            Telefono = @Telefono, Fax = @Fax
        WHERE ProveedorID = @ProveedorID AND Activo = 1
    ELSE IF @Opcion = 'D'
    BEGIN
        -- ELIMINACIÓN LÓGICA
        UPDATE Proveedores SET Activo = 0 WHERE ProveedorID = @ProveedorID
    END
END
GO

-- =============================================
-- 4. CRUD DE PEDIDOS (con eliminación lógica)
-- =============================================
IF OBJECT_ID('SP_Pedidos_Crud', 'P') IS NOT NULL
    DROP PROCEDURE SP_Pedidos_Crud;
GO

CREATE PROCEDURE SP_Pedidos_Crud
    @Opcion CHAR(1),
    @PedidoID INT = NULL,
    @ClienteID INT = NULL,
    @EmpleadoID INT = NULL,
    @FechaPedido DATE = NULL,
    @FechaRequerida DATE = NULL,
    @FechaEnvio DATE = NULL,
    @TransportistaID INT = NULL,
    @Destinatario NVARCHAR(60) = NULL,
    @CiudadDestino NVARCHAR(30) = NULL,
    @PaisDestino NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Opcion = 'C'
    BEGIN
        INSERT INTO Pedidos (ClienteID, EmpleadoID, FechaPedido, FechaRequerida,
                            FechaEnvio, TransportistaID, Destinatario, 
                            CiudadDestino, PaisDestino, Activo)
        VALUES (@ClienteID, @EmpleadoID, @FechaPedido, @FechaRequerida,
                @FechaEnvio, @TransportistaID, @Destinatario, 
                @CiudadDestino, @PaisDestino, 1)
        SELECT SCOPE_IDENTITY() AS PedidoID
    END
    ELSE IF @Opcion = 'R'
    BEGIN
        IF @PedidoID IS NULL
            SELECT p.*, c.Empresa, 
                   e.Nombre + ' ' + e.Apellidos AS EmpleadoNombre,
                   t.CompaniaNombre AS Transportista
            FROM Pedidos p
            LEFT JOIN Clientes c ON p.ClienteID = c.ClienteID
            LEFT JOIN Empleados e ON p.EmpleadoID = e.EmpleadoID
            LEFT JOIN Transportistas t ON p.TransportistaID = t.TransportistaID
            WHERE p.Activo = 1
            ORDER BY p.FechaPedido DESC
        ELSE
            SELECT p.*, c.Empresa,
                   e.Nombre + ' ' + e.Apellidos AS EmpleadoNombre,
                   t.CompaniaNombre AS Transportista
            FROM Pedidos p
            LEFT JOIN Clientes c ON p.ClienteID = c.ClienteID
            LEFT JOIN Empleados e ON p.EmpleadoID = e.EmpleadoID
            LEFT JOIN Transportistas t ON p.TransportistaID = t.TransportistaID
            WHERE p.Activo = 1 AND p.PedidoID = @PedidoID
    END
    ELSE IF @Opcion = 'U'
        UPDATE Pedidos
        SET ClienteID = @ClienteID, EmpleadoID = @EmpleadoID,
            FechaPedido = @FechaPedido, FechaRequerida = @FechaRequerida,
            FechaEnvio = @FechaEnvio, TransportistaID = @TransportistaID,
            Destinatario = @Destinatario, CiudadDestino = @CiudadDestino,
            PaisDestino = @PaisDestino
        WHERE PedidoID = @PedidoID AND Activo = 1
    ELSE IF @Opcion = 'D'
    BEGIN
        -- ELIMINACIÓN LÓGICA
        UPDATE Pedidos SET Activo = 0 WHERE PedidoID = @PedidoID
    END
END
GO

-- =============================================
-- 5. LISTADO DE PROVEEDORES CON FILTROS (solo activos)
-- =============================================
IF OBJECT_ID('SP_Proveedores_Buscar', 'P') IS NOT NULL
    DROP PROCEDURE SP_Proveedores_Buscar;
GO

CREATE PROCEDURE SP_Proveedores_Buscar
    @NombreContacto NVARCHAR(40) = NULL,
    @Ciudad NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM Proveedores
    WHERE Activo = 1
      AND (@NombreContacto IS NULL OR NombreContacto LIKE '%' + @NombreContacto + '%')
      AND (@Ciudad IS NULL OR Ciudad LIKE '%' + @Ciudad + '%')
    ORDER BY CompaniaNombre
END
GO

-- =============================================
-- 6. LISTADO DE DETALLES DE PEDIDOS POR FECHAS (solo activos)
-- =============================================
IF OBJECT_ID('SP_DetallePedidos_PorFechas', 'P') IS NOT NULL
    DROP PROCEDURE SP_DetallePedidos_PorFechas;
GO

CREATE PROCEDURE SP_DetallePedidos_PorFechas
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT dp.PedidoID, dp.ProductoID, dp.PrecioUnidad, dp.Cantidad, dp.Descuento,
           p.NombreProducto, 
           ped.FechaPedido, 
           c.Empresa AS Cliente,
           (dp.PrecioUnidad * dp.Cantidad * (1 - dp.Descuento)) AS Total
    FROM DetallePedidos dp
    INNER JOIN Productos p ON dp.ProductoID = p.ProductoID
    INNER JOIN Pedidos ped ON dp.PedidoID = ped.PedidoID
    INNER JOIN Clientes c ON ped.ClienteID = c.ClienteID
    WHERE ped.Activo = 1
      AND ped.FechaPedido BETWEEN @FechaInicio AND @FechaFin
    ORDER BY ped.FechaPedido, dp.PedidoID
END
GO
