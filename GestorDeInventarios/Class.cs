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
                            EnterInventory();
                            break;

                        case 2:
                            NewInventory();
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
        private static void NewInventory()
        {
            Console.Clear();
            Console.WriteLine("Nuevo Inventario\n");

            Console.WriteLine("Ingrese nombre");
            string? nameInventory = Console.ReadLine();

            Console.WriteLine("\n Ingrese Clave");
            string? passInventory = Console.ReadLine();

            Connection.SaveInventory(nameInventory, passInventory);

            Console.WriteLine(String.Format($"\n Inventario {0} creado!", nameInventory));
        }     

        private static void EnterInventory()
        {

        }
    }

    internal static class Connection
    {
        private static string ConnectionString = "Data Source = sql.bsite.net\\MSSQL2016; user = sebaviguie_inventarios; password = 081092** ; initial catalog = sebaviguie_inventarios";
        
        public static void SaveInventory(string name, string pass)
        {
            using (SqlConnection sesion = new SqlConnection(ConnectionString))
            {
                string sqlQuery = "INSERT INTO inventario (vcInvNom, vcInvCla) VALUES (@name, @pass)";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, sesion))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@pass", pass);
                    sesion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
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