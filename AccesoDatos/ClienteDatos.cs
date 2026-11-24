// AccesoDatos/ClienteDatos.cs
using System;
using System.Collections.Generic;
using ComplejoDeportivoGalilea.Modelos;
using MySqlConnector;

namespace ComplejoDeportivoGalilea.AccesoDatos;

public class ClienteDatos
{
    public void Registrar(Cliente cliente)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = @"INSERT INTO Cliente (Rut, Nombre, Telefono, Correo)
                           VALUES (@Rut, @Nombre, @Telefono, @Correo);
                           SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Rut", cliente.Rut.Trim());
            cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre.Trim());
            cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono.Trim());
            cmd.Parameters.AddWithValue("@Correo", cliente.Correo.Trim());

            cliente.IdCliente = Convert.ToInt32(cmd.ExecuteScalar());
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al registrar cliente en la base de datos: {ex.Message}", ex);
        }
    }

    public List<Cliente> Listar()
    {
        var lista = new List<Cliente>();
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = "SELECT IdCliente, Rut, Nombre, Telefono, Correo FROM Cliente ORDER BY IdCliente;";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Cliente
                {
                    IdCliente = reader.GetInt32("IdCliente"),
                    Rut = reader.GetString("Rut"),
                    Nombre = reader.GetString("Nombre"),
                    Telefono = reader.GetString("Telefono"),
                    Correo = reader.GetString("Correo")
                });
            }
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al listar clientes: {ex.Message}", ex);
        }

        return lista;
    }

    public Cliente? BuscarPorId(int idCliente)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = "SELECT IdCliente, Rut, Nombre, Telefono, Correo FROM Cliente WHERE IdCliente = @IdCliente LIMIT 1;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdCliente", idCliente);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Cliente
                {
                    IdCliente = reader.GetInt32("IdCliente"),
                    Rut = reader.GetString("Rut"),
                    Nombre = reader.GetString("Nombre"),
                    Telefono = reader.GetString("Telefono"),
                    Correo = reader.GetString("Correo")
                };
            }
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al buscar cliente por ID: {ex.Message}", ex);
        }

        return null;
    }

    public Cliente? BuscarPorRut(string rut)
    {
        try
        {
            using var conn = ConexionBd.ObtenerConexion();
            conn.Open();

            string sql = "SELECT IdCliente, Rut, Nombre, Telefono, Correo FROM Cliente WHERE LOWER(TRIM(Rut)) = LOWER(TRIM(@Rut)) LIMIT 1;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Rut", rut);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Cliente
                {
                    IdCliente = reader.GetInt32("IdCliente"),
                    Rut = reader.GetString("Rut"),
                    Nombre = reader.GetString("Nombre"),
                    Telefono = reader.GetString("Telefono"),
                    Correo = reader.GetString("Correo")
                };
            }
        }
        catch (MySqlException ex)
        {
            throw new Exception($"Error al buscar cliente por RUT: {ex.Message}", ex);
        }

        return null;
    }
}