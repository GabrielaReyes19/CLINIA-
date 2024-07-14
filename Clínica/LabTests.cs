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
    public partial class LabTests : Form
    {
        SqlConnection Con = new SqlConnection(Configuracion.CadenaConexion);
        int Key = 0;

        public LabTests()
        {
            InitializeComponent();
            DisplayTest();
            LabTestCb.SelectedIndexChanged += LabTestCb_SelectedIndexChanged;
        }

        private void DisplayTest()
        {
            try
            {
                Con.Open();
                string Query = "Select * from TestTbl";
                SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                var ds = new DataSet();
                sda.Fill(ds);
                LabTestDGV.DataSource = ds.Tables[0];
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
            LabTestCb.Text = "";
            LabCostCb.Text = "";
            Key = 0;
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (LabCostCb.Text == "" || LabTestCb.Text == "")
            {
                MessageBox.Show("Missing Information");
            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("insert into TestTbl(TestName, TestCost) values (@TN, @TC)", Con);
                    cmd.Parameters.AddWithValue("@TN", LabTestCb.Text);
                    cmd.Parameters.AddWithValue("@TC", LabCostCb.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Test Added");
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
                finally
                {
                    if (Con.State == ConnectionState.Open)
                    {
                        Con.Close();
                    }
                }
                DisplayTest();
                Clear();
            }
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            if (LabCostCb.Text == "" || LabTestCb.Text == "")
            {
                MessageBox.Show("Missing Information");
            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("Update TestTbl set TestName = @TN, TestCost = @TC where TestNum=@TKey", Con);
                    cmd.Parameters.AddWithValue("@TN", LabTestCb.Text);
                    cmd.Parameters.AddWithValue("@TC", LabCostCb.Text);
                    cmd.Parameters.AddWithValue("@TKey", Key);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Test Updated");
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
                finally
                {
                    if (Con.State == ConnectionState.Open)
                    {
                        Con.Close();
                    }
                }
                DisplayTest();
                Clear();
            }
        }

        private void DelBtn_Click(object sender, EventArgs e)
        {
            if (Key == 0)
            {
                MessageBox.Show("Select The Lab Test");
            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("Delete from TestTbl where TestNum=@TKey", Con);
                    cmd.Parameters.AddWithValue("@TKey", Key);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Lab Test Deleted");
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
                finally
                {
                    if (Con.State == ConnectionState.Open)
                    {
                        Con.Close();
                    }
                }
                DisplayTest();
                Clear();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Homes Obj = new Homes();
            Obj.Show();
            this.Hide();
        }

        private void LabTestDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            LabTestCb.Text = LabTestDGV.SelectedRows[0].Cells[1].Value.ToString();
            LabCostCb.Text = LabTestDGV.SelectedRows[0].Cells[2].Value.ToString();
            if (LabTestCb.Text == "")
            {
                Key = 0;
            }
            else
            {
                Key = Convert.ToInt32(LabTestDGV.SelectedRows[0].Cells[0].Value.ToString());
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {
            Patients Obj = new Patients();
            Obj.Show();
            this.Hide();
        }

        private void LabTests_Load(object sender, EventArgs e)
        {

        }

        private Point offset;
        private void LabTests_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                offset = new Point(e.X, e.Y);
            }
        }

        private void LabTests_MouseMove(object sender, MouseEventArgs e)
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


        private void LabTestCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtén el valor seleccionado en LabTestCb
            string selectedTest = LabTestCb.SelectedItem?.ToString();

            // Establece el valor predeterminado en LabCostCb según el valor seleccionado en LabTestCb
            switch (selectedTest)
            {
                case "Examen de sangre":
                    LabCostCb.Text = "40";
                    break;
                case "Examen de orina":
                    LabCostCb.Text = "30";
                    break;
                case "Pruebas de hepatitis B y C":
                    LabCostCb.Text = "75";
                    break;
                case "Pruebas de ETS":
                    LabCostCb.Text = "120";
                    break;
                case "Prueba de ARN del VIH":
                    LabCostCb.Text = "80";
                    break;
                case "Prueba de anticuerpos del VIH":
                    LabCostCb.Text = "50";
                    break;
                case "Pruebas de resistencia a los antirretrovirales":
                    LabCostCb.Text = "65";
                    break;
                default:
                    // Si no hay una coincidencia específica, puedes dejar LabCostCb en blanco o establecer otro valor predeterminado.
                    LabCostCb.Text = "";
                    break;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
