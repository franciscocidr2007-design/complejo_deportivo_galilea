// LogicaNegocio/PagoNegocio.cs
using System;
using System.Collections.Generic;
using ComplejoDeportivoGalilea.AccesoDatos;
using ComplejoDeportivoGalilea.Modelos;

namespace ComplejoDeportivoGalilea.LogicaNegocio;

public class PagoNegocio
{
    private readonly PagoDatos _pagoDatos;
    private readonly ReservaDatos _reservaDatos;

    public PagoNegocio(PagoDatos pagoDatos, ReservaDatos reservaDatos)
    {
        _pagoDatos = pagoDatos;
        _reservaDatos = reservaDatos;
    }

    public string RegistrarPago(Pago pago)
    {
        var reserva = _reservaDatos.BuscarPorId(pago.IdReserva);
        if (reserva == null)
            return "La reserva indicada no existe.";

        if (string.Equals(reserva.Estado, "Cancelada", StringComparison.OrdinalIgnoreCase))
            return "No se puede registrar pago para una reserva cancelada.";

        if (pago.Monto <= 0)
            return "El monto del pago debe ser mayor a 0.";

        if (string.IsNullOrWhiteSpace(pago.MedioPago))
            return "El medio de pago es obligatorio.";

        pago.FechaPago = DateTime.Now;
        pago.EstadoPago = "Pagado";
        _pagoDatos.Registrar(pago);
        return $"Pago #{pago.IdPago} registrado correctamente.";
    }

    public List<Pago> ObtenerPagos()
    {
        return _pagoDatos.Listar();
    }
}