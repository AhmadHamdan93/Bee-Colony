

using BeeColony;

// this program solve function {f(x) = x*x} using bee colony 
// data is generated as random between -1 , 1
// the objective funcion is minimum

Random r = new Random();
double[,] a = new double[20,10];


// create random data 
for (int i = 0; i < 20; i++)
{
    double sum = 0.0;
    for(int j = 0; j < 10; j++)
    {
        if (j == 9)
        {
            a[i, j] = sum;
        }
        else
        {
            a[i, j] = Math.Round(r.NextDouble(),6);
            sum += a[i, j] * a[i, j];
        }
    }
}



// call Bee Colony 
var x = new Bee(a);
x.Search();