using System;
using System.Globalization;
using ComplejoDeportivoGalilea.AccesoDatos;
using ComplejoDeportivoGalilea.LogicaNegocio;
using ComplejoDeportivoGalilea.Modelos;

class Program
{
    static void Main(string[] args)
    {
        ClienteDatos clienteDatos = new ClienteDatos();
        CanchaDatos canchaDatos = new CanchaDatos();
        ReservaDatos reservaDatos = new ReservaDatos();
        PagoDatos pagoDatos = new PagoDatos();

        ClienteNegocio clienteNegocio = new ClienteNegocio(clienteDatos);
        CanchaNegocio canchaNegocio = new CanchaNegocio(canchaDatos);
        ReservaNegocio reservaNegocio = new ReservaNegocio(reservaDatos, clienteDatos, canchaDatos);
        PagoNegocio pagoNegocio = new PagoNegocio(pagoDatos, reservaDatos);

        int opcion = -1;

        do
        {
            Console.Clear();
            Console.WriteLine("======================================");
            Console.WriteLine("     COMPLEJO DEPORTIVO GALILEA");
            Console.WriteLine("======================================");
            Console.WriteLine("1. Registrar cliente");
            Console.WriteLine("2. Gestionar canchas");
            Console.WriteLine("3. Consultar disponibilidad");
            Console.WriteLine("4. Crear reserva");
            Console.WriteLine("5. Modificar reserva");
            Console.WriteLine("6. Cancelar reserva");
            Console.WriteLine("7. Registrar pago");
            Console.WriteLine("8. Historial de reservas");
            Console.WriteLine("0. Salir");
            Console.WriteLine("======================================");
            Console.Write("Seleccione una opcion: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
                opcion = -1;

            try
            {
                switch (opcion)
                {
                    case 1:
                        Console.WriteLine("\n--- REGISTRAR CLIENTE ---");
                        Cliente cliente = new Cliente();
                        Console.Write("RUT: ");
                        cliente.Rut = Console.ReadLine() ?? "";
                        Console.Write("Nombre: ");
                        cliente.Nombre = Console.ReadLine() ?? "";
                        Console.Write("Telefono: ");
                        cliente.Telefono = Console.ReadLine() ?? "";
                        Console.Write("Correo: ");
                        cliente.Correo = Console.ReadLine() ?? "";

                        Console.WriteLine("\n" + clienteNegocio.RegistrarCliente(cliente));
                        break;

                    case 2:
                        Console.WriteLine("\n--- GESTIONAR CANCHAS ---");
                        foreach (var c in canchaNegocio.ObtenerCanchas())
                            Console.WriteLine($"ID {c.IdCancha} | {c.Nombre} | Pasto: {c.TipoPasto} | ${c.TarifaPorHora}/h | Estado: {c.Estado}");

                        Console.WriteLine("\n1. Registrar nueva cancha | 2. Actualizar cancha | 0. Volver");
                        string sub = Console.ReadLine() ?? "";
                        if (sub == "1")
                        {
                            Cancha nueva = new Cancha();
                            Console.Write("Nombre: ");
                            nueva.Nombre = Console.ReadLine() ?? "";
                            Console.Write("Tipo de pasto: ");
                            nueva.TipoPasto = Console.ReadLine() ?? "";
                            Console.Write("Tarifa por hora: ");
                            decimal.TryParse(Console.ReadLine(), out decimal tarifa);
                            nueva.TarifaPorHora = tarifa;
                            nueva.Estado = "Disponible";
                            Console.WriteLine("\n" + canchaNegocio.RegistrarCancha(nueva));
                        }
                        else if (sub == "2")
                        {
                            Cancha edit = new Cancha();
                            Console.Write("ID Cancha a actualizar: ");
                            int.TryParse(Console.ReadLine(), out int idEdit);
                            edit.IdCancha = idEdit;
                            Console.Write("Nuevo nombre: ");
                            edit.Nombre = Console.ReadLine() ?? "";
                            Console.Write("Nuevo tipo de pasto: ");
                            edit.TipoPasto = Console.ReadLine() ?? "";
                            Console.Write("Nueva tarifa por hora: ");
                            decimal.TryParse(Console.ReadLine(), out decimal nuevaTarifa);
                            edit.TarifaPorHora = nuevaTarifa;
                            Console.Write("Nuevo estado (Disponible/Mantencion/Inactiva): ");
                            edit.Estado = Console.ReadLine() ?? "";
                            Console.WriteLine("\n" + canchaNegocio.ActualizarCancha(edit));
                        }
                        break;

                    case 3:
                        Console.WriteLine("\n--- CONSULTAR DISPONIBILIDAD ---");
                        Console.Write("Fecha (dd-MM-yyyy): ");
                        if (!DateTime.TryParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaConsulta))
                        {
                            Console.WriteLine("Formato de fecha invalido.");
                            break;
                        }
                        Console.Write("Hora inicio (HH:mm): ");
                        TimeSpan.TryParse(Console.ReadLine(), out TimeSpan hIni);
                        Console.Write("Hora fin (HH:mm): ");
                        TimeSpan.TryParse(Console.ReadLine(), out TimeSpan hFin);

                        if (hFin <= hIni)
                        {
                            Console.WriteLine("La hora de fin debe ser mayor que la de inicio.");
                            break;
                        }

                        foreach (var c in canchaNegocio.ObtenerCanchas())
                        {
                            bool ocupada = reservaDatos.ExisteSuperposicion(c.IdCancha, fechaConsulta, hIni, hFin);
                            string estadoDisp = (!ocupada && c.Estado == "Disponible") ? "DISPONIBLE" : "NO DISPONIBLE";
                            Console.WriteLine($"Cancha {c.IdCancha} ({c.Nombre}): {estadoDisp}");
                        }
                        break;

                    case 4:
                        Console.WriteLine("\n--- CREAR RESERVA ---");
                        Reserva reserva = new Reserva();
                        Console.Write("ID Cliente: ");
                        int.TryParse(Console.ReadLine(), out int idCli);
                        reserva.IdCliente = idCli;
                        Console.Write("ID Cancha: ");
                        int.TryParse(Console.ReadLine(), out int idCan);
                        reserva.IdCancha = idCan;
                        Console.Write("Fecha (dd-MM-yyyy): ");
                        DateTime.TryParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaRes);
                        reserva.Fecha = fechaRes;
                        Console.Write("Hora inicio (HH:mm): ");
                        TimeSpan.TryParse(Console.ReadLine(), out TimeSpan inicioRes);
                        reserva.HoraInicio = inicioRes;
                        Console.Write("Hora fin (HH:mm): ");
                        TimeSpan.TryParse(Console.ReadLine(), out TimeSpan finRes);
                        reserva.HoraFin = finRes;

                        Console.WriteLine("\n" + reservaNegocio.CrearReserva(reserva));
                        break;

                    case 5:
                        Console.WriteLine("\n--- MODIFICAR RESERVA ---");
                        Console.Write("ID Reserva: ");
                        int.TryParse(Console.ReadLine(), out int idMod);
                        Console.Write("Nuevo ID Cancha: ");
                        int.TryParse(Console.ReadLine(), out int nuevaCancha);
                        Console.Write("Nueva fecha (dd-MM-yyyy): ");
                        DateTime.TryParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime nuevaFecha);
                        Console.Write("Nueva hora inicio (HH:mm): ");
                        TimeSpan.TryParse(Console.ReadLine(), out TimeSpan nuevoIni);
                        Console.Write("Nueva hora fin (HH:mm): ");
                        TimeSpan.TryParse(Console.ReadLine(), out TimeSpan nuevoFin);

                        Console.WriteLine("\n" + reservaNegocio.ModificarReserva(idMod, nuevaCancha, nuevaFecha, nuevoIni, nuevoFin));
                        break;

                    case 6:
                        Console.WriteLine("\n--- CANCELAR RESERVA ---");
                        Console.Write("ID Reserva a cancelar: ");
                        int.TryParse(Console.ReadLine(), out int idCanRes);
                        Console.Write("Confirmar cancelacion? (s/n): ");
                        if ((Console.ReadLine() ?? "").Trim().ToLower() == "s")
                            Console.WriteLine("\n" + reservaNegocio.CancelarReserva(idCanRes));
                        else
                            Console.WriteLine("\nOperacion cancelada por el usuario.");
                        break;

                    case 7:
                        Console.WriteLine("\n--- REGISTRAR PAGO ---");
                        Pago pago = new Pago();
                        Console.Write("ID Reserva: ");
                        int.TryParse(Console.ReadLine(), out int idResPago);
                        pago.IdReserva = idResPago;
                        Console.Write("Monto: ");
                        decimal.TryParse(Console.ReadLine(), out decimal montoPago);
                        pago.Monto = montoPago;
                        Console.Write("Medio de pago: ");
                        pago.MedioPago = Console.ReadLine() ?? "";

                        Console.WriteLine("\n" + pagoNegocio.RegistrarPago(pago));
                        break;

                    case 8:
                        Console.WriteLine("\n--- HISTORIAL DE RESERVAS ---");
                        foreach (var r in reservaNegocio.ObtenerReservas())
                            Console.WriteLine($"Reserva #{r.IdReserva} | Cliente ID: {r.IdCliente} | Cancha ID: {r.IdCancha} | {r.Fecha:dd-MM-yyyy} {r.HoraInicio:hh\\:mm}-{r.HoraFin:hh\\:mm} | Estado: {r.Estado}");
                        break;

                    case 0:
                        Console.WriteLine("\nCerrando el sistema...");
                        break;

                    default:
                        Console.WriteLine("\nOpcion no valida.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError inesperado: {ex.Message}");
            }

            if (opcion != 0)
            {
                Console.WriteLine("\nPresione ENTER para continuar...");
                Console.ReadLine();
            }

        } while (opcion != 0);
    }
}