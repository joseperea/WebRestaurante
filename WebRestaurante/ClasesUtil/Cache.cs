using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebRestaurante.ClasesUtil
{
    public class Cache
    {
        public static void cargar(string[] selectedMenu,string[] DCantida)
        {
            string creada = "C:\\Windows\\Temp\\WebR";
            if (Directory.Exists(creada))
            {
                StreamWriter C = new StreamWriter("C:\\Windows\\Temp\\WebR\\selectedMenu.txt", false, System.Text.Encoding.UTF8);
                StreamWriter x = new StreamWriter("C:\\Windows\\Temp\\WebR\\DCantida.txt", false, System.Text.Encoding.UTF8);
                if (selectedMenu != null && DCantida != null)
                {
                    foreach (var item in selectedMenu)
                    {
                        C.WriteLine(item);
                    }
                    foreach (var item in DCantida)
                    {
                        x.WriteLine(item);
                    }
                    C.Close();
                    x.Close();
                }
                else
                {
                    C.WriteLine("");
                    x.WriteLine("");
                    C.Close();
                    x.Close();
                }
            }
            else
            {
                Directory.CreateDirectory(creada);
                StreamWriter C = new StreamWriter("C:\\Windows\\Temp\\WebR\\selectedMenu.txt", false, System.Text.Encoding.UTF8);
                StreamWriter x = new StreamWriter("C:\\Windows\\Temp\\WebR\\DCantida.txt", false, System.Text.Encoding.UTF8);
                if (selectedMenu != null && DCantida != null)
                {
                    foreach (var item in selectedMenu)
                    {
                        C.WriteLine(item);
                    }
                    foreach (var item in DCantida)
                    {
                        x.WriteLine(item);
                    }
                    C.Close();
                    x.Close();
                }
                else
                {
                    C.WriteLine("");
                    x.WriteLine("");
                    C.Close();
                    x.Close();
                }
            }           
        }
        public static string[] selectedMenu()
        {            
            StreamReader C = new StreamReader("C:\\Windows\\Temp\\WebR\\selectedMenu.txt");
            string aux = "",leer="";
            while ((leer = C.ReadLine()) != null)
            {
                aux = aux + "-" + leer;                               
            }
            C.Close();
            return aux.Split(new char[] {'-'});            
        }
        public static string[] DCantida()
        {
            StreamReader C = new StreamReader("C:\\Windows\\Temp\\WebR\\DCantida.txt");
            string aux = "", leer="";            
            while ((leer = C.ReadLine()) != null)
            {
                aux = aux + "-" + leer;
            }
            string[] or = aux.Split(new char[] { '-' });
            string[] org = new string[or.Length];
            for (int i = 0; i <or.Length ; i++)
            {
                if (or[i] != "")
                {
                    org[i-1] = or[i];
                }                
            }
            C.Close();
            return org;
        }
    }
}