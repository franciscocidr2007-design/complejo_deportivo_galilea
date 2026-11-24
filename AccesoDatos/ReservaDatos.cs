using System;
using System.Collections.Generic;
using ComplejoDeportivoGalilea.Modelos;
using MySqlConnector;

namespace ComplejoDeportivoGalilea.AccesoDatos;

public class ReservaDatos
{
    public void Registrar(Reserva reserva)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = @"INSERT INTO Reserva (IdCliente, IdCancha, Fecha, HoraInicio, HoraFin, Estado)
                           VALUES (@IdCliente, @IdCancha, @Fecha, @HoraInicio, @HoraFin, @Estado);
                           SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdCliente", reserva.IdCliente);
            cmd.Parameters.AddWithValue("@IdCancha", reserva.IdCancha);
            cmd.Parameters.AddWithValue("@Fecha", reserva.Fecha.Date);
            cmd.Parameters.AddWithValue("@HoraInicio", reserva.HoraInicio);
            cmd.Parameters.AddWithValue("@HoraFin", reserva.HoraFin);
            cmd.Parameters.AddWithValue("@Estado", reserva.Estado.Trim());

            reserva.IdReserva = Convert.ToInt32(cmd.ExecuteScalar());
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al registrar reserva: {ex.Message}", ex);
        }
    }

    public bool Actualizar(Reserva reserva)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = @"UPDATE Reserva
                           SET IdCliente = @IdCliente,
                               IdCancha = @IdCancha,
                               Fecha = @Fecha,
                               HoraInicio = @HoraInicio,
                               HoraFin = @HoraFin,
                               Estado = @Estado
                           WHERE IdReserva = @IdReserva;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdReserva", reserva.IdReserva);
            cmd.Parameters.AddWithValue("@IdCliente", reserva.IdCliente);
            cmd.Parameters.AddWithValue("@IdCancha", reserva.IdCancha);
            cmd.Parameters.AddWithValue("@Fecha", reserva.Fecha.Date);
            cmd.Parameters.AddWithValue("@HoraInicio", reserva.HoraInicio);
            cmd.Parameters.AddWithValue("@HoraFin", reserva.HoraFin);
            cmd.Parameters.AddWithValue("@Estado", reserva.Estado.Trim());

            return cmd.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al actualizar reserva: {ex.Message}", ex);
        }
    }

    public bool Cancelar(int idReserva)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = @"UPDATE Reserva
                           SET Estado = 'Cancelada'
                           WHERE IdReserva = @IdReserva;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdReserva", idReserva);

            return cmd.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al cancelar reserva: {ex.Message}", ex);
        }
    }

    public bool ExisteSuperposicion(int idCancha, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin, int? excluirIdReserva = null)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = @"SELECT COUNT(*)
                           FROM Reserva
                           WHERE IdCancha = @IdCancha
                             AND Fecha = @Fecha
                             AND LOWER(Estado) <> 'cancelada'
                             AND (@ExcluirId IS NULL OR IdReserva <> @ExcluirId)
                             AND @HoraInicio < HoraFin
                             AND @HoraFin > HoraInicio;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdCancha", idCancha);
            cmd.Parameters.AddWithValue("@Fecha", fecha.Date);
            cmd.Parameters.AddWithValue("@HoraInicio", horaInicio);
            cmd.Parameters.AddWithValue("@HoraFin", horaFin);
            cmd.Parameters.AddWithValue("@ExcluirId", excluirIdReserva.HasValue ? excluirIdReserva.Value : DBNull.Value);

            long total = Convert.ToInt64(cmd.ExecuteScalar());
            return total > 0;
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al verificar superposición de reserva: {ex.Message}", ex);
        }
    }

    public List<Reserva> Listar()
    {
        var lista = new List<Reserva>();
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = "SELECT IdReserva, IdCliente, IdCancha, Fecha, HoraInicio, HoraFin, Estado FROM Reserva ORDER BY Fecha, HoraInicio;";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Reserva
                {
                    IdReserva = reader.GetInt32("IdReserva"),
                    IdCliente = reader.GetInt32("IdCliente"),
                    IdCancha = reader.GetInt32("IdCancha"),
                    Fecha = reader.GetDateTime("Fecha"),
                    HoraInicio = reader.GetTimeSpan("HoraInicio"),
                    HoraFin = reader.GetTimeSpan("HoraFin"),
                    Estado = reader.GetString("Estado")
                });
            }
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al listar reservas: {ex.Message}", ex);
        }

        return lista;
    }

    public Reserva? BuscarPorId(int idReserva)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = "SELECT IdReserva, IdCliente, IdCancha, Fecha, HoraInicio, HoraFin, Estado FROM Reserva WHERE IdReserva = @IdReserva LIMIT 1;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdReserva", idReserva);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Reserva
                {
                    IdReserva = reader.GetInt32("IdReserva"),
                    IdCliente = reader.GetInt32("IdCliente"),
                    IdCancha = reader.GetInt32("IdCancha"),
                    Fecha = reader.GetDateTime("Fecha"),
                    HoraInicio = reader.GetTimeSpan("HoraInicio"),
                    HoraFin = reader.GetTimeSpan("HoraFin"),
                    Estado = reader.GetString("Estado")
                };
            }
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al buscar reserva por ID: {ex.Message}", ex);
        }

        return null;
    }
}