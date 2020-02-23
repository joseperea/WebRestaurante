using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRestaurante.ClasesUtil
{
    public class SeparadorMenu
    {
        private static string[] Codigo;
        private static string[] MenuCantidad;

        public static string[] Menu(string[] selectedMenu, string[] DCantida)
        {
            string id = "";
            if (selectedMenu != null)
            {
                foreach (var item in selectedMenu)
                {
                    if (item != null)
                    {
                        if (item != "")
                        {
                            string[] C = item.Split(new char[] { ',' });
                            id = id + "," + C[1];
                        }
                    }
                }
            }            
            if (id != "")
            {
                Codigo = id.Split(new char[] { ',' });
                return Cantidad(Codigo, DCantida, selectedMenu);
            }
            else
            {
                selectedMenu = null;
                return selectedMenu;
            }
        }

        public static string[] Cantidad(string[] codigo, string[] DCantida, string[] selectedMenu)
        {
            string aux = "";
            foreach (var item in Codigo)
            {
                for (int i = 0; i < selectedMenu.Length; i++)
                {
                    if (selectedMenu[i] != "")
                    {
                        string[] C = selectedMenu[i].Split(new char[] { ',' });
                        if (C[1].Equals(item))
                        {
                            for (int x = 0; x < DCantida.Length; x++)
                            {
                                if (DCantida[x].Equals(item) && (x % 2) != 0)
                                {
                                    aux = aux + "." + C[0] + "," + DCantida[x - 1];
                                    x = DCantida.Length;
                                }
                            }
                            i = selectedMenu.Length;
                        }
                    }
                }
            }
            MenuCantidad = aux.Split(new char[] { '.' });
            return MenuCantidad;
        }

    }
}