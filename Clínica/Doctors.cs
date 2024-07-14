using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Clínica
{
    public partial class Doctors : Form
    {
        SqlConnection Con = new SqlConnection(Configuracion.CadenaConexion);
        int Key = 0;

        public Doctors()
        {
            InitializeComponent();
            if (Form1.Role == "Recepcionista")
            {
                ReceLbl.Enabled = false;
                DoctorLbl.Enabled = false;
            }

            if (Form1.Role == "Doctors")
            {
                ReceLbl.Enabled = false;
                DoctorLbl.Enabled = false;
            }

            DisplayDoc();
            CountPatients();
            CountDoctors();
            CountTest();
        }

        private void CountPatients()
        {
            try
            {
                Con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("Select count(*) from PatientTbl", Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                PatNumLbl.Text = dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        private void CountDoctors()
        {
            try
            {
                Con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("Select count(*) from DoctorTbl", Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                DocNumLbl.Text = dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        private void CountTest()
        {
            try
            {
                Con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("Select count(*) from TestTbl", Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                LabTestLbl.Text = dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        string connectionString = Configuracion.CadenaConexion;

        private void DelBtn_Click(object sender, EventArgs e)
        {
            if (Key == 0)
            {
                MessageBox.Show("Select The Doctor");
            }
            else
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Consulta para verificar si tiene prescripciones
                        string query = "SELECT COUNT(*) FROM PrescriptionTbl WHERE DocId = @KeyId";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@KeyId", Key);

                            int count = (int)cmd.ExecuteScalar();

                            if (count > 0)
                            {
                                MessageBox.Show("No se puede eliminar doctor con una prescripción registrada");
                                return;
                            }

                        }

                        // Código original para eliminar
                        using (SqlCommand deleteCmd = new SqlCommand("Delete from DoctorTbl where DoctorId=@DKey", conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@DKey", Key);

                            deleteCmd.ExecuteNonQuery();

                            MessageBox.Show("Doctor Deleted");

                        }

                        DisplayDoc();
                        Clear();

                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }

            }

        }
        private void DisplayDoc()
        {
            try
            {
                Con.Open();
                string Query = "Select * from DoctorTbl";
                SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                var ds = new DataSet();
                sda.Fill(ds);
                DoctorsDGV.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        private void Clear()
        {
            DNameTb.Text = "";
            DocPhoneTb.Text = "";
            DocAddCb.Text = ""; 
            DocExpTb.Text = "";
            DocPasswordTb.Text = "";
            DocGenCb.SelectedIndex = 0;
            DocSpecCb.SelectedIndex = 0;
            Key = 0;
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            string name = DNameTb.Text.Trim(); // Elimina los espacios en blanco al principio y al final.

            if (string.IsNullOrWhiteSpace(name) || DocPasswordTb.Text == "" || DocPhoneTb.Text == "" || DocAddCb.Text == "" || DocGenCb.SelectedIndex == -1 || DocSpecCb.SelectedIndex == -1)
            {
                MessageBox.Show("Falta información en uno o más campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (DocPhoneTb.Text.Length < 9)
            {
                MessageBox.Show("El número de teléfono debe tener al menos 9 dígitos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (name.Length < 3)
            {
                MessageBox.Show("El nombre debe contener al menos 3 caracteres o espacios en blanco.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (PhoneNumberExists(DocPhoneTb.Text))
            {
                MessageBox.Show("El número de teléfono ya existe en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Verifica si la contraseña cumple con los requisitos

            string password = DocPasswordTb.Text;

            if (password.Length > 8)
            {
                MessageBox.Show("La contraseña no puede tener más de 8 caracteres.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DocPasswordTb.Text = password.Substring(0, 8); // Limita la longitud a 8 caracteres.
                DocPasswordTb.SelectionStart = DocPasswordTb.Text.Length; // Establece el cursor al final del texto.
                return;
            }
            else if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            {
                MessageBox.Show("La contraseña debe contener al menos una letra y un número.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand("insert into DoctorTbl(DocName, DOCDOB, DOCGEN, DOCSPEC, DOCEXP, DOCPHONE, DocAdd, DocPass)values(@DN, @DD, @DG, @DS, @DE, @DP, @DA, @DPA)", Con);
                cmd.Parameters.AddWithValue("@DN", name);
                cmd.Parameters.AddWithValue("@DD", DOcDOB.Value.Date);
                cmd.Parameters.AddWithValue("@DG", DocGenCb.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@DS", DocSpecCb.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@DE", DocExpTb.Text);
                cmd.Parameters.AddWithValue("@DP", DocPhoneTb.Text);
                cmd.Parameters.AddWithValue("@DA", DocAddCb.Text);
                cmd.Parameters.AddWithValue("@DPA", DocPasswordTb.Text);

                if (DocPhoneTb.Text.Length < 9)
                {
                    MessageBox.Show("El número de teléfono debe tener al menos 9 dígitos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Sale de la función si el número es inválido.
                }

                cmd.ExecuteNonQuery();
                MessageBox.Show("Doctor Agregado", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Error al agregar el doctor: " + Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Con.Close();
                DisplayDoc();
                Clear();
            }
        }

        private bool PhoneNumberExists(string phoneNumber)
        {
            try
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM DoctorTbl WHERE DocPhone = @PhoneNumber", Con))
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

        private void DoctorsDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DNameTb.Text = DoctorsDGV.SelectedRows[0].Cells[1].Value.ToString();
            DOcDOB.Text = DoctorsDGV.SelectedRows[0].Cells[2].Value.ToString();
            DocGenCb.SelectedItem = DoctorsDGV.SelectedRows[0].Cells[3].Value.ToString();
            DocSpecCb.SelectedItem = DoctorsDGV.SelectedRows[0].Cells[4].Value.ToString();
            DocExpTb.Text = DoctorsDGV.SelectedRows[0].Cells[5].Value.ToString();
            DocPhoneTb.Text = DoctorsDGV.SelectedRows[0].Cells[6].Value.ToString();
            DocAddCb.Text = DoctorsDGV.SelectedRows[0].Cells[7].Value.ToString(); 
            DocPasswordTb.Text = DoctorsDGV.SelectedRows[0].Cells[8].Value.ToString();
            if (DNameTb.Text == "")
            {
                Key = 0;
            }
            else
            {
                Key = Convert.ToInt32(DoctorsDGV.SelectedRows[0].Cells[0].Value.ToString());
            }
        }


        private void EditBtn_Click(object sender, EventArgs e)
        {
            if (DNameTb.Text.Length <= 2)
            {
                MessageBox.Show("El nombre debe contener al menos 3 letras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (DocPasswordTb.Text == "" || DocPhoneTb.Text == "" || DocAddCb.Text == "" || DocGenCb.SelectedIndex == -1)
                {
                    MessageBox.Show("Falta información.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (DocPhoneTb.Text.Length < 9)
                {
                    MessageBox.Show("El número de teléfono debe tener al menos 9 dígitos.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (DocPhoneTb.Text[0] != '9')
                {
                    MessageBox.Show("El número de teléfono debe comenzar con '9'.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (PhoneNumberExistsForOtherDoctor(DocPhoneTb.Text, Key))
                {
                    MessageBox.Show("El número de teléfono ya está asociado a otro doctor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        Con.Open();
                        SqlCommand cmd = new SqlCommand("Update DoctorTbl set DocName=@DN, DOCDOB=@DD, DOCGEN=@DG, DOCSPEC=@DS, DOCEXP=@DE, DOCPHONE=@DP, DocAdd=@DA, DocPass=@DPA where DoctorId=@DKey", Con);
                        cmd.Parameters.AddWithValue("@DN", DNameTb.Text);
                        cmd.Parameters.AddWithValue("@DD", DOcDOB.Value.Date);
                        cmd.Parameters.AddWithValue("@DG", DocGenCb.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@DS", DocSpecCb.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@DE", DocExpTb.Text);
                        cmd.Parameters.AddWithValue("@DP", DocPhoneTb.Text);
                        cmd.Parameters.AddWithValue("@DA", DocAddCb.Text);
                        cmd.Parameters.AddWithValue("@DPA", DocPasswordTb.Text);
                        cmd.Parameters.AddWithValue("@DKey", Key);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Doctor Edited", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                    }
                    finally
                    {
                        Con.Close();
                        DisplayDoc();
                        Clear();
                    }
                }
            }
        }

        private bool PhoneNumberExistsForOtherDoctor(string phoneNumber, int currentDoctorKey)
        {
            try
            {
                Con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM DoctorTbl WHERE DocPhone = @PhoneNumber AND DocId <> @CurrentDoctorKey", Con))
                {
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmd.Parameters.AddWithValue("@CurrentDoctortKey", currentDoctorKey);
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

        private void Doctors_Load(object sender, EventArgs e)
        {
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
        }

        private void label14_Click(object sender, EventArgs e)
        {
            Form1 Obj = new Form1();
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

        private void PatLbl_Click(object sender, EventArgs e)
        {
            Patients Obj = new Patients();
            Obj.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Homes Obj = new Homes();
            Obj.Show();
            this.Hide();
        }

        private void DOcDOB_onValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = DOcDOB.Value;

            // Establece la fecha mínima como el 1 de enero de 1960
            DateTime minDate = new DateTime(1960, 1, 1);

            // Establece la fecha máxima como el 31 de diciembre del año 2000
            DateTime maxDate = new DateTime(2000, 12, 31);

            if (selectedDate < minDate || selectedDate > maxDate)
            {
                if (selectedDate > maxDate)
                {
                    MessageBox.Show("La fecha seleccionada no puede ser posterior al año 2000.", "Error de fecha", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione una fecha válida (desde 1960 hasta el año 2000).", "Error de fecha", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                DOcDOB.Value = maxDate; // Restablece la fecha a la fecha máxima permitida
            }
        }

        private void DocPhoneTb_TextChanged(object sender, EventArgs e)
        {

            if (DocPhoneTb.Text.Length > 0 && DocPhoneTb.Text[0] != '9')
            {
                MessageBox.Show("El número de teléfono debe comenzar con '9'.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DocPhoneTb_KeyPress(object sender, KeyPressEventArgs e)
        {

            // Verifica si el carácter ingresado es un número o la tecla de retroceso (backspace).
            // Verifica si el carácter ingresado es un número o la tecla de retroceso (backspace).
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true; // Cancela la entrada del carácter si no es un número ni backspace.
            }
            else if (DocPhoneTb.Text.Length == 0 && e.KeyChar != '9')
            {
                e.Handled = true; // Cancela la entrada si el primer dígito no es '9'.
            }
        }



        private void DocExpTb_TextChanged(object sender, EventArgs e)
        {


            if (DocExpTb.Text == "Años")
            {
                DocExpTb.ForeColor = SystemColors.GrayText; // Cambia el color del texto a gris.
            }
            else
            {
                DocExpTb.ForeColor = SystemColors.WindowText; // Cambia el color del texto a negro.
            }
            // Verifica si el contenido del TextBox es "Años" y establece el color del texto a negro.
            if (DocExpTb.Text == "Años")
            {
                DocExpTb.ForeColor = SystemColors.WindowText;
            }

            // Verifica si el contenido del TextBox es un número válido.
            if (!string.IsNullOrEmpty(DocExpTb.Text) && !int.TryParse(DocExpTb.Text, out int years))
            {
                // Si no es un número válido, elimina el último carácter.
                DocExpTb.Text = DocExpTb.Text.Substring(0, DocExpTb.Text.Length - 1);
                DocExpTb.SelectionStart = DocExpTb.Text.Length;
                return;
            }

            // Limita el valor a estar entre 0 y 50.
            if (!string.IsNullOrEmpty(DocExpTb.Text))
            {
                int yearsValue = int.Parse(DocExpTb.Text);
                if (yearsValue < 0)
                {
                    DocExpTb.Text = "0";
                }
                else if (yearsValue > 50)
                {
                    MessageBox.Show("El valor debe estar entre 0 y 50.", "Valor fuera de rango", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DocExpTb.Text = "50"; // Restablece el valor a 50 en caso de ser mayor a 50.
                }
            }

            // Coloca el cursor al final del texto.
            DocExpTb.SelectionStart = DocExpTb.Text.Length;
        }

        private Point offset;
        private void Doctors_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offset = new Point(e.X, e.Y);
            }
        }

        private void Doctors_MouseMove(object sender, MouseEventArgs e)
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

        private void DNameTb_TextChanged(object sender, EventArgs e)
        {
            string input = DNameTb.Text;

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
                    MessageBox.Show("El nombre debe contener al menos 3 letras o espacios en blanco.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DNameTb.Text = ""; // Limpia el campo para que el usuario pueda corregirlo.
                }
            }
        }

        private void DoctorLbl_Click(object sender, EventArgs e)
        {

        }

        private void DocGenCb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DocGenCb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void DocSpecCb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DocSpecCb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void DocGenCb_Enter(object sender, EventArgs e)
        {
  
        }

        private void DocExpTb_Click(object sender, EventArgs e)
        {
            DocExpTb.Text = "";
        }

        private string textoPredefinido = "Años";


        private void DocExpTb_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DocExpTb.Text))
            {
                DocExpTb.Text = textoPredefinido;
            }
        }

        private void DocPasswordTb_TextChanged(object sender, EventArgs e)
        {
            string password = DocPasswordTb.Text;

            if (password.Length > 8)
            {
                MessageBox.Show("La contraseña no puede tener más de 8 caracteres.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DocPasswordTb.Text = password.Substring(0, 8); // Limita la longitud a 8 caracteres.
                DocPasswordTb.SelectionStart = DocPasswordTb.Text.Length; // Establece el cursor al final del texto.
                return;
            }
            else if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            {
                
            }
        }


    }
}