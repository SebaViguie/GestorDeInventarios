using System;
using System.Collections.Generic;
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
                    Console.WriteLine("Ingrese un número válido");
                }

                if (option != 0)
                {
                    switch (option)
                    {
                        case 1:
                            break;

                        case 2:
                            break;

                        case 3:
                            next = false;
                            break;

                        default:
                            Console.WriteLine("Opción inválida");
                            break;
                    }
                }
            }
        }
    }


}
