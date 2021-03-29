using System;
using HexAdder;

Console.WriteLine("Введите первое шестнадцатеричное число:");
var a = Console.ReadLine();
Console.WriteLine("Введите второе шестнадцатеричное число:");
var b = Console.ReadLine();
var sum = a.Length > b.Length ? Sum(a, b) : Sum(b, a);
Console.WriteLine(sum);
Console.ReadKey();


static string Sum(string a, string b)
{
    if (a is null or { Length: 0 }) throw new ArgumentException(nameof(a));
    if (b is null or { Length: 0 }) throw new ArgumentException(nameof(b));
    int lenght = a.Length;
    b = b.PadLeft(lenght, '0');
    int transfer = 0;
    string result = string.Empty;
    for (int i = lenght - 1; i >= 0; i--)
    {
        int sum = a[i].HexCharToInt() + b[i].HexCharToInt() + transfer;
        result += (sum % 16).IntToHexChar();
        transfer = sum / 16;
    }
    if (transfer != 0)
    {
        result += transfer.IntToHexChar();
    }
    return result.Reverse();
}