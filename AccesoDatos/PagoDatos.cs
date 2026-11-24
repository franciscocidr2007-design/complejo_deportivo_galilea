using System;
using System.Collections.Generic;
using ComplejoDeportivoGalilea.Modelos;
using MySqlConnector;

namespace ComplejoDeportivoGalilea.AccesoDatos;

public class PagoDatos
{
    public void Registrar(Pago pago)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = @"INSERT INTO Pago (IdReserva, Monto, FechaPago, EstadoPago, MedioPago)
                           VALUES (@IdReserva, @Monto, @FechaPago, @EstadoPago, @MedioPago);
                           SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdReserva", pago.IdReserva);
            cmd.Parameters.AddWithValue("@Monto", pago.Monto);
            cmd.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
            cmd.Parameters.AddWithValue("@EstadoPago", pago.EstadoPago.Trim());
            cmd.Parameters.AddWithValue("@MedioPago", pago.MedioPago.Trim());

            pago.IdPago = Convert.ToInt32(cmd.ExecuteScalar());
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al registrar pago: {ex.Message}", ex);
        }
    }

    public List<Pago> Listar()
    {
        var lista = new List<Pago>();
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = "SELECT IdPago, IdReserva, Monto, FechaPago, EstadoPago, MedioPago FROM Pago ORDER BY IdPago;";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Pago
                {
                    IdPago = reader.GetInt32("IdPago"),
                    IdReserva = reader.GetInt32("IdReserva"),
                    Monto = reader.GetDecimal("Monto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    EstadoPago = reader.GetString("EstadoPago"),
                    MedioPago = reader.GetString("MedioPago")
                });
            }
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al listar pagos: {ex.Message}", ex);
        }

        return lista;
    }

    public Pago? BuscarPorReserva(int idReserva)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = "SELECT IdPago, IdReserva, Monto, FechaPago, EstadoPago, MedioPago FROM Pago WHERE IdReserva = @IdReserva LIMIT 1;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdReserva", idReserva);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Pago
                {
                    IdPago = reader.GetInt32("IdPago"),
                    IdReserva = reader.GetInt32("IdReserva"),
                    Monto = reader.GetDecimal("Monto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    EstadoPago = reader.GetString("EstadoPago"),
                    MedioPago = reader.GetString("MedioPago")
                };
            }
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al buscar pago por reserva: {ex.Message}", ex);
        }

        return null;
    }
}