// AccesoDatos/CanchaDatos.cs
using System;
using System.Collections.Generic;
using ComplejoDeportivoGalilea.Modelos;
using MySqlConnector;

namespace ComplejoDeportivoGalilea.AccesoDatos;

public class CanchaDatos
{
    public void Registrar(Cancha cancha)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = @"INSERT INTO Cancha (Nombre, TipoPasto, TarifaPorHora, Estado)
                           VALUES (@Nombre, @TipoPasto, @TarifaPorHora, @Estado);
                           SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Nombre", cancha.Nombre.Trim());
            cmd.Parameters.AddWithValue("@TipoPasto", cancha.TipoPasto.Trim());
            cmd.Parameters.AddWithValue("@TarifaPorHora", cancha.TarifaPorHora);
            cmd.Parameters.AddWithValue("@Estado", cancha.Estado.Trim());

            cancha.IdCancha = Convert.ToInt32(cmd.ExecuteScalar());
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al registrar cancha: {ex.Message}", ex);
        }
    }

    public bool Actualizar(Cancha cancha)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = @"UPDATE Cancha
                           SET Nombre = @Nombre,
                               TipoPasto = @TipoPasto,
                               TarifaPorHora = @TarifaPorHora,
                               Estado = @Estado
                           WHERE IdCancha = @IdCancha;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdCancha", cancha.IdCancha);
            cmd.Parameters.AddWithValue("@Nombre", cancha.Nombre.Trim());
            cmd.Parameters.AddWithValue("@TipoPasto", cancha.TipoPasto.Trim());
            cmd.Parameters.AddWithValue("@TarifaPorHora", cancha.TarifaPorHora);
            cmd.Parameters.AddWithValue("@Estado", cancha.Estado.Trim());

            return cmd.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al actualizar cancha: {ex.Message}", ex);
        }
    }

    public List<Cancha> Listar()
    {
        var lista = new List<Cancha>();
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = "SELECT IdCancha, Nombre, TipoPasto, TarifaPorHora, Estado FROM Cancha ORDER BY IdCancha;";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Cancha
                {
                    IdCancha = reader.GetInt32("IdCancha"),
                    Nombre = reader.GetString("Nombre"),
                    TipoPasto = reader.GetString("TipoPasto"),
                    TarifaPorHora = reader.GetDecimal("TarifaPorHora"),
                    Estado = reader.GetString("Estado")
                });
            }
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al listar canchas: {ex.Message}", ex);
        }

        return lista;
    }

    public Cancha? BuscarPorId(int idCancha)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = "SELECT IdCancha, Nombre, TipoPasto, TarifaPorHora, Estado FROM Cancha WHERE IdCancha = @IdCancha LIMIT 1;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdCancha", idCancha);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Cancha
                {
                    IdCancha = reader.GetInt32("IdCancha"),
                    Nombre = reader.GetString("Nombre"),
                    TipoPasto = reader.GetString("TipoPasto"),
                    TarifaPorHora = reader.GetDecimal("TarifaPorHora"),
                    Estado = reader.GetString("Estado")
                };
            }
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al buscar cancha por ID: {ex.Message}", ex);
        }

        return null;
    }
}