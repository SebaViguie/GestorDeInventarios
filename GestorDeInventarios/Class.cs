using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDeInventarios
{
    internal static class App
    {
        public static void Launch()
        {
            Console.WriteLine("Gestión de Inventarios \n");

            bool next = true;
            byte option = 0;

            while (next)
            {
                Console.WriteLine("1 - Ingresar a inventario");
                Console.WriteLine("2 - Crear inventario");
                Console.WriteLine("3 - Salir \n");

                Console.WriteLine("Ingrese número de opción");

                try
                {
                    option = byte.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("Ingrese un número válido\n");
                }

                if (option != 0)
                {
                    switch (option)
                    {
                        case 1:
                            break;

                        case 2:
                            NuevoInventario();
                            break;

                        case 3:
                            next = false;
                            break;

                        default:
                            Console.Clear();
                            Console.WriteLine("Opción inválida");
                            break;
                    }
                }
            }
        }
        private static void NuevoInventario()
        {
            Console.Clear();
            Console.WriteLine("Nuevo Inventario\n");

            Console.WriteLine("Ingrese nombre");
            string nombreInventario = Console.ReadLine();

            Console.WriteLine("\n Ingrese Clave");
            string claveInventario = Console.ReadLine();

            
        }

        private static void Registrar()
        {
            SqlConnection sesion = new SqlConnection();
            sesion.ConnectionString = "Data Source = localhost; user = seba; password = ; initial catalog = POO";

            SqlCommand cmd = new SqlCommand { Connection = sesion };

            
                cmd.CommandText = $"INSERT INTO Cobros VALUES ('C', 'asd')";
                sesion.Open();
                cmd.ExecuteNonQuery();
                sesion.Close();
        }
    }

    internal class Articulo
    {
        private int codigoArt { get; set; }
        private string? nombreArt { get; set; }
        private int stock { get; set; }
        private decimal precio { get; set; }
        private DateTime fechaDeIngreso { get; set; }
        private DateTime fechaDeVencimiento { get; set; }

    }

    internal class Inventario
    {
        List<Articulo> listaDeArticulos = new List<Articulo>();
        private string? nombreInv { get; set; }
        private string? claveInv { get; set; }

        public void Agregar()
        {

        }

        public void Mostrar()
        {

        }

        public void Eliminar()
        {

        }

        public void VerSinStock()
        {

        }
    }
}