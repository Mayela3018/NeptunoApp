USE NeptunoDB;
GO

-- AGREGAR CAMPO ACTIVO A LAS 4 TABLAS


-- 1. Tabla Productos
IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('Productos') 
               AND name = 'Activo')
BEGIN
    ALTER TABLE dbo.Productos ADD Activo BIT NOT NULL DEFAULT 1;
    PRINT 'Campo Activo agregado a Productos';
END
GO

-- 2. Tabla Categorias
IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('Categorias') 
               AND name = 'Activo')
BEGIN
    ALTER TABLE dbo.Categorias ADD Activo BIT NOT NULL DEFAULT 1;
    PRINT 'Campo Activo agregado a Categorias';
END
GO

-- 3. Tabla Proveedores
IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('Proveedores') 
               AND name = 'Activo')
BEGIN
    ALTER TABLE dbo.Proveedores ADD Activo BIT NOT NULL DEFAULT 1;
    PRINT 'Campo Activo agregado a Proveedores';
END
GO

-- 4. Tabla Pedidos
IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('Pedidos') 
               AND name = 'Activo')
BEGIN
    ALTER TABLE dbo.Pedidos ADD Activo BIT NOT NULL DEFAULT 1;
    PRINT 'Campo Activo agregado a Pedidos';
END
GO
