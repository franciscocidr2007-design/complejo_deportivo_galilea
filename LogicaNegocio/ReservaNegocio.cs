// LogicaNegocio/ReservaNegocio.cs
using System;
using System.Collections.Generic;
using ComplejoDeportivoGalilea.AccesoDatos;
using ComplejoDeportivoGalilea.Modelos;

namespace ComplejoDeportivoGalilea.LogicaNegocio;

public class ReservaNegocio
{
    private readonly ReservaDatos _reservaDatos;
    private readonly ClienteDatos _clienteDatos;
    private readonly CanchaDatos _canchaDatos;

    public ReservaNegocio(ReservaDatos reservaDatos, ClienteDatos clienteDatos, CanchaDatos canchaDatos)
    {
        _reservaDatos = reservaDatos;
        _clienteDatos = clienteDatos;
        _canchaDatos = canchaDatos;
    }

    public string CrearReserva(Reserva reserva)
    {
        if (_clienteDatos.BuscarPorId(reserva.IdCliente) == null)
            return "El cliente indicado no existe.";

        var cancha = _canchaDatos.BuscarPorId(reserva.IdCancha);
        if (cancha == null)
            return "La cancha indicada no existe.";

        if (!string.Equals(cancha.Estado, "Disponible", StringComparison.OrdinalIgnoreCase))
            return $"La cancha no esta disponible (Estado actual: {cancha.Estado}).";

        if (reserva.HoraFin <= reserva.HoraInicio)
            return "La hora de fin debe ser mayor que la hora de inicio.";

        if (_reservaDatos.ExisteSuperposicion(reserva.IdCancha, reserva.Fecha, reserva.HoraInicio, reserva.HoraFin))
            return "Ya existe una reserva activa en ese horario para la cancha seleccionada.";

        reserva.Estado = "Confirmada";
        _reservaDatos.Registrar(reserva);
        return $"Reserva #{reserva.IdReserva} creada correctamente.";
    }

    public string ModificarReserva(int idReserva, int nuevaCanchaId, DateTime nuevaFecha, TimeSpan nuevaHoraInicio, TimeSpan nuevaHoraFin)
    {
        var reserva = _reservaDatos.BuscarPorId(idReserva);
        if (reserva == null)
            return "La reserva indicada no existe.";

        if (string.Equals(reserva.Estado, "Cancelada", StringComparison.OrdinalIgnoreCase))
            return "No se puede modificar una reserva cancelada.";

        var cancha = _canchaDatos.BuscarPorId(nuevaCanchaId);
        if (cancha == null)
            return "La cancha indicada no existe.";

        if (nuevaHoraFin <= nuevaHoraInicio)
            return "La hora de fin debe ser mayor que la hora de inicio.";

        if (_reservaDatos.ExisteSuperposicion(nuevaCanchaId, nuevaFecha, nuevaHoraInicio, nuevaHoraFin, idReserva))
            return "No hay disponibilidad en ese nuevo horario o cancha.";

        reserva.IdCancha = nuevaCanchaId;
        reserva.Fecha = nuevaFecha;
        reserva.HoraInicio = nuevaHoraInicio;
        reserva.HoraFin = nuevaHoraFin;

        _reservaDatos.Actualizar(reserva);
        return $"Reserva #{idReserva} modificada correctamente.";
    }

    public string CancelarReserva(int idReserva)
    {
        var reserva = _reservaDatos.BuscarPorId(idReserva);
        if (reserva == null)
            return "La reserva indicada no existe.";

        if (string.Equals(reserva.Estado, "Cancelada", StringComparison.OrdinalIgnoreCase))
            return "La reserva ya se encuentra cancelada.";

        _reservaDatos.Cancelar(idReserva);
        return $"Reserva #{idReserva} cancelada correctamente.";
    }

    public List<Reserva> ObtenerReservas()
    {
        return _reservaDatos.Listar();
    }
}