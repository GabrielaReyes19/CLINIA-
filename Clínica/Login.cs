using Bunifu.Framework.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clínica
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Random rand = new Random();
            int num = rand.Next(6, 8);
            string captcha = "";
            int totl = 0;
            while (totl < num)
            {
                int chr = rand.Next(65, 91); // Genera un número entre 65 (A) y 90 (Z) para letras mayúsculas
                captcha += (char)chr;
                totl++;
            }

            lbcaptcha.Text = captcha;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        SqlConnection Con = new SqlConnection(Configuracion.CadenaConexion);
        public static string Role;
        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            if (lbcaptcha.Text == txtcaptcha.Text)
            {
                if (RoleCb.SelectedIndex == -1)
                {
                    MessageBox.Show("Select your Position");
                }
                else if (RoleCb.SelectedIndex == 0)
                {
                    if (UnameTb.Text == "" || PassTb.Text == "")
                    {
                        MessageBox.Show("Enter Both Admin Name and Password");

                    }
                    else if (UnameTb.Text == "Admin" && PassTb.Text == "Password")
                    {
                        Role = "Admin";
                        Patients Obj = new Patients();
                        Obj.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Wrong Admin Name and Password");
                    }
                }
                else if (RoleCb.SelectedIndex == 1)
                {
                    if (UnameTb.Text == "" || PassTb.Text == "")
                    {
                        MessageBox.Show("Enter Both Doctor Name and Password");

                    }
                    else
                    {
                        Con.Open();
                        SqlDataAdapter sda = new SqlDataAdapter("Select Count(*) from DoctorTbl where DocName='" + UnameTb.Text + "' and DocPass='" + PassTb.Text + "'", Con);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            Role = "Doctor";
                            Prescripciones Obj = new Prescripciones();
                            Obj.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Doctor Not found");
                        }
                        Con.Close();
                    }

                }
                else
                {
                    if (UnameTb.Text == "" || PassTb.Text == "")
                    {
                        MessageBox.Show("Enter Both Receptionist Name and Password");

                    }
                    else
                    {
                        Con.Open();
                        SqlDataAdapter sda = new SqlDataAdapter("Select Count(*) from ReceptionistTbl where RecepName='" + UnameTb.Text + "' and RecepPass='" + PassTb.Text + "'", Con);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            Role = "Recepcionista";
                            Homes Obj = new Homes();
                            Obj.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Receptionist Not found");
                        }
                        Con.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Captcha is incorrect");
                txtcaptcha.Clear(); // Limpiar el campo del CAPTCHA
            }

        }
        private void label4_Click(object sender, EventArgs e)
        {
            RoleCb.SelectedIndex = -1;
            UnameTb.Text = "";
            PassTb.Text = "";
            txtcaptcha.Text = "";
            checkBox1.Checked = false;
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RoleCb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void captcha_Click(object sender, EventArgs e)
        {

        }

        private void captchatb_TextChanged(object sender, EventArgs e)
        {

        }

        private void PassTb_TextChanged(object sender, EventArgs e)
        {

        }

        private void GenerateAndDisplayCaptcha()
        {
            Random rand = new Random();
            int num = rand.Next(6, 8);
            string captcha = "";
            int totl = 0;

            while (totl < num)
            {
                int chr = rand.Next(65, 91); // Genera un número entre 65 (A) y 90 (Z) para letras mayúsculas
                captcha += (char)chr;
                totl++;
            }

            lbcaptcha.Text = captcha;
        }
        private void btnActualizarCaptcha_Click(object sender, EventArgs e)
        {
            GenerateAndDisplayCaptcha();
        }


        private void txtcaptcha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (lbcaptcha.Text == txtcaptcha.Text)
                {
                    if (RoleCb.SelectedIndex == -1)
                    {
                        MessageBox.Show("Select your Position");
                    }
                    else if (RoleCb.SelectedIndex == 0)
                    {
                        if (UnameTb.Text == "" || PassTb.Text == "")
                        {
                            MessageBox.Show("Enter Both Admin Name and Password");

                        }
                        else if (UnameTb.Text == "Admin" && PassTb.Text == "Password")
                        {
                            Role = "Admin";
                            Patients Obj = new Patients();
                            Obj.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Wrong Admin Name and Password");
                        }
                    }
                    else if (RoleCb.SelectedIndex == 1)
                    {
                        if (UnameTb.Text == "" || PassTb.Text == "")
                        {
                            MessageBox.Show("Enter Both Doctor Name and Password");

                        }
                        else
                        {
                            Con.Open();
                            SqlDataAdapter sda = new SqlDataAdapter("Select Count(*) from DoctorTbl where DocName='" + UnameTb.Text + "' and DocPass='" + PassTb.Text + "'", Con);
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            if (dt.Rows[0][0].ToString() == "1")
                            {
                                Role = "Doctor";
                                Prescripciones Obj = new Prescripciones();
                                Obj.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Doctor Not found");
                            }
                            Con.Close();
                        }

                    }
                    else
                    {
                        if (UnameTb.Text == "" || PassTb.Text == "")
                        {
                            MessageBox.Show("Enter Both Receptionist Name and Password");

                        }
                        else
                        {
                            Con.Open();
                            SqlDataAdapter sda = new SqlDataAdapter("Select Count(*) from ReceptionistTbl where RecepName='" + UnameTb.Text + "' and RecepPass='" + PassTb.Text + "'", Con);
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            if (dt.Rows[0][0].ToString() == "1")
                            {
                                Role = "Recepcionista";
                                Homes Obj = new Homes();
                                Obj.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Receptionist Not found");
                            }
                            Con.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Captcha is incorrect");
                    txtcaptcha.Clear(); // Limpiar el campo del CAPTCHA
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked==true)
            {
                PassTb.UseSystemPasswordChar = false;
            }
            else
            {
                PassTb.UseSystemPasswordChar = true;
            }
        }

        private Point offset;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offset = new Point(e.X, e.Y);
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Mueve el formulario según la posición del mouse
                Point newLocation = this.Location;
                newLocation.X += e.X - offset.X;
                newLocation.Y += e.Y - offset.Y;
                this.Location = newLocation;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}



