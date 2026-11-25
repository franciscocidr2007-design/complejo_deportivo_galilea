CREATE DATABASE IF NOT EXISTS `complejo_deportivo_galilea`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE `complejo_deportivo_galilea`;

CREATE TABLE `cliente` (
    `id_cliente` INT AUTO_INCREMENT PRIMARY KEY,
    `rut` VARCHAR(12) NOT NULL UNIQUE,
    `nombre` VARCHAR(100) NOT NULL,
    `telefono` VARCHAR(20) NOT NULL,
    `correo` VARCHAR(100) NOT NULL,
    CONSTRAINT `chk_cliente_nombre_len` CHECK (CHAR_LENGTH(TRIM(`nombre`)) >= 2)
);

CREATE TABLE `cancha` (
    `id_cancha` INT AUTO_INCREMENT PRIMARY KEY,
    `nombre` VARCHAR(50) NOT NULL UNIQUE,
    `tipo_pasto` VARCHAR(30) NOT NULL,
    `tarifa_por_hora` DECIMAL(10, 2) NOT NULL,
    `estado` VARCHAR(20) NOT NULL DEFAULT 'Disponible',
    CONSTRAINT `chk_cancha_tarifa` CHECK (`tarifa_por_hora` > 0),
    CONSTRAINT `chk_cancha_estado` CHECK (`estado` IN ('Disponible', 'Mantencion', 'Inactiva'))
);

CREATE TABLE `reserva` (
    `id_reserva` INT AUTO_INCREMENT PRIMARY KEY,
    `id_cliente` INT NOT NULL,
    `id_cancha` INT NOT NULL,
    `fecha` DATE NOT NULL,
    `hora_inicio` TIME NOT NULL,
    `hora_fin` TIME NOT NULL,
    `estado` VARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    CONSTRAINT `fk_reserva_cliente`
        FOREIGN KEY (`id_cliente`) REFERENCES `cliente` (`id_cliente`)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT `fk_reserva_cancha`
        FOREIGN KEY (`id_cancha`) REFERENCES `cancha` (`id_cancha`)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT `chk_reserva_horario` CHECK (`hora_fin` > `hora_inicio`),
    CONSTRAINT `chk_reserva_estado` CHECK (`estado` IN ('Pendiente', 'Confirmada', 'Cancelada', 'Completada'))
);

CREATE TABLE `pago` (
    `id_pago` INT AUTO_INCREMENT PRIMARY KEY,
    `id_reserva` INT NOT NULL UNIQUE,
    `monto` DECIMAL(10, 2) NOT NULL,
    `fecha_pago` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `estado_pago` VARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    `medio_pago` VARCHAR(30) NOT NULL,
    CONSTRAINT `fk_pago_reserva`
        FOREIGN KEY (`id_reserva`) REFERENCES `reserva` (`id_reserva`)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT `chk_pago_monto` CHECK (`monto` >= 0),
    CONSTRAINT `chk_pago_estado` CHECK (`estado_pago` IN ('Pendiente', 'Pagado', 'Anulado', 'Reembolsado')),
    CONSTRAINT `chk_pago_medio` CHECK (`medio_pago` IN ('Efectivo', 'Debito', 'Credito', 'Transferencia'))
);
