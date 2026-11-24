
namespace ComplejoDeportivoGalilea.Modelos;

public class Cancha
{
    public int IdCancha { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string TipoPasto { get; set; } = string.Empty;
    public decimal TarifaPorHora { get; set; }
    public string Estado { get; set; } = string.Empty;
}
