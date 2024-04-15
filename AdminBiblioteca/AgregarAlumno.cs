using Conexiones;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminBiblioteca
{
    public partial class AgregarAlumno : Form
    {
        public dbSqlServer conexion = null;

        public String sData = "";
        string sUsuario, sPassword;

        public AgregarAlumno(string Usuario, string Pasword)
        {
            InitializeComponent();

            sUsuario = Usuario;
            sPassword = Pasword;
            //lblUser.Text = "Usuario: " + Usuario;

            conexion = new dbSqlServer(sUsuario, sPassword);
        }

        #region Botones de Ventana
        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnRMenu_Click(object sender, EventArgs e)
        {
            this.Hide();
            Menu c = new Menu(sUsuario, sPassword);
            c.ShowDialog();
        }
        #endregion

        #region Mover Arrastrar Formulario
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        private void lblform_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void AgregarAlumno_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);




        #endregion

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (tbGrupo.Text == "" || tbMatricula.Text == "" || tbNombre.Text == "")
            {
                MessageBox.Show("Termine de rellenar los apartados");
                return;
            }

            string grupo = tbGrupo.Text; // asumiendo que tienes un TextBox llamado txtTitulo para ingresar el título del libro
            string matricula = tbMatricula.Text; // asumiendo que tienes un TextBox llamado txtAutor para ingresar el autor del libro
            string nombre = tbNombre.Text;


            if (conexion.RegistrarAlumno(nombre, matricula, grupo))
            {
                MessageBox.Show("El alumno se ha registrado correctamente.");

                tbGrupo.Text = "";
                tbMatricula.Text = "";
                tbNombre.Text = "";
            }
        }

    }
}
