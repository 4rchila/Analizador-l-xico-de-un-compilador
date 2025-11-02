using System.Collections.Generic;

namespace AnalizadorLexico.Sintactico
{
    /// <summary>
    /// Define las reglas sintácticas válidas del lenguaje.
    /// Aquí se describe la gramática formal que tu analizador reconocerá.
    /// </summary>
    public static class GramaticaDefinida
    {
        // 💡 Puedes usar estas reglas para referencia o mostrar en pantalla.
        public static List<string> Reglas = new List<string>
        {
            "<programa> ::= <declaraciones>",

            "<declaraciones> ::= <declaracion> <declaraciones> | ε",
        
            "<declaracion> ::= <declaracionVariable> | <declaracionFuncion> | <estructuraControl> | <sentenciaControl> | <declaracionClase> | <expresion> \";\"",
        
            // DECLARACIONES DE VARIABLES
            "<declaracionVariable> ::= <tipo> <identificador> <opcionalAsignacion> \";\"",
            "<tipo> ::= \"entero\" | \"decimal\" | \"booleano\" | \"cadena\" | \"caracter\" | \"Entero\" | \"Decimal\" | \"Booleano\" | \"Cadena\" | \"Caracter\"",
            "<opcionalAsignacion> ::= \"=\" <expresion> | ε",
        
            // FUNCIONES
            "<declaracionFuncion> ::= \"funcion\" <identificador> \"(\" <parametros> \")\" \"{\" <declaraciones> \"}\"",
            "<parametros> ::= <tipo> <identificador> <masParametros> | ε",
            "<masParametros> ::= \",\" <tipo> <identificador> <masParametros> | ε",
            "<llamadaFuncion> ::= <identificador> \"(\" <argumentos> \")\"",
            "<argumentos> ::= <expresion> <masArgumentos> | ε",
            "<masArgumentos> ::= \",\" <expresion> <masArgumentos> | ε",
        
            // EXPRESIONES
            "<expresion> ::= <termino> <expresionRestante>",
            "<expresionRestante> ::= <operadorAritmetico> <termino> <expresionRestante> | ε",
            "<termino> ::= <identificador> | <numero> | <cadena> | <caracter> | <booleano> | <llamadaFuncion> | \"(\" <expresion> \")\"",
            "<operadorAritmetico> ::= \"+\" | \"-\" | \"*\" | \"/\" | \"%\"",
            "<numero> ::= <entero> | <decimal>",
            "<entero> ::= \"0\" | \"1\" | \"2\" | ... | \"9\"",
            "<decimal> ::= <entero> \".\" <entero>",
        
            // EXPRESIONES LÓGICAS Y RELACIONALES
            "<expresionLogica> ::= <expresion> <operadorRelacional> <expresion> <operadoresLogicosOpcionales>",
            "<operadorRelacional> ::= \"==\" | \"!=\" | \"<\" | \">\" | \"<=\" | \">=\"",
            "<operadoresLogicosOpcionales> ::= <operadorLogico> <expresionLogica> | ε",
            "<operadorLogico> ::= \"y\" | \"o\" | \"Y\" | \"O\"",
        
            // ESTRUCTURAS DE CONTROL
            "<estructuraControl> ::= <si> | <mientras> | <para> | <tryCatch>",
            "<si> ::= \"si\" \"(\" <expresionLogica> \")\" \"{\" <declaraciones> \"}\" <opcionalSino>",
            "<opcionalSino> ::= \"sino\" \"{\" <declaraciones> \"}\" | ε",
            "<mientras> ::= \"mientras\" \"(\" <expresionLogica> \")\" \"{\" <declaraciones> \"}\"",
            "<para> ::= \"para\" \"(\" <declaracionVariable> <expresionLogica> \";\" <expresion> \")\" \"{\" <declaraciones> \"}\"",
            "<tryCatch> ::= \"intentar\" \"{\" <declaraciones> \"}\" \"atrapar\" \"(\" <identificador> \")\" \"{\" <declaraciones> \"}\"",
        
            // SENTENCIAS DE CONTROL
            "<sentenciaControl> ::= \"romper\" \";\" | \"continuar\" \";\" | \"devolver\" <expresion> \";\" | \"imprimir\" \"(\" <argumentos> \")\" \";\" | \"entrada\" \"(\" \")\" \";\"",
        
            // CLASES Y OBJETOS
            "<declaracionClase> ::= \"clase\" <identificador> \"{\" <miembrosClase> \"}\"",
            "<miembrosClase> ::= <miembro> <miembrosClase> | ε",
            "<miembro> ::= <modificadorAcceso> <tipo> <identificador> <opcionalAsignacion> \";\" | <modificadorAcceso> <declaracionFuncion>",
            "<modificadorAcceso> ::= \"publico\" | \"privado\" | \"protegida\" | \"estatica\" | \"Publico\" | \"Privado\" | \"Protegida\" | \"Estatica\"",
            "<instanciacion> ::= \"nuevo\" <identificador> \"(\" <argumentos> \")\"",
        
            // IDENTIFICADORES Y VALORES
            "<identificador> ::= letra (letra | digito | \"_\")*",
            "<booleano> ::= \"verdadero\" | \"falso\" | \"Verdadero\" | \"Falso\"",
            "<cadena> ::= \"\\\"\" (caracter)* \"\\\"\"",
            "<caracter> ::= \"'\" . \"'\""
        };
    }
}
