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
using System.Xml.Linq;

namespace Clínica
{
    public partial class Patients : Form
    {
        public Patients()
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

            }
            DisplayPat();
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
        private void DisplayPat()
        {
            Con.Open();
            string Query = "Select * from PatientTbl";
            SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            PatientsDGV.DataSource = ds.Tables[0];
            Con.Close();
        }
        int Key = 0;
        private void Clear()
        {
            PatNameTb.Text = "";
            PatGenCb.SelectedIndex = 0;
            PatHIVCb.SelectedIndex = 0;
            PatAddCb.Text = "";
            PatPhoneTb.Text = "";
            PatAlTb.Text = "";

            Key = 0;
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            string name = PatNameTb.Text.Trim(); // Elimina los espacios en blanco al principio y al final.

            if (string.IsNullOrWhiteSpace(name) || PatAlTb.Text == "" || PatAddCb.Text == "" || PatPhoneTb.Text == "" || PatGenCb.SelectedIndex == -1 || PatHIVCb.SelectedIndex == -1)
            {
                MessageBox.Show("Falta información en uno o más campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (PatPhoneTb.Text.Length < 9)
            {
                MessageBox.Show("El número de teléfono debe tener al menos 9 dígitos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (name.Length < 3)
            {
                MessageBox.Show("El nombre debe contener letras y al menos 3 caracteres.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (name.Length < 3 || PatAlTb.Text.Length < 3 || !IsOnlyLetters(PatAlTb.Text))
            {
                MessageBox.Show("El campo Alergias debe contener letras y al menos 3 caracteres.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (PhoneNumberExists(PatPhoneTb.Text))
            {
                MessageBox.Show("El número de teléfono ya existe en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("insert into PatientTbl(PatName, PatGen, PatDOB, PatAdd, PatPhone, PatHIV, PatAll)values(@PN, @PG, @PD, @PA, @PP, @PH, @PAl)", Con))
                {
                    cmd.Parameters.AddWithValue("@PN", PatNameTb.Text.Trim());
                    cmd.Parameters.AddWithValue("@PG", PatGenCb.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@PD", PatDOB.Value.Date);
                    cmd.Parameters.AddWithValue("@PA", PatAddCb.Text);
                    cmd.Parameters.AddWithValue("@PP", PatPhoneTb.Text);
                    cmd.Parameters.AddWithValue("@PH", PatHIVCb.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@PAl", PatAlTb.Text);

                    if (PatPhoneTb.Text.Length < 9)
                    {
                        MessageBox.Show("El número de teléfono debe tener al menos 9 dígitos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Sale de la función si el número es inválido.
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Paciente Agregado", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Error al agregar el paciente: " + Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Con.Close();
                Clear();
                DisplayPat();
            }
        }

        private bool PhoneNumberExists(string phoneNumber)
        {
            try
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM PatientTbl WHERE PatPhone = @PhoneNumber", Con))
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

        private bool IsOnlyLetters(string input)
        {
            return input.All(char.IsLetter);
        }
        private void PatientsDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PatNameTb.Text = PatientsDGV.SelectedRows[0].Cells[1].Value.ToString();
            PatGenCb.SelectedItem = PatientsDGV.SelectedRows[0].Cells[2].Value.ToString();
            PatDOB.Text = PatientsDGV.SelectedRows[0].Cells[3].Value.ToString();
            PatAddCb.Text = PatientsDGV.SelectedRows[0].Cells[4].Value.ToString();
            PatPhoneTb.Text = PatientsDGV.SelectedRows[0].Cells[5].Value.ToString();
            PatHIVCb.SelectedItem = PatientsDGV.SelectedRows[0].Cells[6].Value.ToString();
            PatAlTb.Text = PatientsDGV.SelectedRows[0].Cells[7].Value.ToString();

            if (PatNameTb.Text == "")
            {
                Key = 0;
            }
            else
            {
                Key = Convert.ToInt32(PatientsDGV.SelectedRows[0].Cells[0].Value.ToString());
            }
        }
        private void EditBtn_Click(object sender, EventArgs e)
            {
            
              if (PatNameTb.Text.Length <= 2)
               {
                   MessageBox.Show("El nombre debe contener al menos 3 letras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               }

            else
            {

                if (PatAlTb.Text == "" || PatAddCb.Text == "" || PatPhoneTb.Text == "" || PatGenCb.SelectedIndex == -1 || PatHIVCb.SelectedIndex == -1)
                {
                    MessageBox.Show("Falta información en uno o más campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (PatPhoneTb.Text.Length < 9)
                {
                    MessageBox.Show("El número de teléfono debe tener al menos 9 dígitos.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (PatPhoneTb.Text[0] != '9')
                {
                    MessageBox.Show("El número de teléfono debe comenzar con '9'.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (PhoneNumberExistsForOtherPatient(PatPhoneTb.Text, Key))
                {
                    MessageBox.Show("El número de teléfono ya está asociado a otro paciente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (PatNameTb.Text.Length <= 2 || PatAlTb.Text.Length < 3 || !IsOnlyLetters(PatAlTb.Text))
                {
                    MessageBox.Show("El campo Alergias debe contener al menos 3 letras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                else
                {
                    try
                    {
                        Con.Open();
                        SqlCommand cmd = new SqlCommand("Update PatientTbl set PatName=@PN, PatGen=@PG, PatDOB=@PD, PatAdd=@PA, PatPhone=@PP, PatHIV=@PH, PatAll=@PAl where PatId=@PKey", Con);
                            cmd.Parameters.AddWithValue("@PN", PatNameTb.Text.Trim());
                            cmd.Parameters.AddWithValue("@PG", PatGenCb.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@PD", PatDOB.Value.Date);
                            cmd.Parameters.AddWithValue("@PA", PatAddCb.Text);
                            cmd.Parameters.AddWithValue("@PP", PatPhoneTb.Text);
                            cmd.Parameters.AddWithValue("@PH", PatHIVCb.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@PAl", PatAlTb.Text);
                            cmd.Parameters.AddWithValue("@PKey", Key);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Patient Updated");
                        
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                    }
                    finally
                    {
                        Con.Close();
                        Clear();
                        DisplayPat();
                    }
                }
            }
        }


        private bool PhoneNumberExistsForOtherPatient(string phoneNumber, int currentPatientKey)
        {
            try
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM PatientTbl WHERE PatPhone = @PhoneNumber AND PatId <> @CurrentPatientKey", Con))
                {
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmd.Parameters.AddWithValue("@CurrentPatientKey", currentPatientKey);
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

        string connectionString = Configuracion.CadenaConexion;
        private void DelBtn_Click(object sender, EventArgs e)
        {

            if (Key == 0)
            {
                MessageBox.Show("Select The Patient");
            }
            else
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Consulta para verificar si tiene prescripciones
                        string query = "SELECT COUNT(*) FROM PrescriptionTbl WHERE PatId = @KeyId";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@KeyId", Key);

                            int count = (int)cmd.ExecuteScalar();

                            if (count > 0)
                            {
                                MessageBox.Show("No se puede eliminar paciente con una prescripción registrada");
                                return;
                            }

                        }

                        // Código original para eliminar
                        using (SqlCommand deleteCmd = new SqlCommand("Delete from PatientTbl where PatId=@PKey", conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@PKey", Key);

                            deleteCmd.ExecuteNonQuery();

                            MessageBox.Show("Patient Deleted");

                        }

                        DisplayPat();
                        Clear();

                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }

        }

        private void Patients_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label10_Click(object sender, EventArgs e)
        {


            Patients Obj = new Patients();
            Obj.Show();
            this.Hide();

        }


        private void label14_Click(object sender, EventArgs e)
        {
            Form1 Obj = new Form1();
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

        private void HomeLbl_Click(object sender, EventArgs e)
        {
            Homes Obj = new Homes();
            Obj.Show();
            this.Hide();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Prescripciones Obj = new Prescripciones();
            Obj.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Prescripciones Obj = new Prescripciones();
            Obj.Show();
            this.Hide();
        }

        private void PatPhoneTb_Leave(object sender, EventArgs e)
        {

        }

        private void PatPhoneTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verifica si el carácter ingresado es un número o la tecla de retroceso (backspace).
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true; // Cancela la entrada del carácter si no es un número ni backspace.
            }
            else if (PatPhoneTb.Text.Length == 0 && e.KeyChar != '9')
            {
                e.Handled = true; // Cancela la entrada si el primer dígito no es '9'.
            }
        }

        private void PatDOB_onValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = PatDOB.Value;

            // Establece la fecha mínima como el 1 de enero de 1960
            DateTime minDate = new DateTime(1960, 1, 1);

            // Establece la fecha máxima como la fecha actual
            DateTime maxDate = DateTime.Today;

            if (selectedDate < minDate || selectedDate > maxDate)
            {
                MessageBox.Show("Por favor, seleccione una fecha válida (desde 1960 hasta el año actual).", "Error de fecha", MessageBoxButtons.OK, MessageBoxIcon.Error);
                PatDOB.Value = maxDate; // Restablece la fecha a la fecha actual
            }
        }

        private void PatPhoneTb_TextChanged(object sender, EventArgs e)
        {
            if (PatPhoneTb.Text.Length > 0 && PatPhoneTb.Text[0] != '9')
            {
                MessageBox.Show("El número de teléfono debe comenzar con '9'.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Point offset;
        private void Patients_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offset = new Point(e.X, e.Y);
            }
        }

        private void Patients_MouseMove(object sender, MouseEventArgs e)
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

        private void PatNameTb_TextChanged(object sender, EventArgs e)
        {
            string input = PatNameTb.Text;

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
                    PatNameTb.Text = ""; // Limpia el cuadro de texto si no cumple con los requisitos.
                }
            }
        }

        private void PatGenCb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void PatGenCb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void PatHIVCb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }



    }
}
