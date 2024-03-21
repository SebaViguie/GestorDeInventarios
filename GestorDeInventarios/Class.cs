using Microsoft.VisualBasic.FileIO;
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
                            Inventory inventory = EnterInventory();
                            ManageInventory(inventory);
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
            //validar si ya existe con ese nombre
            Connection.SaveInventory(nameInventory, passInventory);

            Console.WriteLine(String.Format("\n Inventario {0} creado!\n", nameInventory));
        }     

        private static Inventory EnterInventory()
        {
            Console.Clear();
            Console.WriteLine("Lista de inventarios\n");

            Connection.ShowInventory();

            Console.WriteLine("\n Ingrese nombre de inventario");
            string? nameInv = Console.ReadLine();

            Console.WriteLine("\n Ingrese contraseña");
            string? passInv = Console.ReadLine();

            Inventory inventory = Connection.SearchInventory(nameInv, passInv);

            return inventory;
        }

        private static void ManageInventory(Inventory inventory)
        {
            byte option = 0;
            bool next = true;

            while(next)
            {
                Console.Clear();
                Console.WriteLine($"{inventory.GetName()}");
                Console.WriteLine("------------------------");
                Console.WriteLine("Elija una opción\n");
                Console.WriteLine("1 - Mostrar artículos");
                Console.WriteLine("2 - Agregar artículo");
                Console.WriteLine("3 - Eliminar artículo");
                Console.WriteLine("4 - Eliminar inventario");
                Console.WriteLine("5 - Salir\n");

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
                            inventory.Show();
                            break;

                        case 2:
                            inventory.Add();
                            break;

                        case 3:
                            inventory.RemoveArticle();
                            break;

                        case 4:
                            Console.WriteLine("¿Seguro que desea eliminar este inventario? (s/n)");
                            char confirm = char.Parse(Console.ReadLine());
                            if(confirm == 's')
                            {
                                Connection.DeleteInventory(inventory.GetCode());
                                next = false;
                                Console.Clear();
                            }
                            break;

                        case 5:
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

        public static List<Article> LoadArticles(int codInv)
        {
            List<Article> articles = new List<Article>();

            DbBase((sesion) =>
            {
                string sqlQuery = "SELECT * FROM articulo WHERE iInvCod=@invCode";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, sesion))
                {
                    cmd.Parameters.AddWithValue("@invCode", codInv);

                    sesion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            articles.Add(new Article(reader.GetString(2), reader.GetInt32(3), reader.GetDecimal(4), reader.GetDateTime(5), reader.GetDateTime(6)));
                        }
                    }

                }
            });

            return articles;
        }

        public static void SaveArticle(int invCode, string name, int stock, decimal price, DateTime dateExp)
        {
            DbBase((sesion) =>
            {
                string sqlQuery = "INSERT INTO articulo (iInvCod, vcArtNom, iArtSto, deArtPre, daArtIng, daArtVen) VALUES (@invCode, @name, @stock, @price, @dateIn, @dateExp)";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, sesion))
                {
                    cmd.Parameters.AddWithValue("@invCode", invCode);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@dateIn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@dateExp", dateExp);

                    sesion.Open();
                    cmd.ExecuteNonQuery();
                }
            });
        }

        public static int RemoveArticle(string name, int code)
        {
            int affected = 0;

            Connection.DbBase((sesion) =>
            {
                string sqlQuery = $"DELETE FROM articulo WHERE vcArtNom = @name AND iInvCod = @code";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, sesion))
                {
                    cmd.Parameters.AddWithValue($"@name", name);
                    cmd.Parameters.AddWithValue($"@code", code);
                    sesion.Open();
                    affected = cmd.ExecuteNonQuery();
                }
            });

            return affected;
        }

        public static void DeleteInventory(int code)
        {
            DbBase((sesion) =>
            {
                string sqlQuery = "DELETE FROM inventario WHERE iInvCod = @code";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, sesion))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    sesion.Open();
                    cmd.ExecuteNonQuery();
                }
            });
        }
    }

    internal class Article
    {
        private string? nameArt { get; set; }
        private int stock { get; set; }
        private decimal price { get; set; }
        private DateTime dateIn { get; set; }
        private DateTime dateExp { get; set; }

        public Article(string name, int stock, decimal price, DateTime dateIn, DateTime exp)
        {
            this.nameArt = name;
            this.stock = stock;
            this.price = price;
            this.dateIn = dateIn;
            this.dateExp = exp;
        }

        public string GetName()
        {
            return this.nameArt;
        }

        public int GetStock()
        {
            return this.stock;
        }

        public decimal GetPrice()
        {
            return this.price;
        }

        public DateTime GetDateIn()
        {
            return this.dateIn;
        }

        public DateTime GetDateExp()
        {
            return this.dateExp;
        }
    }

    internal class Inventory
    {
        List<Article> listOfArticles = new List<Article>();

        private int codInv { get; set; }
        private string? nameInv { get; set; }

        public Inventory(int code, string name)
        {
            this.codInv = code;
            this.nameInv = name;
            this.listOfArticles = Connection.LoadArticles(this.GetCode());
        }

        public int GetCode()
        {
            return this.codInv;
        }

        public string GetName()
        {
            return this.nameInv;
        }
        public void Add()
        {
            //validaciones
            Console.Clear();
            Console.WriteLine("Producto nuevo\n");
            Console.Write("Nombre: ");
            string name = Console.ReadLine();
            Console.Write("Stock: ");
            int stock = int.Parse(Console.ReadLine());
            Console.Write("Precio: ");
            decimal price = decimal.Parse(Console.ReadLine());
            Console.Write("Año de vencimiento: ");
            int year = int.Parse(Console.ReadLine());
            Console.Write("Mes de vencimiento: ");
            int month = int.Parse(Console.ReadLine());
            Console.Write("Día de vencimiento: ");
            int day = int.Parse(Console.ReadLine());
            DateTime exp = new DateTime(year, month, day);

            Connection.SaveArticle(this.GetCode(), name, stock, price, exp);
            this.listOfArticles = Connection.LoadArticles(this.GetCode());
        }

        public void Show()
        {
            Console.Clear();

            foreach (Article item in this.listOfArticles)
            {
                Console.WriteLine($"{item.GetName()} - ${item.GetPrice()} - {item.GetStock()} unidades - Ingreso: {item.GetDateIn()} - Vencimiento: {item.GetDateExp()}");
            }

            Console.WriteLine("\nPresione enter para continuar");
            Console.ReadLine();
        }

        public void RemoveArticle()
        {
            Console.Clear();
            Console.Write("Nombre de producto a eliminar: ");
            string name = Console.ReadLine();

            //validaciones
            Console.Clear();
            Console.WriteLine(string.Format("{0} artículos eliminados", Connection.RemoveArticle(name, this.GetCode())));
            this.listOfArticles = Connection.LoadArticles(this.GetCode());
            Console.Write("\nPresione enter para continuar");
            Console.ReadLine();
        }
    }
}