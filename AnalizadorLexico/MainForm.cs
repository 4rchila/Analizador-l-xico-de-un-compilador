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
using System.Text.RegularExpressions;

namespace AnalizadorLexico
{
    public partial class MainForm : Form
    {
        private string archivoSeleccionado = "";
        private List<TokenResult> tokensEncontrados = new List<TokenResult>();
        private List<ErrorLexico> erroresEncontrados = new List<ErrorLexico>();

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
            erroresEncontrados.Clear();

            // Mostrar mensaje de procesamiento
            txtResultados.Text = "Analizando archivo...\r\n";
            Application.DoEvents();

            try
            {
                // <CHANGE> Implementar análisis léxico real usando LenguajeDefinido
                RealizarAnalisisLexico();
                
                MostrarResultados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el análisis: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // <CHANGE> Nueva implementación del análisis léxico usando tu LenguajeDefinido
        private void RealizarAnalisisLexico()
        {
            string contenido = File.ReadAllText(archivoSeleccionado);
            string[] lineas = contenido.Split('\n');
            
            StringBuilder log = new StringBuilder();
            log.AppendLine("=== INICIANDO ANÁLISIS LÉXICO ===\r\n");
            
            Dictionary<string, int> contadorTokens = new Dictionary<string, int>();
            
            for (int numeroLinea = 0; numeroLinea < lineas.Length; numeroLinea++)
            {
                string linea = lineas[numeroLinea].Trim();
                if (string.IsNullOrEmpty(linea)) continue;
                
                log.AppendLine($"Línea {numeroLinea + 1}: {linea}");
                
                // Tokenizar la línea
                List<string> tokens = TokenizarLinea(linea);
                
                foreach (string token in tokens)
                {
                    if (string.IsNullOrWhiteSpace(token)) continue;
                    
                    string tipoToken = ClasificarToken(token);
                    
                    if (tipoToken == "ERROR_LEXICO")
                    {
                        erroresEncontrados.Add(new ErrorLexico(token, numeroLinea + 1));
                        log.AppendLine($"  ❌ ERROR: '{token}' no pertenece al lenguaje");
                    }
                    else
                    {
                        log.AppendLine($"  ✓ '{token}' -> {tipoToken}");
                        
                        // Contar tokens
                        string clave = $"{token}|{tipoToken}";
                        if (contadorTokens.ContainsKey(clave))
                            contadorTokens[clave]++;
                        else
                            contadorTokens[clave] = 1;
                    }
                }
                
                log.AppendLine();
            }
            
            // Convertir contador a lista de resultados
            foreach (var kvp in contadorTokens)
            {
                string[] partes = kvp.Key.Split('|');
                tokensEncontrados.Add(new TokenResult(partes[0], partes[1], kvp.Value));
            }
            
            log.AppendLine("=== ANÁLISIS COMPLETADO ===");
            txtResultados.Text = log.ToString();
        }

        // <CHANGE> Método para tokenizar una línea separando por espacios y operadores
        private List<string> TokenizarLinea(string linea)
        {
            List<string> tokens = new List<string>();
            StringBuilder tokenActual = new StringBuilder();
            
            for (int i = 0; i < linea.Length; i++)
            {
                char caracter = linea[i];
                
                // Si es un espacio, agregar token actual si existe
                if (char.IsWhiteSpace(caracter))
                {
                    if (tokenActual.Length > 0)
                    {
                        tokens.Add(tokenActual.ToString());
                        tokenActual.Clear();
                    }
                }
                // Si es un operador de un solo carácter
                else if (EsOperadorSimple(caracter.ToString()))
                {
                    // Agregar token actual si existe
                    if (tokenActual.Length > 0)
                    {
                        tokens.Add(tokenActual.ToString());
                        tokenActual.Clear();
                    }
                    
                    // Verificar operadores de dos caracteres
                    if (i + 1 < linea.Length)
                    {
                        string operadorDoble = caracter.ToString() + linea[i + 1].ToString();
                        if (LenguajeDefinido.Operador.Contains(operadorDoble))
                        {
                            tokens.Add(operadorDoble);
                            i++; // Saltar el siguiente carácter
                            continue;
                        }
                    }
                    
                    // Agregar operador simple
                    tokens.Add(caracter.ToString());
                }
                else
                {
                    // Agregar carácter al token actual
                    tokenActual.Append(caracter);
                }
            }
            
            // Agregar último token si existe
            if (tokenActual.Length > 0)
            {
                tokens.Add(tokenActual.ToString());
            }
            
            return tokens;
        }

        // <CHANGE> Verificar si es un operador simple
        private bool EsOperadorSimple(string caracter)
        {
            return LenguajeDefinido.Operador.Contains(caracter);
        }

        // <CHANGE> Clasificar token usando tu LenguajeDefinido
        private string ClasificarToken(string token)
        {
            // Verificar palabras reservadas
            if (LenguajeDefinido.PalabrasReservadas.Contains(token))
            {
                return "Palabra Reservada";
            }
            
            // Verificar operadores
            if (LenguajeDefinido.Operador.Contains(token))
            {
                return "Operador";
            }
            
            // Verificar números decimales (debe ir antes que enteros)
            if (LenguajeDefinido.NumeroDecimal.IsMatch(token))
            {
                return "Numero Decimal";
            }
            
            // Verificar números enteros
            if (LenguajeDefinido.NumeroEntero.IsMatch(token))
            {
                return "Numero Entero";
            }
            
            // Verificar identificadores
            if (LenguajeDefinido.Identificador.IsMatch(token))
            {
                return "Identificador";
            }
            
            // Si no coincide con ningún patrón, es un error léxico
            return "ERROR_LEXICO";
        }

        private void MostrarResultados()
        {
            // Mostrar errores si existen
            if (erroresEncontrados.Count > 0)
            {
                StringBuilder errores = new StringBuilder();
                errores.AppendLine("\r\n=== ERRORES LÉXICOS ENCONTRADOS ===");
                foreach (var error in erroresEncontrados)
                {
                    errores.AppendLine($"Línea {error.NumeroLinea}: Token '{error.Token}' no reconocido");
                }
                txtResultados.Text += errores.ToString();
                
                MessageBox.Show($"Se encontraron {erroresEncontrados.Count} errores léxicos. Revisa el log para más detalles.", 
                    "Errores Encontrados", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            // Mostrar tokens en DataGridView
            foreach (var token in tokensEncontrados.OrderBy(t => t.Tipo).ThenBy(t => t.Token))
            {
                dgvResultados.Rows.Add(token.Token, token.Tipo, token.Cantidad);
            }

            // Mostrar resumen
            StringBuilder resumen = new StringBuilder();
            resumen.AppendLine($"\r\n=== RESUMEN DEL ANÁLISIS ===");
            resumen.AppendLine($"Total de tokens únicos encontrados: {tokensEncontrados.Count}");
            resumen.AppendLine($"Total de tokens procesados: {tokensEncontrados.Sum(t => t.Cantidad)}");
            resumen.AppendLine($"Errores léxicos: {erroresEncontrados.Count}");
            
            var tiposAgrupados = tokensEncontrados.GroupBy(t => t.Tipo);
            foreach (var grupo in tiposAgrupados)
            {
                resumen.AppendLine($"{grupo.Key}: {grupo.Sum(t => t.Cantidad)} tokens");
            }

            txtResultados.Text += resumen.ToString();

            // Mostrar mensaje de resultado
            if (erroresEncontrados.Count == 0)
            {
                MessageBox.Show("Análisis completado exitosamente sin errores.", "Análisis Exitoso", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ... existing code ... (métodos btnLimpiar_Click, btnExportar_Click, ExportarResultados permanecen igual)

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
            
            // Agregar errores si existen
            if (erroresEncontrados.Count > 0)
            {
                csv.AppendLine("\nERRORES LÉXICOS:");
                csv.AppendLine("TOKEN,LÍNEA");
                foreach (var error in erroresEncontrados)
                {
                    csv.AppendLine($"{error.Token},{error.NumeroLinea}");
                }
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

    // <CHANGE> Nueva clase para manejar errores léxicos
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

    // <CHANGE> Agregar la clase LenguajeDefinido directamente en el proyecto
    public static class LenguajeDefinido
    {
        public static readonly List<string> PalabrasReservadas = new List<string>()
        {
            "entero", "decimal", "booleano", "cadena", "si", "sino", "mientras", "hacer",
            "verdadero", "falso", "o", "y", "nulo", "romper", "devolver", "intentar",
            "atrapar", "imprimir", "entrada", "para", "en", "importar", "publico",
            "privado", "clase", "estatica", "privada", "protegida", "nuevo", "caracter",
            "continuar", "esto"
        };

        public static readonly List<string> Operador = new List<string>()
        {
            "+", "-", "*", "/", "%", "=", "==", "!=", "<", ">", "<=", ">=",
            "(", ")", "{", "}", "'", ":", ";"
        };

        public static readonly Regex NumeroEntero = new Regex(@"^\d+$");
        public static readonly Regex NumeroDecimal = new Regex(@"^\d+\.\d+$");
        public static readonly Regex Identificador = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
    }
}