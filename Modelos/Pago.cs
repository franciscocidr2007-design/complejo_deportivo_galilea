
namespace ComplejoDeportivoGalilea.Modelos;

public class Pago
{
    public int IdPago { get; set; }
    public int IdReserva { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string EstadoPago { get; set; } = string.Empty;
    public string MedioPago { get; set; } = string.Empty;
}
