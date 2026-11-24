// LogicaNegocio/ClienteNegocio.cs
using System.Collections.Generic;
using ComplejoDeportivoGalilea.AccesoDatos;
using ComplejoDeportivoGalilea.Modelos;

namespace ComplejoDeportivoGalilea.LogicaNegocio;

public class ClienteNegocio
{
    private readonly ClienteDatos _datos;

    public ClienteNegocio(ClienteDatos datos)
    {
        _datos = datos;
    }

    public string RegistrarCliente(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Rut))
            return "El RUT es obligatorio.";

        if (string.IsNullOrWhiteSpace(cliente.Nombre))
            return "El nombre es obligatorio.";

        if (string.IsNullOrWhiteSpace(cliente.Telefono))
            return "El telefono es obligatorio.";

        if (string.IsNullOrWhiteSpace(cliente.Correo))
            return "El correo es obligatorio.";

        if (_datos.BuscarPorRut(cliente.Rut) != null)
            return "Ya existe un cliente registrado con ese RUT.";

        _datos.Registrar(cliente);
        return $"Cliente registrado correctamente (ID: {cliente.IdCliente}).";
    }

    public List<Cliente> ObtenerClientes()
    {
        return _datos.Listar();
    }
}