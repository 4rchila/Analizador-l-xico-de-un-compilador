using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class LenguajeDefinido
{
    //CUANDO SE IDENTIFICAN LOS TOKENS SE VERIFICA SI EST[AN EN ALGUNA DE LAS LISTAS ESTATICAS (.Contains(palabra))
    public static readonly List<string> PalabrasReservadas = new List<string>()
    {
        "entero",
        "decimal",
        "booleano",
        "cadena",
        "si",
        "sino",
        "mientras",
        "hacer",
        "verdadero",
        "falso",
        "o",
        "y",
        "nulo",
        "romper",
        "devolver",
        "intentar",
        "atrapar",
        "imprimir",
        "entrada",
        "para",
        "en",
        "importar",
        "publico",
        "privado",
        "clase",
        "estatica",
        "privada",
        "protegida",
        "nuevo",
        "caracter",
        "continuar",
        "esto",
    };
    public static readonly List<string> Operador = new List<string>()
    {
        "+",
        "-",
        "*",
        "/",
        "%",
        "=",
        "==",
        "!=",
        "<",
        ">",
        "<=",
        ">=",
        "(",
        ")",
        "{",
        "}",
        "'",
        ":",
        ";"
    };
    //LAS EXPRESIONES REGULARES SE VALIDAN CON .IsMatch(palabra))
    //EXPRESION REGULAR PARA NUMEROS ENTEROS
    public static readonly Regex NumeroEntero = new Regex(@"^\d+$"); //"\d" es un dígito (0-9) | + es una o más repeticiones | "$" fin de la cadena
    //EXPRESION REGULAR PARA NUMEROS DECIMALES
    public static readonly Regex NumeroDecimal = new Regex(@"^\d+\.\d+$"); //"d\+" es uno o mas digitos | "\." punto | "\d+" uno o mas digitos despues del punto
    //E.R PARA VARIABLES
    public static readonly Regex Identificador = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$"); //"[a-zA-Z_]" el primer caracter debe ser una letra mayuscula o minuscula, o gui[on bajo | "[a-zA-Z0-9_]*" es cero o más caracteres que sean letras, dígitos o guion bajo.  

}