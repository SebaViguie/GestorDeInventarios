using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

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

            Console.WriteLine(String.Format("\n Inventario {0} creado!\n", nameInventory));
        }     

        private static void EnterInventory()
        {
            Console.Clear();
            Console.WriteLine("Lista de inventarios\n");

            Connection.ShowInventory();

            Console.WriteLine("\n Ingrese nombre de inventario");
            string? nameInv = Console.ReadLine();

            Console.WriteLine("\n Ingrese contraseña");
            string? passInv = Console.ReadLine();

            Inventory inventory = Connection.SearchInventory(nameInv, passInv);

            Console.WriteLine(inventory.GetCode());
        }
    }

    internal static class Connection
    {
        private static string ConnectionString = "Data Source = sql.bsite.net\\MSSQL2016; user = sebaviguie_inventarios; password = 081092** ; initial catalog = sebaviguie_inventarios";
        
        private static void DbBase(Action<SqlConnection> action)
        {
            using (SqlConnection sesion = new SqlConnection(ConnectionString))
            {
                try
                {
                    action(sesion);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(string.Format("Error: {0}", ex.Message)); 
                }
            }
        }

        public static void SaveInventory(string name, string pass)
        {
            DbBase((sesion) =>
            {
                string sqlQuery = "INSERT INTO inventario (vcInvNom, vcInvCla) VALUES (@name, @pass)";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, sesion))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@pass", pass);
                    sesion.Open();
                    cmd.ExecuteNonQuery();
                }
            });
        }

        public static void ShowInventory()
        {
            DbBase((sesion) =>
            {
                string sqlQuery = "SELECT vcInvNom FROM inventario";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, sesion))
                {
                    sesion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            Console.WriteLine(name);
                        }
                    }
                }
            });
        }

        public static Inventory SearchInventory(string name, string pass)
        {
            Inventory inventory = null;

            DbBase((sesion) =>
            {

                string sqlQuery = "SELECT * FROM inventario WHERE vcInvNom = @name AND vcInvCla = @pass";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, sesion))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@pass", pass);

                    sesion.Open();
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            inventory = new Inventory(reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Inventario o contraseña incorrectos");
                    }

                }
            });

            return inventory;
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

    internal class Inventory
    {
        List<Articulo> listaDeArticulos = new List<Articulo>();

        private int codInv { get; set; }
        private string? nombreInv { get; set; }

        public Inventory(int code, string name)
        {
            this.codInv = code;
            this.nombreInv = name;
        }

        public int GetCode()
        {
            return this.codInv;
        }
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