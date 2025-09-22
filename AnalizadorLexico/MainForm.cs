using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalizadorLexico
{
    public partial class MainForm : Form
    {
        private string archivoSeleccionado = "";
        private List<TokenResult> tokensEncontrados = new List<TokenResult>();

        public MainForm()
        {
            InitializeComponent();
            ConfigurarInterfaz();
        }

        private void ConfigurarInterfaz()
        {
            // Configurar el DataGridView
            dgvResultados.AutoGenerateColumns = false;
            dgvResultados.Columns.Add("Token", "TOKEN");
            dgvResultados.Columns.Add("Tipo", "TIPO");
            dgvResultados.Columns.Add("Cantidad", "CANTIDAD");
            
            dgvResultados.Columns[0].Width = 150;
            dgvResultados.Columns[1].Width = 150;
            dgvResultados.Columns[2].Width = 100;

            // Configurar colores y estilos
            dgvResultados.BackgroundColor = Color.White;
            dgvResultados.GridColor = Color.LightGray;
            dgvResultados.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            
            txtContenido.ReadOnly = true;
            txtContenido.BackColor = Color.White;
            txtContenido.Font = new Font("Consolas", 10);
            
            txtResultados.ReadOnly = true;
            txtResultados.BackColor = Color.White;
            txtResultados.Font = new Font("Consolas", 9);

            // Configurar labels
            lblArchivo.Text = "Ningún archivo seleccionado";
            lblArchivo.ForeColor = Color.Gray;
        }

        private void btnSeleccionarArchivo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                openFileDialog.Title = "Seleccionar archivo para analizar";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    archivoSeleccionado = openFileDialog.FileName;
                    lblArchivo.Text = Path.GetFileName(archivoSeleccionado);
                    lblArchivo.ForeColor = Color.Black;
                    
                    CargarContenidoArchivo();
                    btnAnalizar.Enabled = true;
                }
            }
        }

        private void CargarContenidoArchivo()
        {
            try
            {
                string contenido = File.ReadAllText(archivoSeleccionado, Encoding.UTF8);
                txtContenido.Text = contenido;
                
                // Agregar números de línea
                string[] lineas = contenido.Split('\n');
                StringBuilder contenidoConLineas = new StringBuilder();
                
                for (int i = 0; i < lineas.Length; i++)
                {
                    contenidoConLineas.AppendLine($"{i + 1:D3}: {lineas[i]}");
                }
                
                txtContenido.Text = contenidoConLineas.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el archivo: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnalizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(archivoSeleccionado))
            {
                MessageBox.Show("Por favor selecciona un archivo primero.", "Advertencia", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Limpiar resultados anteriores
            dgvResultados.Rows.Clear();
            txtResultados.Clear();
            tokensEncontrados.Clear();

            // Mostrar mensaje de procesamiento
            txtResultados.Text = "Analizando archivo...\r\n";
            Application.DoEvents();

            try
            {
                // Aquí llamarás a tu clase Analizador
                // Por ahora simulo el proceso
                SimularAnalisisLexico();
                
                MostrarResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el análisis: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SimularAnalisisLexico()
        {
            // Esta función será reemplazada por tu lógica real
            // Aquí solo simulo algunos resultados para mostrar la interfaz
            
            string contenido = File.ReadAllText(archivoSeleccionado);
            string[] lineas = contenido.Split('\n');
            
            StringBuilder log = new StringBuilder();
            log.AppendLine("=== INICIANDO ANÁLISIS LÉXICO ===\r\n");
            
            for (int i = 0; i < lineas.Length; i++)
            {
                log.AppendLine($"Analizando línea {i + 1}: {lineas[i].Trim()}");
                
                // Aquí integrarás tu lógica de análisis
                // Por ejemplo: var tokens = analizador.AnalizarLinea(lineas[i]);
            }
            
            log.AppendLine("\r\n=== ANÁLISIS COMPLETADO ===");
            txtResultados.Text = log.ToString();
            
            // Datos de ejemplo - reemplazar con resultados reales
            tokensEncontrados.Add(new TokenResult("entero", "Palabra Reservada", 2));
            tokensEncontrados.Add(new TokenResult("numero1", "Identificador", 1));
            tokensEncontrados.Add(new TokenResult("+", "Operador", 3));
            tokensEncontrados.Add(new TokenResult("si", "Palabra Reservada", 1));
            tokensEncontrados.Add(new TokenResult("==", "Operador", 2));
        }

        private void MostrarResultados()
        {
            // Mostrar en DataGridView
            foreach (var token in tokensEncontrados)
            {
                dgvResultados.Rows.Add(token.Token, token.Tipo, token.Cantidad);
            }

            // Mostrar resumen
            StringBuilder resumen = new StringBuilder();
            resumen.AppendLine($"\r\n=== RESUMEN DEL ANÁLISIS ===");
            resumen.AppendLine($"Total de tokens únicos encontrados: {tokensEncontrados.Count}");
            resumen.AppendLine($"Total de tokens procesados: {tokensEncontrados.Sum(t => t.Cantidad)}");
            
            var tiposAgrupados = tokensEncontrados.GroupBy(t => t.Tipo);
            foreach (var grupo in tiposAgrupados)
            {
                resumen.AppendLine($"{grupo.Key}: {grupo.Sum(t => t.Cantidad)} tokens");
            }

            txtResultados.Text += resumen.ToString();

            // Mostrar mensaje de éxito
            MessageBox.Show("Análisis completado exitosamente.", "Información", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtContenido.Clear();
            dgvResultados.Rows.Clear();
            txtResultados.Clear();
            lblArchivo.Text = "Ningún archivo seleccionado";
            lblArchivo.ForeColor = Color.Gray;
            archivoSeleccionado = "";
            btnAnalizar.Enabled = false;
            tokensEncontrados.Clear();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (tokensEncontrados.Count == 0)
            {
                MessageBox.Show("No hay resultados para exportar.", "Advertencia", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Archivos CSV (*.csv)|*.csv|Archivos de texto (*.txt)|*.txt";
                saveFileDialog.Title = "Exportar resultados";
                saveFileDialog.FileName = "resultados_analisis.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportarResultados(saveFileDialog.FileName);
                        MessageBox.Show("Resultados exportados exitosamente.", "Información", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al exportar: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportarResultados(string archivo)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("TOKEN,TIPO,CANTIDAD");
            
            foreach (var token in tokensEncontrados)
            {
                csv.AppendLine($"{token.Token},{token.Tipo},{token.Cantidad}");
            }
            
            File.WriteAllText(archivo, csv.ToString(), Encoding.UTF8);
        }
    }

    // Clase auxiliar para almacenar resultados
    public class TokenResult
    {
        public string Token { get; set; }
        public string Tipo { get; set; }
        public int Cantidad { get; set; }

        public TokenResult(string token, string tipo, int cantidad)
        {
            Token = token;
            Tipo = tipo;
            Cantidad = cantidad;
        }
    }
}

