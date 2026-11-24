using MySqlConnector;

namespace ComplejoDeportivoGalilea.AccesoDatos;

public class ConexionBd
{
    private static readonly string CadenaConexion =
        "Server=localhost;Port=3306;Database=complejo_deportivo_galilea;Uid=root;Pwd=;";

    public static MySqlConnection ObtenerConexion()
    {
        return new MySqlConnection(CadenaConexion);
    }
}