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

namespace BibliotecaDB
{
    public partial class Form1 : Form
    {
        SqlConnection conexionDB = new SqlConnection("Data Source = localhost; Initial Catalog = biblioteca; Integrated Security = True");
        string idEditar = "";
        int cont = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conexionDB.Open();
            SqlCommand comando = new SqlCommand("SELECT * FROM libros;", conexionDB);
            SqlDataAdapter adaptador = new SqlDataAdapter();
            adaptador.SelectCommand = comando;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dataGridView1.DataSource = tabla;
        }

        public bool comprobarISBN(string isbn)
        {
            int nFilas = dataGridView1.Rows.Count - 1;
            bool repetido = false;

            for (int i = 0; i < nFilas; i++)
            {
                DataGridViewRow fila = dataGridView1.Rows[i];
                if (fila.Cells[6].Value.ToString() == isbn)
                {
                    repetido = true;
                    break;
                }
            }

            return repetido;
        }

        public bool todoNumeros(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text;
            string autor = txtAutor.Text;
            string isbn = txtIsbn.Text;
            
            if (todoNumeros(isbn))
            {
                if (txtIsbn.Text.Length <= 13)
                {
                    if (!comprobarISBN(txtIsbn.Text))
                    {
                        if (nombre != "" && autor != "")
                        {
                            SqlCommand comando = new SqlCommand("INSERT INTO libros VALUES ('" + nombre + "', '" + autor + "', '" + isbn + "');", conexionDB);
                            int r = comando.ExecuteNonQuery();
                            SqlCommand comand = new SqlCommand("SELECT * FROM libros;", conexionDB);
                            SqlDataAdapter adaptador = new SqlDataAdapter();
                            adaptador.SelectCommand = comand;
                            DataTable tabla = new DataTable();
                            adaptador.Fill(tabla);
                            dataGridView1.DataSource = tabla;
                            txtNombre.Text = "";
                            txtAutor.Text = "";
                            txtIsbn.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Tienes que rellenar todos los campos", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No puedes repetir el campo 'isbn'", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("El campo 'isbn' puedes tener un longitud maxima de 13 numeros", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Solo puedes introducir elementos numéricos en el campo ISBN", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (todoNumeros(txtIsbn.Text))
            {
                if (txtNombre.Text != "" && txtAutor.Text != "" && txtIsbn.Text != "")
                {
                    string query = "DELETE FROM libros WHERE nombre = '" + txtNombre.Text + "' AND autor ='" + txtAutor.Text + "' AND isbn = '" + txtIsbn.Text + "'";
                    SqlCommand comando = new SqlCommand(query, conexionDB);
                    int r = comando.ExecuteNonQuery();
                    txtNombre.Text = "";
                    txtAutor.Text = "";
                    txtIsbn.Text = "";
                    SqlCommand comand = new SqlCommand("SELECT * FROM libros;", conexionDB);
                    SqlDataAdapter adaptador = new SqlDataAdapter();
                    adaptador.SelectCommand = comand;
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    dataGridView1.DataSource = tabla;
                }
                else
                {
                    MessageBox.Show("Tienes que rellenar todos los campos", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Solo puedes introducir campos numéricos en el ISBN", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            SqlCommand comand = new SqlCommand("SELECT * FROM libros;", conexionDB);
            SqlDataAdapter adaptador = new SqlDataAdapter();
            adaptador.SelectCommand = comand;
            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            dataGridView1.DataSource = tabla;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];
                //int row = e.RowIndex;
                string idBorrar = fila.Cells[2].Value.ToString();
                SqlCommand comando = new SqlCommand("DELETE FROM libros WHERE id = '" + idBorrar + "'", conexionDB);
                int r = comando.ExecuteNonQuery();
                SqlCommand comand = new SqlCommand("SELECT * FROM libros;", conexionDB);
                SqlDataAdapter adaptador = new SqlDataAdapter();
                adaptador.SelectCommand = comand;
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dataGridView1.DataSource = tabla;
            }
            if (e.ColumnIndex == 1)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];
                string id = fila.Cells[2].Value.ToString();
                string query = "SELECT nombre FROM libros WHERE id = '" + id + "'";
                SqlCommand comando = new SqlCommand(query, conexionDB);
                string nombre = comando.ExecuteScalar().ToString();
                SqlCommand comand = new SqlCommand("SELECT autor FROM libros WHERE id = '" + id + "'", conexionDB);
                string autor = comand.ExecuteScalar().ToString();
                SqlCommand comando2 = new SqlCommand("SELECT isbn FROM libros WHERE id = '" + id + "'", conexionDB);
                string isbn = comando2.ExecuteScalar().ToString();
                idEditar = id;
                txtNombre.Text = nombre;
                txtAutor.Text = autor;
                txtIsbn.Text = isbn;
                cont += 1;
            }
            if (e.ColumnIndex == 2)
            {
                DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];
                string libroIsbn = fila.Cells[6].Value.ToString();
                string fechaActual = DateTime.Now.ToString("f");
                MessageBox.Show(fechaActual);

                //DESHABILITAR LOS FORMULARIOS//
                label1.Text = "Nombre Cliente";
                txtIsbn.Enabled = false;
                txtAutor.Enabled = false;
                txtIsbn.Visible = false;
                txtAutor.Visible = false;
                btnAdd.Visible = false;
                btnAdd.Enabled = false;
                btnUpdate.Visible = false;
                btnUpdate.Enabled = false;
                btnRefresh.Visible = false;
                btnRefresh.Enabled = false;
                btnDelete.Visible = false;
                btnDelete.Enabled = false;
                label2.Visible = false;
                label3.Visible = false;
                btnSend.Visible = true;
                btnSend.Enabled = true;
                ////////////////////////////////
               
                SqlCommand comando = new SqlCommand("INSERT INTO registros VALUES ('' ,'" + libroIsbn + "', '" + fechaActual + "', '" + fechaActual + "', 'Roberto' )");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            //HAY QUE COMPROBAR SI TIENE LETRAS DENTRO//
            if (todoNumeros(txtIsbn.Text))
            {
                if (cont >= 1)
                {
                    if (txtNombre.Text != "" && txtAutor.Text != "" && txtIsbn.Text != "")
                    {
                        string query = "UPDATE libros SET nombre = '" + txtNombre.Text + "', autor = '" + txtAutor.Text + "', isbn = '" + txtIsbn.Text + "' WHERE id = '" + idEditar + "';";
                        SqlCommand comando = new SqlCommand(query, conexionDB);
                        int r = comando.ExecuteNonQuery();
                        SqlCommand comand = new SqlCommand("SELECT * FROM libros;", conexionDB);
                        SqlDataAdapter adaptador = new SqlDataAdapter();
                        adaptador.SelectCommand = comand;
                        DataTable tabla = new DataTable();
                        adaptador.Fill(tabla);
                        dataGridView1.DataSource = tabla;
                    }
                    else
                    {
                        MessageBox.Show("Tienes que rellenar todos los campos", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Tienes que hacer click en la tabla para seleccionar el campo", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Solo puedes introducir elementos numéricos en el campo ISBN", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
