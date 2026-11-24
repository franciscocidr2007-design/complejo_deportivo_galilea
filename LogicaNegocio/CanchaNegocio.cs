// LogicaNegocio/CanchaNegocio.cs
using System.Collections.Generic;
using ComplejoDeportivoGalilea.AccesoDatos;
using ComplejoDeportivoGalilea.Modelos;

namespace ComplejoDeportivoGalilea.LogicaNegocio;

public class CanchaNegocio
{
    private readonly CanchaDatos _datos;

    public CanchaNegocio(CanchaDatos datos)
    {
        _datos = datos;
    }

    public string RegistrarCancha(Cancha cancha)
    {
        if (string.IsNullOrWhiteSpace(cancha.Nombre))
            return "El nombre de la cancha es obligatorio.";

        if (string.IsNullOrWhiteSpace(cancha.TipoPasto))
            return "El tipo de pasto es obligatorio.";

        if (cancha.TarifaPorHora <= 0)
            return "La tarifa por hora debe ser mayor a 0.";

        if (string.IsNullOrWhiteSpace(cancha.Estado))
            cancha.Estado = "Disponible";

        _datos.Registrar(cancha);
        return $"Cancha registrada correctamente (ID: {cancha.IdCancha}).";
    }

    public string ActualizarCancha(Cancha cancha)
    {
        if (cancha.IdCancha <= 0)
            return "ID de cancha invalido.";

        if (string.IsNullOrWhiteSpace(cancha.Nombre) || string.IsNullOrWhiteSpace(cancha.TipoPasto) || string.IsNullOrWhiteSpace(cancha.Estado))
            return "Todos los campos de la cancha son obligatorios.";

        if (cancha.TarifaPorHora <= 0)
            return "La tarifa por hora debe ser mayor a 0.";

        bool ok = _datos.Actualizar(cancha);
        return ok ? "Cancha actualizada correctamente." : "No se encontro la cancha indicada.";
    }

    public List<Cancha> ObtenerCanchas()
    {
        return _datos.Listar();
    }
}