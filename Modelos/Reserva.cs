
namespace ComplejoDeportivoGalilea.Modelos;

public class Reserva
{
    public int IdReserva { get; set; }
    public int IdCliente { get; set; }
    public int IdCancha { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string Estado { get; set; } = string.Empty;
}
