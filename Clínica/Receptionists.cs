using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Runtime.Remoting.Contexts;

namespace Clínica
{
    public partial class Receptionists : Form
    {
        public Receptionists()
        {
            InitializeComponent();
            if (Form1.Role == "Recepcionista")
            {
                ReceLbl.Enabled = false;
                DoctorLbl.Enabled = false;
            }

            if (Form1.Role == "Doctor")
            {
                ReceLbl.Enabled = false;
                DoctorLbl.Enabled = false;
            }

            DisplayRec();
            CountPatients();
            CountDoctors();
            CountTest();
        }
        SqlConnection Con = new SqlConnection(Configuracion.CadenaConexion);
        private void CountPatients()
        {
            Con.Open();
            SqlDataAdapter sda = new SqlDataAdapter("Select count(*) from PatientTbl", Con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            PatNumLbl.Text = dt.Rows[0][0].ToString();
            Con.Close();
        }

        private void CountDoctors()
        {
            Con.Open();
            SqlDataAdapter sda = new SqlDataAdapter("Select count(*) from DoctorTbl", Con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            DocNumLbl.Text = dt.Rows[0][0].ToString();
            Con.Close();
        }

        private void CountTest()
        {
            Con.Open();
            SqlDataAdapter sda = new SqlDataAdapter("Select count(*) from TestTbl", Con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            LabTestLbl.Text = dt.Rows[0][0].ToString();
            Con.Close();
        }
        private void DelBtn_Click(object sender, EventArgs e)
        {
            if (Key == 0)
            {
                MessageBox.Show("Select The Receptionist");

            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("Delete from ReceptionistTbl where RecepId=@RKey", Con);
             
                    cmd.Parameters.AddWithValue("@RKey", Key);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Receptionist Deleted");
                    Con.Close();
                    DisplayRec();
                    Clear();
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
        }
        private void DisplayRec()
        {
            Con.Open();
            string Query = "Select * from ReceptionistTbl";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            ReceptionistDGV.DataSource = ds.Tables[0];
            Con.Close();
        }
        private void AddBtn_Click(object sender, EventArgs e)
        {
            string name = RNameTb.Text.Trim();

            // Validaciones (código existente)
            if (string.IsNullOrWhiteSpace(name) || RPassword.Text == "" || RPhoneTb.Text == "" || RAddressCb.Text == "")
            {
                MessageBox.Show("Falta información en uno o más campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (RPhoneTb.Text.Length < 9)
            {
                MessageBox.Show("El número de teléfono debe tener al menos 9 dígitos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (name.Length < 3)
            {
                MessageBox.Show("El nombre debe contener al menos 3 letras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (PhoneNumberExistsForReceptionist(RPhoneTb.Text))
            {
                MessageBox.Show("El número de teléfono ya existe en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Nueva verificación para la contraseña
            string password = RPassword.Text;

            if (string.IsNullOrWhiteSpace(password) || password.Length > 8 || !password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            {
                MessageBox.Show("La contraseña debe contener al menos una letra y un número, y no puede tener más de 8 caracteres.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("insert into ReceptionistTbl(RecepName, RecepPhone, RecepAdd, RecepPass) values(@RN, @RP, @RA, @RPA)", Con);
                cmd.Parameters.AddWithValue("@RN", RNameTb.Text);
                cmd.Parameters.AddWithValue("@RP", RPhoneTb.Text);
                cmd.Parameters.AddWithValue("@RA", RAddressCb.Text);
                cmd.Parameters.AddWithValue("@RPA", RPassword.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Recepcionista agregado", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Error al agregar el recepcionista: " + Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Con.Close();
                Clear();
                DisplayRec();
            }
        }
        int Key = 0;

        private bool PhoneNumberExistsForReceptionist(string phoneNumber)
        {
            try
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ReceptionistTbl WHERE RecepPhone = @PhoneNumber", Con))
                {
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception)
            {
                return false; // En caso de error, considera que el número de teléfono no existe.
            }
            finally
            {
                Con.Close();
            }
        }

        private void ReceptionistDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            RNameTb.Text = ReceptionistDGV.SelectedRows[0].Cells[1].Value.ToString();
            RPhoneTb.Text = ReceptionistDGV.SelectedRows[0].Cells[2].Value.ToString();
            RAddressCb.Text = ReceptionistDGV.SelectedRows[0].Cells[3].Value.ToString();
            RPassword.Text = ReceptionistDGV.SelectedRows[0].Cells[4].Value.ToString();
            if (RNameTb.Text == "")
            {
                Key = 0;
            }
            else
            {
                Key = Convert.ToInt32(ReceptionistDGV.SelectedRows[0].Cells[0].Value.ToString());
            }
        }



        private void EditBtn_Click(object sender, EventArgs e)
        {

            if (RNameTb.Text.Length <= 2)
            {
                MessageBox.Show("El nombre debe contener al menos 3 letras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else 
            { 

                if (RPassword.Text == "" || RPhoneTb.Text == "" || RAddressCb.Text == "")
                {
                    MessageBox.Show("Falta información.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                else if (RPhoneTb.Text.Length < 9)
                {
                    MessageBox.Show("El número de teléfono debe tener al menos 9 dígitos.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (RPhoneTb.Text[0] != '9')
                {
                    MessageBox.Show("El número de teléfono debe comenzar con '9'.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (PhoneNumberExistsForOtherReceptionist(RPhoneTb.Text, Key))
                {
                    MessageBox.Show("El número de teléfono ya está asociado a otro recepcionista.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        Con.Open();
                        SqlCommand cmd = new SqlCommand("update ReceptionistTbl set RecepName=@RN, RecepPhone=@RP, RecepAdd=@RA, RecepPass=@RPA where RecepId=@RKey", Con);
                        cmd.Parameters.AddWithValue("@RN", RNameTb.Text);
                        cmd.Parameters.AddWithValue("@RP", RPhoneTb.Text);
                        cmd.Parameters.AddWithValue("@RA", RAddressCb.Text);
                        cmd.Parameters.AddWithValue("@RPA", RPassword.Text);
                        cmd.Parameters.AddWithValue("@RKey", Key);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Recepcionista actualizado", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show("Error al actualizar el recepcionista: " + Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Con.Close();
                        Clear();
                        DisplayRec();
                    }
                }
            }
        }
        private bool PhoneNumberExistsForOtherReceptionist(string phoneNumber, int currentReceptionistKey)
        {
            try
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ReceptionistTbl WHERE RecepPhone = @PhoneNumber AND RecepId <> @CurrentReceptionistKey", Con))
                {
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmd.Parameters.AddWithValue("@CurrentReceptionistKey", currentReceptionistKey);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception)
            {
                return false; // En caso de error, considera que el número de teléfono no existe.
            }
            finally
            {
                Con.Close();
            }
        }

        private void Clear()
        {
            RNameTb.Text = "";
            RPassword.Text = "";
            RPhoneTb.Text = "";
            RAddressCb.Text = "";
            Key = 0;
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Receptionists_Load(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {
            Patients Obj = new Patients();
            Obj.Show();
            this.Hide();
        }

        private void label11_Click(object sender, EventArgs e)
        {
            Doctors Obj = new Doctors();
            Obj.Show();
            this.Hide();
        }

        private void label12_Click(object sender, EventArgs e)
        {
            LabTests Obj = new LabTests();
            Obj.Show();
            this.Hide();
        }

        private void label13_Click(object sender, EventArgs e)
        {
            Receptionists Obj = new Receptionists();
            Obj.Show();
            this.Hide();
        }

        private void label14_Click(object sender, EventArgs e)
        {
            Form1 Obj = new Form1();
            Obj.Show();
            this.Hide();
        }

        private void HomeLbl_Click(object sender, EventArgs e)
        {
            Homes Obj = new Homes();
            Obj.Show();
            this.Hide();
        }

        private void RPhoneTb_TextChanged(object sender, EventArgs e)
        {
            if (RPhoneTb.Text.Length > 0 && RPhoneTb.Text[0] != '9')
            {
                MessageBox.Show("El número de teléfono debe comenzar con '9'.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
            }
        }

        private void RPhoneTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verifica si el carácter ingresado es un número o la tecla de retroceso (backspace).
            // Verifica si el carácter ingresado es un número o la tecla de retroceso (backspace).
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true; // Cancela la entrada del carácter si no es un número ni backspace.
            }
            else if (RPhoneTb.Text.Length == 0 && e.KeyChar != '9')
            {
                e.Handled = true; // Cancela la entrada si el primer dígito no es '9'.
            }
        }

        private Point offset;

        private void Receptionists_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offset = new Point(e.X, e.Y);
            }
        }

        private void Receptionists_MouseMove(object sender, MouseEventArgs e)
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

        private void RNameTb_TextChanged(object sender, EventArgs e)
        {
            string input = RNameTb.Text;

            // Comprueba si la longitud del campo es mayor o igual a 3 o si contiene solo letras y espacios en blanco
            if (input.Length >= 3 || input.All(char.IsLetter) || input.All(char.IsWhiteSpace))
            {
                // El nombre tiene al menos 3 caracteres o solo contiene letras y espacios en blanco, no es necesario mostrar un mensaje de error.
            }
            
            else
            {
                // Si no cumple con la condición, muestra el mensaje de error solo si no contiene espacios en blanco.
                if (!input.All(char.IsWhiteSpace))
                {
                    MessageBox.Show("El nombre debe contener solo letras", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    RNameTb.Text = ""; // Limpia el campo para que el usuario pueda corregirlo.
                }
            }
        }

        private void RPassword_TextChanged(object sender, EventArgs e)
        {
            string password = RPassword.Text;

            if (password.Length > 8)
            {
                MessageBox.Show("La contraseña no puede tener más de 8 caracteres.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RPassword.Text = password.Substring(0, 8); // Limita la longitud a 8 caracteres.
                RPassword.SelectionStart = RPassword.Text.Length; // Establece el cursor al final del texto.
            }

            if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            {
                
            }
        }

    }
}


