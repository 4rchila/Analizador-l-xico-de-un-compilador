using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AnalizadorLexico.Lexico;
using AnalizadorLexico.Semantico;

namespace AnalizadorLexico
{
    public partial class MainForm : Form
    {
        private string archivoSeleccionado = "";
        private List<TokenResult> tokensEncontrados = new List<TokenResult>();
        private List<ErrorLexico> erroresEncontrados = new List<ErrorLexico>();
        private List<string> erroresSemanticos = new List<string>();

        public MainForm()
        {
            InitializeComponent();
            ConfigurarInterfaz();
        }

        private void ConfigurarInterfaz()
        {
            dgvResultados.AutoGenerateColumns = false;

            if (dgvResultados.Columns.Count == 0)
            {
                dgvResultados.Columns.Add("Token", "TOKEN");
                dgvResultados.Columns.Add("Tipo", "TIPO");
                dgvResultados.Columns.Add("Cantidad", "CANTIDAD");
                dgvResultados.Columns[0].Width = 150;
                dgvResultados.Columns[1].Width = 150;
                dgvResultados.Columns[2].Width = 100;
            }

            dgvResultados.BackgroundColor = Color.White;
            dgvResultados.GridColor = Color.LightGray;
            dgvResultados.DefaultCellStyle.SelectionBackColor = Color.LightBlue;

            txtContenido.ReadOnly = true;
            txtContenido.BackColor = Color.White;
            txtContenido.Font = new Font("Consolas", 10);

            txtResultados.ReadOnly = true;
            txtResultados.BackColor = Color.White;
            txtResultados.Font = new Font("Consolas", 9);

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

            dgvResultados.Rows.Clear();
            txtResultados.Clear();
            tokensEncontrados.Clear();
            erroresEncontrados.Clear();
            erroresSemanticos.Clear();

            txtResultados.Text = "Analizando archivo...\r\n";
            Application.DoEvents();

            try
            {
                RealizarAnalisisLexico();
                MostrarResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el análisis: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RealizarAnalisisLexico()
        {
            string contenido = File.ReadAllText(archivoSeleccionado, Encoding.UTF8);

            Analizador analizador = new Analizador();
            List<Token> tokens = analizador.Analizar(contenido);

            StringBuilder log = new StringBuilder();
            log.AppendLine("=== INICIANDO ANÁLISIS LÉXICO ===\r\n");

            Dictionary<string, int> contadorTokens = new Dictionary<string, int>();

            foreach (var token in tokens)
            {
                log.AppendLine($"Línea {token.Linea}, Col {token.Columna}: '{token.Lexema}' -> {token.Tipo}");

                string clave = $"{token.Lexema}|{token.Tipo}";
                if (contadorTokens.ContainsKey(clave)) contadorTokens[clave]++;
                else contadorTokens[clave] = 1;

                if (token.Tipo == TokenType.Error)
                {
                    erroresEncontrados.Add(new ErrorLexico(token.Lexema, token.Linea));
                }
            }

            foreach (var kvp in contadorTokens)
            {
                string[] partes = kvp.Key.Split('|');
                string lexema = partes[0];
                string tipo = partes.Length > 1 ? partes[1] : "Desconocido";
                tokensEncontrados.Add(new TokenResult(lexema, tipo, kvp.Value));
            }

            log.AppendLine("\r\n=== ANÁLISIS LÉXICO COMPLETADO ===");
            
            if (erroresEncontrados.Count == 0)
            {
                log.AppendLine("\r\n=== INICIANDO ANÁLISIS SEMÁNTICO ===");
                RealizarAnalisisSemantico(tokens, log);
            }

            txtResultados.Text = log.ToString();
        }

        private void RealizarAnalisisSemantico(List<Token> tokens, StringBuilder log)
        {
            try
            {
                var analizadorSemantico = new AnalizadorSemantico(tokens);
                analizadorSemantico.Analizar();

                foreach (var error in analizadorSemantico.Errores)
                {
                    erroresSemanticos.Add(error);
                }

                if (analizadorSemantico.Errores.Count == 0)
                {
                    log.AppendLine("✅ Análisis semántico completado sin errores.");
                }
                else
                {
                    log.AppendLine($"❌ Se encontraron {analizadorSemantico.Errores.Count} errores semánticos:");
                    foreach (var error in analizadorSemantico.Errores)
                    {
                        log.AppendLine($"   - {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.AppendLine($"⚠️ Error durante análisis semántico: {ex.Message}");
                erroresSemanticos.Add($"Error en análisis semántico: {ex.Message}");
            }
        }

        private void MostrarResultados()
        {
            if (erroresEncontrados.Count > 0)
            {
                StringBuilder errores = new StringBuilder();
                errores.AppendLine("\r\n=== ERRORES LÉXICOS ENCONTRADOS ===");
                foreach (var error in erroresEncontrados)
                {
                    errores.AppendLine($"Línea {error.NumeroLinea}: Token '{error.Token}' no reconocido");
                }
                txtResultados.Text += errores.ToString();

                MessageBox.Show($"Se encontraron {erroresEncontrados.Count} errores léxicos.",
                    "Errores Encontrados", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (erroresSemanticos.Count > 0)
            {
                StringBuilder erroresSem = new StringBuilder();
                erroresSem.AppendLine("\r\n=== ERRORES SEMÁNTICOS ENCONTRADOS ===");
                foreach (var error in erroresSemanticos)
                {
                    erroresSem.AppendLine(error);
                }
                txtResultados.Text += erroresSem.ToString();

                if (erroresEncontrados.Count > 0)
                {
                    MessageBox.Show($"Se encontraron {erroresEncontrados.Count} errores léxicos y {erroresSemanticos.Count} errores semánticos.",
                        "Errores Encontrados", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Se encontraron {erroresSemanticos.Count} errores semánticos.",
                        "Errores Semánticos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            foreach (var token in tokensEncontrados.OrderBy(t => t.Tipo).ThenBy(t => t.Token))
            {
                dgvResultados.Rows.Add(token.Token, token.Tipo, token.Cantidad);
            }

            StringBuilder resumen = new StringBuilder();
            resumen.AppendLine($"\r\n=== RESUMEN DEL ANÁLISIS ===");
            resumen.AppendLine($"Total de tokens únicos encontrados: {tokensEncontrados.Count}");
            resumen.AppendLine($"Total de tokens procesados: {tokensEncontrados.Sum(t => t.Cantidad)}");
            resumen.AppendLine($"Errores léxicos: {erroresEncontrados.Count}");
            resumen.AppendLine($"Errores semánticos: {erroresSemanticos.Count}");

            var tiposAgrupados = tokensEncontrados.GroupBy(t => t.Tipo);
            foreach (var grupo in tiposAgrupados)
            {
                resumen.AppendLine($"{grupo.Key}: {grupo.Sum(t => t.Cantidad)} tokens");
            }

            txtResultados.Text += resumen.ToString();

            if (erroresEncontrados.Count == 0 && erroresSemanticos.Count == 0)
            {
                MessageBox.Show("Análisis completado exitosamente sin errores.", "Análisis Exitoso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
            erroresEncontrados.Clear();
            erroresSemanticos.Clear();
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

            if (erroresEncontrados.Count > 0)
            {
                csv.AppendLine();
                csv.AppendLine("ERRORES LÉXICOS:");
                csv.AppendLine("TOKEN,LÍNEA");
                foreach (var error in erroresEncontrados)
                {
                    csv.AppendLine($"{error.Token},{error.NumeroLinea}");
                }
            }

            if (erroresSemanticos.Count > 0)
            {
                csv.AppendLine();
                csv.AppendLine("ERRORES SEMÁNTICOS:");
                csv.AppendLine("MENSAJE");
                foreach (var error in erroresSemanticos)
                {
                    string mensaje = error.Replace("\"", "\"\"");
                    csv.AppendLine($"\"{mensaje}\"");
                }
            }

            File.WriteAllText(archivo, csv.ToString(), Encoding.UTF8);
        }
    }

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

    public class ErrorLexico
    {
        public string Token { get; set; }
        public int NumeroLinea { get; set; }

        public ErrorLexico(string token, int numeroLinea)
        {
            Token = token;
            NumeroLinea = numeroLinea;
        }
    }
}