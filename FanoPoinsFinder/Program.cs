


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace FanoPoinsFinder
{
    class Program
    {

        static ArrayList list_of_remain = new ArrayList();
        static ArrayList list_of_selected = new ArrayList();
        static ArrayList list_of_checked = new ArrayList();
        static ArrayList selected_x_adresses = new ArrayList();
        static ArrayList selected_y_adresses = new ArrayList();
        static ArrayList list_of_remain_for_sort = new ArrayList();
        static string is_print_to_screen = "evet";

        const  int rows_count = 91;
        const  int column_count = 10;
        static string file_location = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\files\";

        static string[,] matrix_of_excell = new string[rows_count, column_count];


        //this method for read points from excell file
        static void load_matrix_from_excell_file()
        {
            string loc_path = file_location+"fano.xls";
            string cnn_str = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + loc_path + "; Extended Properties='Excel 8.0;HDR=Yes'";
            OleDbConnection con = new OleDbConnection(cnn_str);
            con.Open();
            string query = "select * from [fano_points$]";
            OleDbDataAdapter data_adaptor = new OleDbDataAdapter(query, con);
            con.Close();
            DataTable dt = new DataTable();
            data_adaptor.Fill(dt);
            int rows_a = dt.Rows.Count;
            int column_a = dt.Columns.Count;
            for (int i = 0; i < rows_a; i++)
            {
                for (int j = 0; j < column_a; j++)
                {
                    matrix_of_excell[i, j] = dt.Rows[i][j].ToString();
                }
            }

            Console.WriteLine("\nexcell had tranported to maxrix array.");
        }


        //this method for get static points from text file which given
        static void get_const_point()
        {
            
            StreamReader read = new StreamReader(file_location + "other_const.txt");
            string row;
            while ((row = read.ReadLine()) != null)
            {
                list_of_selected.Add(row.ToString());
                Console.Write(row.ToString()+"\t");
            }
            read.Close();
            Console.WriteLine("\npoints had been readed.");
        }



        // this method for select a new point
        static void select_new_point()
        {
               step1:
               Console.Write(list_of_selected.Count+1 + ". Point Input : ");
               string inputted = Console.ReadLine();
           
               if(is_on_list_of_remain(inputted))
               {
                   if (!is_on_list_of_selected(inputted))
                   {
                       list_of_selected.Add(inputted);
                   }
                   else
                   {
                       Console.ForegroundColor = ConsoleColor.Red;
                       Console.WriteLine(inputted + " Already Selected Before !");
                       Console.ForegroundColor = ConsoleColor.Black;
                       goto step1;
                   }
               }
               else
               {
                   Console.ForegroundColor = ConsoleColor.Red;
                   Console.WriteLine(inputted + " Cant Be Select OuT Of List!");
                   Console.ForegroundColor = ConsoleColor.Black;
                   goto step1;
               }
        }
        

  
       static bool is_checked(string point)
       {
            bool d = false;
            for (int i = 0; i < list_of_checked.Count; i++)
            {
                if (point == list_of_checked[i].ToString())
                {
                    d = true;
                    break;
                }
            }
            return d;
        }



       static bool is_point_selected(string x)
       {
           bool d = false;
           for (int i = 0; i < list_of_selected.Count; i++)
           {
               if (list_of_selected[i].ToString()==x) 
               {
                   d = true;
                   break;
               }
           }
           return d;
       }


       static void print_matrix()
       {
            for (int i = 0; i < rows_count; i++)
            {
                for (int j = 0; j < column_count; j++)
                {
                    if (is_point_selected(matrix_of_excell[i, j]))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.Write(string.Format("{0,-4}", matrix_of_excell[i, j]));  
                }
                Console.WriteLine("");
            }
            Console.WriteLine("\n");
        }



        static void create_combination()
        {
            int lookup = 0;
            while (lookup < list_of_selected.Count)
            {
                for (int i = lookup + 1; i < list_of_selected.Count; i++)
                {
                    find_crossed_points_row_index(list_of_selected[lookup].ToString(), list_of_selected[i].ToString());
                }
                lookup++;
            }
        }


        static void find_crossed_points_row_index(string n1, string n2)
        {
           
            for (int i = 0; i < rows_count; i++)
            {
                int total = 0;
                for (int j = 0; j < column_count; j++)
                {
                    if (matrix_of_excell[i, j] == n1 || matrix_of_excell[i, j] == n2)
                    {
                        total++;
                    }
                }
                if (total >= 2)
                {
                    Console.WriteLine(n1+"\t"+n2+"\t"+i+" row");
                    if (is_print_to_screen == "yes") Console.Write("checked =" + n1 + "," + n2 + ", row= " + i + "  deleted points :");
                    delete_from_row_unselected_points(i, n1, n2);
                }
            }
        }


        static void delete_from_row_unselected_points(int row,string n1,string n2)
        {
            for (int j = 0; j < column_count; j++)
            {
                if (matrix_of_excell[row, j] != n1 && matrix_of_excell[row, j] != n2)
                {
                    if (matrix_of_excell[row, j] != " ")
                    {
                        if (is_print_to_screen == "evet")
                        Console.Write(matrix_of_excell[row, j] + " ");
                        delete_from_matrix(matrix_of_excell[row, j]);
                    }
                }
            }

            if (is_print_to_screen == "yes")
            Console.WriteLine("\n---------------");

        }


        static void delete_from_matrix(string point)
        {
            for (int i = 0; i < rows_count; i++)
            {
                for (int j = 0; j < column_count; j++)
                {
                    if (!is_point_selected(matrix_of_excell[i, j]) && matrix_of_excell[i, j] == point)
                    {
                        matrix_of_excell[i, j] = " ";
                    }
                }
            }
        }


        static void add_remain_to_list()
        {
            list_of_remain.Clear();

            for (int i = 0; i < rows_count; i++)
            {
                for (int j = 0; j < column_count; j++)
                {
                    if (matrix_of_excell[i, j] != " " && !is_on_list_of_remain(matrix_of_excell[i, j]) && !is_on_list_of_selected(matrix_of_excell[i, j]))
                    {
                        list_of_remain.Add(matrix_of_excell[i, j]);
                    }
                }
            }
        }

        static bool is_on_list_of_remain(string point)
        {
            bool d = false;
            for (int i = 0; i < list_of_remain.Count; i++)
            {
                if (list_of_remain[i].ToString() == point)
                {
                    d = true;
                    break;
                }
            }
            return d;
        }


        static bool is_on_list_of_selected(string point)
        {
            bool d = false;
            for (int i = 0; i < list_of_selected.Count; i++)
            {
                if (list_of_selected[i].ToString() == point)
                {
                    d = true;
                    break;
                }
            }
            return d;
        }


        static void print_remain()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            for (int i = 0; i < list_of_remain.Count; i++)
            {
                list_of_remain_for_sort.Add(Convert.ToInt32(list_of_remain[i]));
            }
            list_of_remain_for_sort.Sort();
            Console.Write("Remain : ");
            for (int i = 0; i < list_of_remain_for_sort.Count; i++)
            {
                Console.Write(list_of_remain_for_sort[i] + "  ");
            }
            Console.WriteLine("\n total = " + list_of_remain_for_sort.Count + " reman point");
            Console.ForegroundColor = ConsoleColor.Black;
            list_of_remain_for_sort.Clear();
        }


 
        static void clear_data()
        {
            list_of_remain.Clear();
            list_of_selected.Clear();
            list_of_checked.Clear();
            selected_x_adresses.Clear();
            selected_y_adresses.Clear();
            list_of_remain_for_sort.Clear();
        }

        static void print_parameters()
        {


            StreamReader read = new StreamReader(file_location + "parameters.txt");
            string text = read.ReadToEnd();
            read.Close();
            //Yazma işlemini başarı ile tamamladığımızı kullanıcıya bildirelim..
            Console.WriteLine("File had been wrotten...");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("inputted points =");
            string temp = "";
            for (int i = 0; i < list_of_selected.Count; i++)
			{
                temp += (string.Format("{0,-4}",list_of_selected[i]));
			    Console.Write("\t"+list_of_selected[i]);
			}

            StreamWriter write = new StreamWriter(file_location + "parameters.txt");
            text += "\n"+temp;
            write.WriteLine(text);
            write.Close();
            Console.ForegroundColor = ConsoleColor.Black;
        }

        static void actions()
        {
            select_new_point();
            create_combination();
            print_matrix();
            add_remain_to_list();
            print_remain();
        }



        static void Main(string[] args)
        {

            StreamReader load_prefences = new StreamReader(file_location + "is_print.txt");
            is_print_to_screen = load_prefences.ReadLine();
            load_prefences.Close();

            step1:
            clear_data();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            load_matrix_from_excell_file();
          
            print_matrix();
            add_remain_to_list();
            print_remain();

            get_const_point();
         
            bool continueee = true;
            while (continueee)
            {
                try
                {
                    actions();
                    if (list_of_remain.Count > 0)
                    {
                        continueee = true;
                      
                    }
                    else
                    {
                        continueee = false;
                    }
                }
                catch (Exception)
                {
                    Console.Write("Unknown Error !");
                    Console.ReadLine();
                    continueee = false;
                }
            }

            print_matrix();
            print_parameters();
            goto step1;
            Console.ReadKey();

        }
    }
}
