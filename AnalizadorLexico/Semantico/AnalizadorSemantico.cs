using System;
using System.Collections.Generic;
using AnalizadorLexico.Lexico;

namespace AnalizadorLexico.Semantico
{
    public class AnalizadorSemantico
    {
        private readonly List<Token> tokens;
        private readonly TablaSimbolos tablaSimbolos;
        private int indiceActual;
        private Token tokenActual;

        public List<string> Errores { get; } = new List<string>();

        public AnalizadorSemantico(List<Token> tokens)
        {
            this.tokens = tokens;
            this.tablaSimbolos = new TablaSimbolos();
            this.indiceActual = 0;
            if (tokens.Count > 0)
                tokenActual = tokens[0];
        }

        public void Analizar()
        {
            while (indiceActual < tokens.Count && tokenActual != null)
            {
                if (EsTipoDato(tokenActual.Lexema))
                {
                    AnalizarDeclaracion();
                }
                else if (tokenActual.Tipo == TokenType.Identificador)
                {
                    AnalizarAsignacion();
                }
                else
                {
                    Avanzar();
                }
            }
        }

        private void AnalizarDeclaracion()
        {
            string tipoVariable = tokenActual.Lexema;
            Avanzar();

            // Si no hay más tokens, salir
            if (tokenActual == null)
            {
                Errores.Add($"Error: Declaración incompleta después de '{tipoVariable}'");
                return;
            }

            if (tokenActual.Tipo != TokenType.Identificador)
            {
                Errores.Add($"Error: Se esperaba identificador después de '{tipoVariable}' (Línea {tokenActual.Linea})");
                // IMPORTANTE: Avanzar para evitar bucle infinito
                Avanzar();
                return;
            }

            string nombreVariable = tokenActual.Lexema;
            
            // Registrar variable en tabla de símbolos
            try
            {
                tablaSimbolos.DeclararVariable(nombreVariable, tipoVariable, tokenActual.Linea, tokenActual.Columna);
            }
            catch (Exception ex)
            {
                Errores.Add(ex.Message);
            }
            
            Avanzar();

            // Verificar asignación si existe
            if (tokenActual != null && tokenActual.Tipo == TokenType.OperadorAsignacion)
            {
                Avanzar(); // Consumir "="
                
                // Verificar que hay tokens después del "="
                if (tokenActual == null)
                {
                    Errores.Add($"Error: Se esperaba expresión después de '=' (Línea {tokens[indiceActual-1].Linea})");
                    return;
                }
                
                string tipoExpresion = ObtenerTipoExpresion();
                
                if (!SonTiposCompatibles(tipoVariable, tipoExpresion))
                {
                    Errores.Add($"Error semántico: No se puede asignar {tipoExpresion} a '{nombreVariable}' de tipo {tipoVariable} (Línea {tokenActual?.Linea ?? tokens[tokens.Count-1].Linea})");
                }
            }
        }

        private void AnalizarAsignacion()
        {
            string nombreVariable = tokenActual.Lexema;
            var simbolo = tablaSimbolos.ObtenerSimbolo(nombreVariable);

            if (simbolo == null)
            {
                Errores.Add($"Error: Variable '{nombreVariable}' no declarada (Línea {tokenActual.Linea})");
                Avanzar(); //Avanzar para evitar bucle infinito
                return;
            }

            Avanzar(); // Consumir identificador

            if (tokenActual != null && tokenActual.Tipo == TokenType.OperadorAsignacion)
            {
                Avanzar(); // Consumir "="
                
                // Verificar que hay tokens después del "="
                if (tokenActual == null)
                {
                    Errores.Add($"Error: Se esperaba expresión después de '=' (Línea {tokens[indiceActual-1].Linea})");
                    return;
                }
                
                string tipoExpresion = ObtenerTipoExpresion();
                
                if (!SonTiposCompatibles(simbolo.Tipo, tipoExpresion))
                {
                    Errores.Add($"Error semántico: No se puede asignar {tipoExpresion} a '{nombreVariable}' de tipo {simbolo.Tipo} (Línea {tokenActual?.Linea ?? tokens[tokens.Count-1].Linea})");
                }
            }
        }

        private string ObtenerTipoExpresion()
        {
            if (indiceActual >= tokens.Count || tokenActual == null) 
                return "desconocido";

            var token = tokenActual;

            switch (token.Tipo)
            {
                case TokenType.Entero:
                    Avanzar();
                    return "entero";
                case TokenType.Decimal:
                    Avanzar();
                    return "decimal";
                case TokenType.Cadena:
                    Avanzar();
                    return "cadena";
                case TokenType.Caracter:
                    Avanzar();
                    return "caracter";
                case TokenType.Booleano:
                    Avanzar();
                    return "booleano";
                case TokenType.Identificador:
                    var simbolo = tablaSimbolos.ObtenerSimbolo(token.Lexema);
                    Avanzar();
                    return simbolo?.Tipo ?? "desconocido";
                default:
                    // Si no es un tipo reconocido, avanzar y retornar desconocido
                    Avanzar();
                    return "desconocido";
            }
        }

        private bool SonTiposCompatibles(string tipoVariable, string tipoExpresion)
        {
            if (tipoExpresion == "desconocido") 
                return true; // No verificar si no se pudo determinar
            
            var compatibilidad = new Dictionary<string, List<string>>
            {
                { "entero", new List<string> { "entero", "decimal" } },
                { "decimal", new List<string> { "entero", "decimal" } },
                { "cadena", new List<string> { "cadena" } },
                { "caracter", new List<string> { "caracter" } },
                { "booleano", new List<string> { "booleano" } }
            };

            return compatibilidad.ContainsKey(tipoVariable) && 
                   compatibilidad[tipoVariable].Contains(tipoExpresion);
        }

        private bool EsTipoDato(string lexema)
        {
            return lexema == "entero" || lexema == "Entero" || 
            lexema == "decimal" || lexema == "Decimal" || 
            lexema == "booleano" || lexema == "Booleano" || 
            lexema == "cadena" || lexema == "Cadena" || 
            lexema == "caracter" || lexema == "Caracter";
        }

        private void Avanzar()
        {
            indiceActual++;
            tokenActual = indiceActual < tokens.Count ? tokens[indiceActual] : null;
        }
    }
}