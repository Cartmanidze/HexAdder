using System;
using System.Linq;
using System.Linq.Expressions;


Console.WriteLine("Введите первое шестнадцатеричное число:");
var a = Console.ReadLine();
Console.WriteLine("Введите второе шестнадцатеричное число:");
var b = Console.ReadLine();
if (a is null or { Length: 0 }) throw new ArgumentException(nameof(a));
if (b is null or { Length: 0 }) throw new ArgumentException(nameof(b));
var sumFunc = ExpressionSum();
var sum = a.Length > b.Length ? sumFunc(a.ToLower(), b.ToLower()) : sumFunc(b.ToLower(), a.ToLower());
Console.WriteLine(sum);
Console.ReadKey();

static Func<string, string, string> ExpressionSum()
{
    var a = Expression.Parameter(typeof(string), "a");
    var b = Expression.Parameter(typeof(string), "b");
    var map = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
    var mapExpression = Expression.Constant(map);
    var length = Expression.MakeMemberAccess(a, typeof(string).GetProperty("Length") ?? throw new InvalidOperationException());
    var padLeftMethod = typeof(string).GetMethod("PadLeft", new[] { typeof(int), typeof(char) });
    var padLeftResult = Expression.Call(b, padLeftMethod ?? throw new InvalidOperationException(), length, Expression.Constant('0', typeof(char)));
    var toCharArrayMethod = typeof(string).GetMethod("ToCharArray", new Type[] { });
    var aCharArray = Expression.Call(a, toCharArrayMethod ?? throw new InvalidOperationException());
    var bCharArray = Expression.Call(b, toCharArrayMethod);
    var indexOfMethod = typeof(Array).GetMethods().FirstOrDefault(m => m.Name == "IndexOf" && m.GetParameters().Length == 2 && m.IsGenericMethod)?.MakeGenericMethod(typeof(char));
    var concatMethod = typeof(string).GetMethod("Concat", 0, new[] { typeof(object), typeof(object) });
    var arrayReverseMethod = typeof(Array).GetMethod("Reverse", 0, new[] { typeof(char[]) });
    var i = Expression.Variable(typeof(int), "i");
    var sum = Expression.Variable(typeof(int), "sum");
    var transfer = Expression.Variable(typeof(int), "transfer");
    var result = Expression.Variable(typeof(string), "result");
    var resultAsCharArray = Expression.Variable(typeof(char[]), "resultAsCharArray");
    var breakLabel = Expression.Label("break");
    var loop = Expression.Loop(
    Expression.Block(
        Expression.IfThenElse(
            Expression.GreaterThanOrEqual(i, Expression.Constant(0)),
            Expression.Block(
                Expression.Assign(sum, Expression.Add(
                    Expression.Add(
                        Expression.Call(indexOfMethod!, mapExpression, Expression.ArrayIndex(aCharArray, i)),
                        Expression.Call(indexOfMethod, mapExpression, Expression.ArrayIndex(bCharArray, i))),
                    transfer)),
                Expression.Assign(result, Expression.Call(concatMethod!, result,
                Expression.Convert(
                Expression.ArrayIndex(mapExpression,
                    Expression.Modulo(sum, Expression.Constant(16))), typeof(object)))),
                Expression.PreDecrementAssign(i)),
            Expression.Break(breakLabel))), breakLabel);
    var block = Expression.Block(
        new[] { transfer, result, resultAsCharArray, i, sum },
        Expression.Assign(transfer, Expression.Constant(0)),
        Expression.Assign(result, Expression.Constant("", typeof(string))),
        Expression.Assign(i, Expression.Subtract(length, Expression.Constant(1))),
        Expression.Assign(b, padLeftResult),
        loop,
        Expression.Block(
            Expression.IfThen(Expression.NotEqual(transfer, Expression.Constant(0)), Expression.Assign(result,
                    Expression.Call(concatMethod!, result,
                Expression.Convert(
                    Expression.ArrayIndex(mapExpression,
                        Expression.Modulo(sum, Expression.Constant(16))), typeof(object))))),
            Expression.Assign(resultAsCharArray, Expression.Call(result, toCharArrayMethod)),
            Expression.Call(arrayReverseMethod!, resultAsCharArray),
            Expression.Assign(result, Expression.New(typeof(string).GetConstructor(new[] { typeof(char[]) })!, resultAsCharArray))),
        result);
    return Expression.Lambda<Func<string, string, string>>(block, a, b).Compile();
}

static string Sum(string a, string b)
{
    var map = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
    int length = a.Length;
    b = b.PadLeft(length, '0');
    int transfer = 0;
    string result = string.Empty;
    for (int i = length - 1; i >= 0; i--)
    {
        int sum = Array.IndexOf(map, a[i]) + Array.IndexOf(map, b[i]) + transfer;
        result = string.Concat(result, map[sum % 16]);
        transfer = sum / 16;
    }
    if (transfer != 0)
    {
        result += map[transfer];
    }
    char[] arr = result.ToCharArray();
    Array.Reverse(arr);
    return new string(arr);
}
