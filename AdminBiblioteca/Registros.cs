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
    public partial class Registros : Form
    {

        public dbSqlServer conexion = null;

        public String sData = "";
        string sUsuario, sPassword;


        public Registros(string Usuario, string Pasword)
        {
            InitializeComponent();

            sUsuario = Usuario;
            sPassword = Pasword;
            //lblUser.Text = "Usuario: " + Usuario;

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

            if (txtPrestamoID.Text == "")
            {
                txtPrestamoID.Text = "PrestamoID";
                txtPrestamoID.ForeColor = Color.Silver;
            }

        }

        private void Prestamos_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);

            if (txtPrestamoID.Text == "")
            {
                txtPrestamoID.Text = "PrestamoID";
                txtPrestamoID.ForeColor = Color.Silver;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);

            if (txtPrestamoID.Text == "")
            {
                txtPrestamoID.Text = "PrestamoID";
                txtPrestamoID.ForeColor = Color.Silver;
            }
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


        #region Marca de Agua

        private void txtPrestamoID_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtPrestamoID.Text == "PrestamoID")
            {
                txtPrestamoID.Text = "";
                txtPrestamoID.ForeColor = Color.Black;
            }
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtPrestamoID.Text == "")
            {
                txtPrestamoID.Text = "PrestamoID";
                txtPrestamoID.ForeColor = Color.Silver;
            }
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtPrestamoID.Text == "")
            {
                txtPrestamoID.Text = "PrestamoID";
                txtPrestamoID.ForeColor = Color.Silver;
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtPrestamoID.Text == "")
            {
                txtPrestamoID.Text = "PrestamoID";
                txtPrestamoID.ForeColor = Color.Silver;
            }
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtPrestamoID.Text == "")
            {
                txtPrestamoID.Text = "PrestamoID";
                txtPrestamoID.ForeColor = Color.Silver;
            }
        }

        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtPrestamoID.Text == "")
            {
                txtPrestamoID.Text = "PrestamoID";
                txtPrestamoID.ForeColor = Color.Silver;
            }
        }

        private void dataGridView2_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtPrestamoID.Text == "")
            {
                txtPrestamoID.Text = "PrestamoID";
                txtPrestamoID.ForeColor = Color.Silver;
            }
        }

        #endregion



        private void Registros_Load(object sender, EventArgs e)
        {
            Llenar_Registros();


            DataTable prueba = new DataTable();
            prueba.Columns.Add("PrestamoID", typeof(string));
            prueba.Columns.Add("LibroID", typeof(string));
            prueba.Columns.Add("Titulo", typeof(string));
            prueba.Columns.Add("nombreAlumno", typeof(string));
            prueba.Columns.Add("Matricula", typeof(string));
            prueba.Columns.Add("Grupo", typeof(string));
            prueba.Columns.Add("Fecha_Prestamo", typeof(string));
            prueba.Columns.Add("Fecha_Vencimiento", typeof(string));
            prueba.Columns.Add("Turno_del_prestamo", typeof(string));
            prueba = conexion.AlumnosVencidos();

            foreach (DataRow r in prueba.Rows)
            {
                cbAlumnosTontos.Items.Add(r["nombreAlumno"].ToString());
            }
        }


        private void btnFinalizarRegistro_Click(object sender, EventArgs e)
        {
            if (txtPrestamoID.Text == "")
            {
                txtPrestamoID.Text = "PrestamoID";
                txtPrestamoID.ForeColor = Color.Silver;
            }

            if (txtPrestamoID.Text == "PrestamoID")
            {
                MessageBox.Show("Termine de rellenar los apartados");
                return;
            }

            if (!conexion.Finalizar_Prestamo(txtPrestamoID.Text))
            {
                MessageBox.Show(conexion.sLastError);
            }

            else
            {
                dataGridView1.Rows.Clear();

                dataGridView2.Rows.Clear();

                Llenar_Registros();

                MessageBox.Show("El Prestamo se Finalizo correctamente :)");
            }
        }


        public void Llenar_Registros()
        {
            // Fecha del dia de hoy
            DateTime fechaActual = DateTime.Now;
            int diaActual = fechaActual.Day;
            int mesActual = fechaActual.Month;
            int añoActual = fechaActual.Year;

            DataTable dt = new DataTable();
            int Row = 0;
            int Row2 = 0;



            dt = conexion.Registros();
            int n = dt.Rows.Count;


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
                    dataGridView2.Rows.Add();
                    dataGridView2.Rows[Row2].Cells[0].Value = r["PrestamoID"].ToString();
                    dataGridView2.Rows[Row2].Cells[1].Value = r["LibroID"].ToString();
                    dataGridView2.Rows[Row2].Cells[2].Value = r["Titulo"].ToString();
                    dataGridView2.Rows[Row2].Cells[3].Value = r["nombreAlumno"].ToString();
                    dataGridView2.Rows[Row2].Cells[4].Value = r["Matricula"].ToString();
                    dataGridView2.Rows[Row2].Cells[5].Value = r["Grupo"].ToString();
                    dataGridView2.Rows[Row2].Cells[6].Value = r["Fecha_Prestamo"].ToString();
                    dataGridView2.Rows[Row2].Cells[7].Value = r["Fecha_Vencimiento"].ToString();
                    dataGridView2.Rows[Row2].Cells[8].Value = r["Turno_del_prestamo"].ToString();
                    Row2 = Row2 + 1;
                }
                else
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[Row].Cells[0].Value = r["PrestamoID"].ToString();
                    dataGridView1.Rows[Row].Cells[1].Value = r["LibroID"].ToString();
                    dataGridView1.Rows[Row].Cells[2].Value = r["Titulo"].ToString();
                    dataGridView1.Rows[Row].Cells[3].Value = r["nombreAlumno"].ToString();
                    dataGridView1.Rows[Row].Cells[4].Value = r["Matricula"].ToString();
                    dataGridView1.Rows[Row].Cells[5].Value = r["Grupo"].ToString();
                    dataGridView1.Rows[Row].Cells[6].Value = r["Fecha_Prestamo"].ToString();
                    dataGridView1.Rows[Row].Cells[7].Value = r["Fecha_Vencimiento"].ToString();
                    dataGridView1.Rows[Row].Cells[8].Value = r["Turno_del_prestamo"].ToString();
                    Row = Row + 1;
                }
            }
        }
    }
}
