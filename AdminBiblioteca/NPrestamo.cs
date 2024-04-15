using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using Conexiones;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AdminBiblioteca
{
    public partial class NPrestamo : Form
    {
        public dbSqlServer conexion = null;

        public String sData = "";
        string sUsuario, sPassword;


        public NPrestamo(string Usuario, string Pasword)
        {
            InitializeComponent();

            sUsuario = Usuario;
            sPassword = Pasword;
            //lblUser.Text = "Usuario: " + Usuario;

            conexion = new dbSqlServer(sUsuario, sPassword);
        }

        public NPrestamo(string Usuario, string Pasword,string Titulo)
        {
            InitializeComponent();

            sUsuario = Usuario;
            sPassword = Pasword;

            tbTitulo.Text = Titulo;

            conexion = new dbSqlServer(sUsuario, sPassword);
        }


        #region Mover Arrastrar Formulario
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void lblform_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void Inventarios_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        #endregion


        #region Botones de Ventana
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnRMenu_Click(object sender, EventArgs e)
        {
            this.Hide();
            Menu c = new Menu(sUsuario, sPassword);
            c.ShowDialog();
        }
        #endregion

        private void tbMatricula_TextChanged(object sender, EventArgs e)
        {
            DataTable datosAlumnos = conexion.seleccionarAlumno(tbMatricula.Text);

            if (tbAlumno.Text != "" && datosAlumnos.Rows.Count == 0)
            {
                
            }
            else
            {
                tbAlumno.Text = "";
            }

            if (tbGrupo.Text != "" && datosAlumnos.Rows.Count == 0)
            {

            }
            else
            {
                tbGrupo.Text = "";
            }



           

            if (datosAlumnos.Rows.Count > 0)
            {
                foreach (DataRow row in datosAlumnos.Rows)
                {
                    tbAlumno.Text = row[1].ToString();
                    tbMatricula.Text = row[2].ToString();
                    tbGrupo.Text = row[3].ToString();

                    break;
                }
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (tbTitulo.Text == "" || tbAlumno.Text == "" || tbMatricula.Text == "" || tbGrupo.Text == "")
            {
                MessageBox.Show("Termine de rellenar los apartados");
                return;
            }

            DataTable prestamosVencidos = new DataTable();
            prestamosVencidos.Columns.Add("PrestamoID", typeof(string));
            prestamosVencidos.Columns.Add("LibroID", typeof(string));
            prestamosVencidos.Columns.Add("Titulo", typeof(string));
            prestamosVencidos.Columns.Add("nombreAlumno", typeof(string));
            prestamosVencidos.Columns.Add("Matricula", typeof(string));
            prestamosVencidos.Columns.Add("Grupo", typeof(string));
            prestamosVencidos.Columns.Add("Fecha_Prestamo", typeof(string));
            prestamosVencidos.Columns.Add("Fecha_Vencimiento", typeof(string));
            prestamosVencidos.Columns.Add("Turno_del_prestamo", typeof(string));
            prestamosVencidos = conexion.AlumnosVencidos();

            foreach (DataRow registro in prestamosVencidos.Rows)
            {
                if (tbAlumno.Text == registro["nombreAlumno"].ToString())
                {
                    MessageBox.Show("El alumno tiene un prestamo vencido pendiente...");
                    return;
                }
            }

            string titulo = tbTitulo.Text; // asumiendo que tienes un TextBox llamado txtTitulo para ingresar el título del libro
            string nombreAlumno = tbAlumno.Text; // asumiendo que tienes un TextBox llamado txtAutor para ingresar el autor del libro
            string Matricula = tbMatricula.Text;
            string grupo = tbGrupo.Text; // asumiendo que tienes un TextBox llamado txtTitulo para ingresar el título del libro


            if (!conexion.AgregarPrestamo(titulo, nombreAlumno, Matricula, grupo))
            {
                MessageBox.Show(conexion.sLastError);

                tbTitulo.Text = ""; // asumiendo que tienes un TextBox llamado txtTitulo para ingresar el título del libro
                tbAlumno.Text = ""; // asumiendo que tienes un TextBox llamado txtAutor para ingresar el autor del libro
                tbMatricula.Text = "";
                tbGrupo.Text = "";
            }

            else
            {
               tbTitulo.Text = ""; // asumiendo que tienes un TextBox llamado txtTitulo para ingresar el título del libro
               tbAlumno.Text = ""; // asumiendo que tienes un TextBox llamado txtAutor para ingresar el autor del libro
               tbMatricula.Text = "";
               tbGrupo.Text = "";

               MessageBox.Show("El Prestamo se realizo correctamente :)");
            }
        }
    }
}
