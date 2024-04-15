using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class dbSqlServer
    {
        public String sDatabase;
        public string sLastError = "";
        public SqlConnection conexion;
        public string Usuario, Password;
        String Servidor = "DESKTOP-5Q93D28\\DESKTOPJUAN";


        public dbSqlServer(string sUsuario, string sPasword)
        {
            Usuario = sUsuario;
            Password = sPasword;
            conexion = new SqlConnection($"server={Servidor};Initial Catalog=BibliotecaEST4;User ID={sUsuario};Password={sPasword};");
        }

        public Boolean AbrirConexion()
        {
            Boolean bALLOK = true;

            try
            {
                conexion.Open();
            }
            catch (Exception EX)
            {
                sLastError = EX.Message;
                bALLOK = false;
            }
            return bALLOK;
        }


        public Boolean ConexionAbierta()
        {
            Boolean bALLOK = true;
            try
            {
                bALLOK = conexion.State == System.Data.ConnectionState.Open ? true : false;
            }
            catch (Exception EX)
            {
                sLastError = EX.Message;
                bALLOK = false;
            }
            return bALLOK;
        }


        public Boolean EjecutarCommando(String sCmd)
        {
            Boolean bALLOK = true;

            conexion.Open();

            try
            {
                using (SqlCommand cmd = new SqlCommand(sCmd, conexion))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            catch (Exception EX)
            {
                bALLOK = false;
                sLastError = EX.Message;
            }
            conexion.Close();

            return bALLOK;
        }



        //Agragado por jorge
        public bool AgregarLibro(string titulo, string editorial, string categoria, string autor, int numEjemplar)
        {
            DateTime dateTime = DateTime.Now;
            string dias = dateTime.Day.ToString();
            string mes = dateTime.Month.ToString();
            string año = dateTime.Year.ToString();
            string horas = dateTime.Hour.ToString();
            string minutos = dateTime.Minute.ToString();
            string segundos = dateTime.Second.ToString();
            string milisegundos = dateTime.Millisecond.ToString();
            String Fecha = $"{año}/{mes}/{dias} {horas}:{minutos}:{segundos}:{milisegundos}";

            bool bALLOK = false;

            String query2 = "";

            using (SqlConnection conn = new SqlConnection($"server={Servidor};Initial Catalog=BibliotecaEST4;User ID={Usuario};Password={Password};"))
            {
                conexion.Open();

                string query = "INSERT INTO Libros (Titulo, Editorial, Categoria, Autor, Disponible ) VALUES (@Titulo, @Editorial, @Categoria, @Autor,@Disponible)";

                    SqlCommand command = new SqlCommand(query, conexion);
                    command.Parameters.AddWithValue("@Titulo", titulo);
                    command.Parameters.AddWithValue("@Editorial", editorial);
                    command.Parameters.AddWithValue("@Categoria", categoria);
                    command.Parameters.AddWithValue("@Autor", autor);
                    command.Parameters.AddWithValue("@Disponible", 1);

                    try
                    {
                        int rowsAffected = 0;
                        for (int i = 0; i < numEjemplar; i++)
                        {
                        DataTable Tabla = new DataTable();

                        rowsAffected += command.ExecuteNonQuery();

                            query2 = $"select top 1 * from Libros order by LibroID desc;";

                        SqlCommand comando = new SqlCommand(query2, conexion);
                        Tabla.Load(comando.ExecuteReader());

                        foreach (DataRow r in Tabla.Rows)
                          {

                            string query3 = $"INSERT INTO HistorialGeneral (LibroID, Titulo, Accion, FechaAccion, Turno_del_la_accion) VALUES ({int.Parse(r["LibroID"].ToString())}, '{r["Titulo"].ToString()}', 'Registro de Libro','{Fecha}', '{Usuario}');";
                           
                            SqlCommand comando2 = new SqlCommand(query3, conexion);
                            comando2.ExecuteNonQuery();
                          }
                        }

                        bALLOK = true;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al agregar el libro: " + ex.Message);
                        return false;
                    }


                    finally { conexion.Close(); }


                conexion.Close();
                return bALLOK;
               
            }
        }

        ///////////

        //////Agregado por Jorge////
        public bool AgregarPrestamo(string titulo, string alumno, String matricula, String grupo)
        {
            //Confiiguracion de la decha de prestamo
          
            bool bALLOK = false;
            DateTime dateTime = DateTime.Now;
            string dias = dateTime.Day.ToString();
            string mes = dateTime.Month.ToString();
            string año = dateTime.Year.ToString();
            string horas = dateTime.Hour.ToString();
            string minutos = dateTime.Minute.ToString();
            string segundos = dateTime.Second.ToString();
            string milisegundos = dateTime.Millisecond.ToString();
            String Fecha = $"{año}/{mes}/{dias} {horas}:{minutos}:{segundos}:{milisegundos}";

            //Configuracion de la decha de vencimiento
            DateTime fechaActual = DateTime.Now;
            DateTime fechaFutura = fechaActual.AddDays(15);
            string dias2 = fechaFutura.Day.ToString();
            string mes2 = fechaFutura.Month.ToString();
            string año2 = fechaFutura.Year.ToString();
            string horas2 = fechaFutura.Hour.ToString();
            string minutos2 = fechaFutura.Minute.ToString();
            string segundo2 = fechaFutura.Second.ToString();
            string milisegundos2 = fechaFutura.Millisecond.ToString();
            String Fecha2 = $"{año2}/{mes2}/{dias2} {horas2}:{minutos2}:{segundo2}:{milisegundos2}";
       

            DataTable Table = new DataTable();

            int LibroID = 0;

            Boolean bExist = true;

          

            using (SqlConnection conn = new SqlConnection($"server={Servidor};Initial Catalog=BibliotecaEST4;User ID={Usuario};Password={Password};"))
            {
                conexion.Open();

                String strCmd = "SELECT * FROM Libros " +
                              $"WHERE Titulo = '{titulo}'; ";

                SqlCommand cmd = new SqlCommand(strCmd, conexion);

                Table.Load(cmd.ExecuteReader());

                foreach (DataRow r in Table.Rows)
                {

                        String strCmd2 = "SELECT COUNT (*) FROM Prestamos " +
                                       $"WHERE LibroID = '{r["LibroID"].ToString()}'; ";

                        SqlCommand cmd2 = new SqlCommand(strCmd2, conexion);

                   

                    if (Int32.Parse(cmd2.ExecuteScalar().ToString()) <= 0)
                    {
                            bExist = false;
                            LibroID = int.Parse(r["LibroID"].ToString());
                            break;
                    }
                }

                if (bExist == false)
                {
                    string query = $"INSERT INTO Prestamos (Titulo, LibroID, nombreAlumno, Matricula, Grupo, Fecha_Prestamo, Fecha_Vencimiento, Turno_del_prestamo) VALUES (@Titulo, @LibroID, @nombreAlumno, @Matricula, @Grupo, @Fecha, @Fecha2, @Usuario)";

                    SqlCommand command = new SqlCommand(query, conexion);
                    command.Parameters.AddWithValue("@Titulo", titulo);
                    command.Parameters.AddWithValue("@nombreAlumno", alumno);
                    command.Parameters.AddWithValue("@Matricula", matricula);
                    command.Parameters.AddWithValue("@Grupo", grupo);
                    command.Parameters.AddWithValue("@Fecha", Fecha);
                    command.Parameters.AddWithValue("@Fecha2", Fecha2);
                    command.Parameters.AddWithValue("@Usuario", Usuario);
                    command.Parameters.AddWithValue("@LibroID", LibroID);


                    try
                    {
                        int rowsAffected = 0;

                        rowsAffected += command.ExecuteNonQuery();

                        string query3 = $"INSERT INTO HistorialGeneral (LibroID, Titulo, Accion, FechaAccion, Turno_del_la_accion) VALUES ({LibroID}, '{titulo}', 'Prestamo de Libro','{Fecha}', '{Usuario}');";

                        SqlCommand comando2 = new SqlCommand(query3, conexion);
                        comando2.ExecuteNonQuery();

                        sLastError = "El prestamo se realizo correctamente.";

                    }
                    catch (Exception ex)
                    {
                        sLastError = "Error al agregar el libro: " + ex.Message;
                        return false;
                    }
                    finally {conexion.Close(); }



                    // AGREGAAR ALUMNOOOOOOOOOOOO

                    //DataTable datosAlumnos = seleccionarAlumno(matricula);

                    //if (datosAlumnos.Rows.Count == 0)
                    //{
                    //    String strCmd3 = $"INSERT INTO Alumnos (nombreAlumno, Matricula, Grupo) VALUES ('{alumno}', '{matricula}', '{grupo}');";

                    //    SqlCommand cmd3 = new SqlCommand(strCmd3, conexion);

                    //    cmd.ExecuteReader();
                    //}


                    return bALLOK;
                }
                else
                {
                    sLastError = "El libro no está disponible.";
                    conexion.Close();
                    return false; 
                }


                    
            }
        }
        //////fin////

        public DataTable Registros()
        {
            DataTable Tabla = new DataTable();

            using (SqlConnection conn = new SqlConnection($"server={Servidor};Initial Catalog=BibliotecaEST4;User ID={Usuario};Password={Password};"))
            {
                conexion.Open();

                String strCmd = "SELECT * FROM Prestamos;";

                SqlCommand cmd = new SqlCommand(strCmd, conexion);

                Tabla.Load(cmd.ExecuteReader());

                conexion.Close();
            }
            return Tabla;
        }


        public Boolean EliminarLibro(String libroID)
        {
            DateTime dateTime = DateTime.Now;
            string dias = dateTime.Day.ToString();
            string mes = dateTime.Month.ToString();
            string año = dateTime.Year.ToString();
            string horas = dateTime.Hour.ToString();
            string minutos = dateTime.Minute.ToString();
            string segundos = dateTime.Second.ToString();
            string milisegundos = dateTime.Millisecond.ToString();
            String Fecha = $"{año}/{mes}/{dias} {horas}:{minutos}:{segundos}:{milisegundos}";

            bool bALLOK = false;

            using (SqlConnection conn = new SqlConnection($"server={Servidor};Initial Catalog=BibliotecaEST4;User ID={Usuario};Password={Password};"))
            {
                conexion.Open();

                string query = $"DELETE from Libros where LibroID = {libroID}";

                SqlCommand command = new SqlCommand(query, conexion);

                try
                {
                    int rowsAffected = 0;

                    DataTable Tabla = new DataTable();

                    string query2 = $"select * from Libros where LibroID = {libroID};";

                    SqlCommand comando = new SqlCommand(query2, conexion);
                    Tabla.Load(comando.ExecuteReader());

                    rowsAffected = command.ExecuteNonQuery();

                    foreach (DataRow r in Tabla.Rows)
                    {

                        string query3 = $"INSERT INTO HistorialGeneral (LibroID, Titulo, Accion, FechaAccion, Turno_del_la_accion) VALUES ({int.Parse(r["LibroID"].ToString())}, '{r["Titulo"].ToString()}', 'Eliminacion de Libro','{Fecha}', '{Usuario}');";

                        SqlCommand comando2 = new SqlCommand(query3, conexion);
                        comando2.ExecuteNonQuery();
                    }

                    if (rowsAffected > 0)
                    {
                        bALLOK = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al Eliminar el Libro: " + ex.Message);
                }
                finally
                {
                    conexion.Close();
                }
            }

            return bALLOK;
        }

        public DataTable BuscarLibro(String criterio, String sBusqueda)
        {
        
            DataTable datosTabla = new DataTable();
            DataTable Tabla = new DataTable();
            int R;
            String query = "";

            switch (criterio)
            {
                case "Por Titulo":
                    query = $"SELECT * from Libros where Titulo = '{sBusqueda}'";
                    break;
                case "Por Autor":
                    query = $"SELECT * from Libros where Autor = '{sBusqueda}'";
                    break;
                case "Por Categoria":
                    query = $"SELECT * from Libros where Categoria = '{sBusqueda}'";
                    break;
                case "Todos":
                    query = "Select * from Libros";
                    break;
                default:
                    query = $"SELECT * from Libros where Titulo = '{sBusqueda}'";
                    break;
            }

            using (SqlConnection conn = new SqlConnection($"server={Servidor};Initial Catalog=BibliotecaEST4;User ID={Usuario};Password={Password};"))
            {
                conexion.Open();

                try
                {
                    SqlCommand comando = new SqlCommand(query, conexion);
                    datosTabla.Load(comando.ExecuteReader());


                    foreach (DataRow r in datosTabla.Rows)
                    {
                        R = 0;

                        String strCmd2 = "SELECT * FROM Prestamos " +
                                       $"WHERE LibroID = '{r["LibroID"].ToString()}'; ";

                        SqlCommand cmd2 = new SqlCommand(strCmd2, conexion);

                        Tabla.Load(cmd2.ExecuteReader());

                        R = Tabla.Rows.Count;

                        Tabla.Rows.Clear();

                        if (R == 1)
                        {
                            r["Disponible"] = "False";
                        }
                    }

                }
                catch (SqlException ex)
                {
                    sLastError = ex.Message;
                }
                finally
                {
                    conexion.Close();
                }
                return datosTabla;
            }
        }

        public Boolean Finalizar_Prestamo(String PrestamoID)
        {
            DateTime dateTime = DateTime.Now;
            string dias = dateTime.Day.ToString();
            string mes = dateTime.Month.ToString();
            string año = dateTime.Year.ToString();
            string horas = dateTime.Hour.ToString();
            string minutos = dateTime.Minute.ToString();
            string segundos = dateTime.Second.ToString();
            string milisegundos = dateTime.Millisecond.ToString();
            String Fecha = $"{año}/{mes}/{dias} {horas}:{minutos}:{segundos}:{milisegundos}";

            bool bALLOK = false;

            using (SqlConnection conn = new SqlConnection($"server={Servidor};Initial Catalog=BibliotecaEST4;User ID={Usuario};Password={Password};"))
            {
                conexion.Open();

                string query = $"DELETE from Prestamos where PrestamoID = {PrestamoID}";

                SqlCommand command = new SqlCommand(query, conexion);

                try
                {
                    int rowsAffected = 0;

                    DataTable Tabla = new DataTable();

                    string query2 = $"select * from Prestamos where PrestamoID = {PrestamoID};";

                    SqlCommand comando = new SqlCommand(query2, conexion);
                    Tabla.Load(comando.ExecuteReader());

                    rowsAffected = command.ExecuteNonQuery();

                    foreach (DataRow r in Tabla.Rows)
                    {

                        string query3 = $"INSERT INTO HistorialGeneral (LibroID, Titulo, Accion, FechaAccion, Turno_del_la_accion) VALUES ({int.Parse(r["LibroID"].ToString())}, '{r["Titulo"].ToString()}', 'Finalizacion de prestamo','{Fecha}', '{Usuario}');";

                        SqlCommand comando2 = new SqlCommand(query3, conexion);
                        comando2.ExecuteNonQuery();
                    }

                    if (rowsAffected > 0)
                    {
                        bALLOK = true;
                    }
                }
                catch (Exception ex)
                {
                    sLastError = "Error al eliminar el registro: " + ex.Message;
                    return false;
                }
                
                finally
                {
                    conexion.Close();
                }
            }

            return bALLOK;
        }


        public DataTable Historial(String opcion)
        {
            DataTable datosTabla = new DataTable();
            String query = "";

            switch (opcion)
            {
                case "completo":
                    query = $"select * from HistorialGeneral;";
                    break;
                case "Eliminacion de Libro":
                    query = "SELECT * from HistorialGeneral where Accion = 'Eliminacion de Libro'";
                    break;
                case "Finalizacion de prestamo":
                    query = "SELECT * from HistorialGeneral where Accion = 'Finalizacion de prestamo'";
                    break;
                case "Registro de Libro":
                    query = "SELECT * from HistorialGeneral where Accion = 'Registro de Libro'";
                    break;
                case "Prestamo de Libro":
                    query = "SELECT * from HistorialGeneral where Accion = 'Prestamo de Libro'";
                    break;
                default:
                    query = $"select * from HistorialGeneral;";
                    break;
            }

            

            using (SqlConnection conn = new SqlConnection($"server={Servidor};Initial Catalog=BibliotecaEST4;User ID={Usuario};Password={Password};"))
            {
                conexion.Open();

                try
                {
                    SqlCommand comando = new SqlCommand(query, conexion);
                    datosTabla.Load(comando.ExecuteReader());
                }
                catch (SqlException ex)
                {
                    sLastError = ex.Message;
                }
                finally
                {
                    conexion.Close();
                }
                return datosTabla;
            }
        }
          public void CerrarConexion()
          {
            conexion.Close();
          }

        public DataTable AlumnosVencidos()
        {
            DateTime fechaActual = DateTime.Now;
            int diaActual = fechaActual.Day;
            int mesActual = fechaActual.Month;
            int añoActual = fechaActual.Year;

            DataTable dt = new DataTable();
            int Row = 0;
            int Row2 = 0;

            dt = Registros();
            int n = dt.Rows.Count;

            DataTable datos = new DataTable();
            datos.Columns.Add("PrestamoID", typeof(string));
            datos.Columns.Add("LibroID", typeof(string));
            datos.Columns.Add("Titulo", typeof(string));
            datos.Columns.Add("nombreAlumno", typeof(string));
            datos.Columns.Add("Matricula", typeof(string));
            datos.Columns.Add("Grupo", typeof(string));
            datos.Columns.Add("Fecha_Prestamo", typeof(string));
            datos.Columns.Add("Fecha_Vencimiento", typeof(string));
            datos.Columns.Add("Turno_del_prestamo", typeof(string));

            foreach (DataRow r in dt.Rows)
            {
                int o = 1;
                int diaVencimiento = 0;
                int mesVencimiento = 0;
                int añoVencimiento = 0;

                string[] FechaVencimiento = r["Fecha_Vencimiento"].ToString().Split('/', ' ');

                foreach (string parte in FechaVencimiento)
                {
                    if (o == 1)
                    {
                        diaVencimiento = int.Parse(parte);
                    }

                    if (o == 2)
                    {
                        mesVencimiento = int.Parse(parte);
                    }

                    if (o == 3)
                    {
                        añoVencimiento = int.Parse(parte);
                    }

                    o++;
                }

                if (mesVencimiento == mesActual && diaVencimiento < diaActual || mesVencimiento < mesActual && diaVencimiento > diaActual)
                {
                    datos.Rows.Add(
                        r["PrestamoID"].ToString(),
                        r["LibroID"].ToString(),
                        r["Titulo"].ToString(),
                        r["nombreAlumno"].ToString(),
                        r["Matricula"].ToString(),
                        r["Grupo"].ToString(),
                        r["Fecha_Prestamo"].ToString(),
                        r["Fecha_Vencimiento"].ToString(),
                        r["Turno_del_prestamo"].ToString()
                    );
                    Row2++;
                }
                else
                {
                    datos.Rows.Add(
                       r["PrestamoID"].ToString(),
                       r["LibroID"].ToString(),
                       r["Titulo"].ToString(),
                       r["nombreAlumno"].ToString(),
                       r["Matricula"].ToString(),
                       r["Grupo"].ToString(),
                       r["Fecha_Prestamo"].ToString(),
                       r["Fecha_Vencimiento"].ToString(),
                       r["Turno_del_prestamo"].ToString()
                   );
                    Row2++;
                }

            }


            return datos;
        }

        public Boolean RegistrarAlumno(String nombre, String matricula, String grupo)
        {
            Boolean bAllOk = false;

            using (SqlConnection conn = new SqlConnection($"server={Servidor};Initial Catalog=BibliotecaEST4;User ID={Usuario};Password={Password};"))
            {
                try
                {
                    conexion.Open();
                    String Query = $"INSERT INTO Alumnos (nombreAlumno, Matricula, Grupo) VALUES ('{nombre}', '{matricula}', '{grupo}');";

                    SqlCommand comando = new SqlCommand(Query, conexion);
                    comando.ExecuteNonQuery();

                    bAllOk = true;
                }
                catch (Exception error)
                {
                    string sError = error.Message;
                }
                finally
                {
                    conexion.Close();
                }
            }

            return bAllOk;
        }

        public DataTable seleccionarAlumno(String matricula)
        {
            DataTable datosAlumno = new DataTable();
            String query = $"Select * from Alumnos where Matricula = '{matricula}'";

            using (SqlConnection conn = new SqlConnection($"server={Servidor};Initial Catalog=BibliotecaEST4;User ID={Usuario};Password={Password};"))
            {
                try
                {
                    if (!ConexionAbierta())
                    {
                        conexion.Open();
                    }
                    
                    SqlCommand comando = new SqlCommand(query, conexion);
                    datosAlumno.Load(comando.ExecuteReader());
                }
                catch (SqlException ex)
                {
                    sLastError = ex.Message;
                }
                finally
                {
                    conexion.Close();
                }
            }

            return datosAlumno;
        }

    }
}
