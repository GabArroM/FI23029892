
int SumForA(int n)
{
    return n * (n + 1) / 2;
}

int SumForD(int n)
{
    return (int)((-1 + Math.Sqrt(1 + 8 * n)) / 2);
}
//Profe una disculpa se que especifico que se solo con numero entero pero para 
// invertir la formula no encontre por el momento otra forma de realizarlo.

int SumIteA(int n)
{
    int resultado = 0;
    for (int i = 1; i <= n; i++)
    {
        resultado += i;
    }

    return resultado;

}

int SumIteD(int n)
{
    int sum = 0;
    while (n  > 0)
    {
        sum++;
        n = n - sum;
    }

    return sum;

}

Console.WriteLine("SumForA fórmula ascendente: " + SumForA(46340));

Console.WriteLine("SumForD fórmula descendente: " + SumForD(268435455));

Console.WriteLine("SumIteA iterativa ascendente: " + SumIteA(65535));

Console.WriteLine("SumIteD iterativa descendente: " + SumIteD(2147483647));


