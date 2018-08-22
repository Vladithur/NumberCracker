using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Threading;

namespace NumberFactorisation
{
    static class Program
    {
        static void Main(string[] args)
        {
            int byteLength = 1;
            // Generate first number
            BigInteger factor1 = getRandomPrime(byteLength);
            Console.WriteLine("Factor one: " + factor1);

            // Wait to avoid the same result
            Thread.Sleep(1000);

            // Generate seccond number
            BigInteger factor2 = getRandomPrime(byteLength);
            Console.WriteLine("Factor two: " + factor2);

            // Multiply
            BigInteger bigInteger = factor1 * factor2;
            Console.WriteLine("Result: " + bigInteger);

            // Crack!
            BigInteger[] factors = Factorise(bigInteger, 2, 2, 10, byteLength);

            // Print
            Console.WriteLine();
            Console.WriteLine("Factors: ");
            foreach (BigInteger factor in factors)
            {
                Console.WriteLine(factor);
            }

            // Wait for user to exit
            Console.WriteLine();
            Console.WriteLine("Wait");
            Thread.Sleep(3000);
            Console.WriteLine("Type yes to Exit");

            string line = "";
            do { line = Console.ReadLine(); }
            while (line != "yes");
        }

        static BigInteger[] Factorise(BigInteger number, UInt64 minLayers, UInt64 maxLayers, UInt64 numPPerLayer, int byteLength)
        {
            var squareRoot = Sqrt(number);
            List<Particle> particles = createParticles(minLayers, numPPerLayer, byteLength);
            return new BigInteger[] { };
        }

        static List<Particle> createParticles(UInt64 layersLeft, UInt64 pLayer, int byteLength)
        {
            List<Particle> particles = new List<Particle>((int)Math.Pow(pLayer, layersLeft + 1));
            if (layersLeft == 0)
            {
                for (UInt64 i = 0; i < pLayer; i++)
                {
                    Particle p = new Particle();
                    p.number = getRandom(byteLength);
                    particles.Append(p);
                }
            }
            else
            {
                for (UInt64 i = 0; i < pLayer; i++)
                {
                    var ps = createParticles(layersLeft - 1, pLayer, byteLength);
                    particles.AddRange(ps);
                }
            }

            return particles;
        }

        public class Particle
        {
            public BigInteger number;

            public Particle() { }
        }

        private static BigInteger Sqrt(this BigInteger n)
        {
            if (n == 0) return 0;
            if (n > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!isSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }
            else return -1;
        }

        private static Boolean isSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }

        public static BigInteger getRandom(int byteLength)
        {
            Random random = new Random();
            byte[] data = new byte[byteLength];
            random.NextBytes(data);
            return new BigInteger(data);
        }

        public static BigInteger getRandomPrime(int byteLength)
        {
            BigInteger num = new BigInteger();
            do
            {
                Random random = new Random();
                byte[] data = new byte[byteLength];
                random.NextBytes(data);
                num = new BigInteger(data);
            }
            while (!PrimeExtensions.IsProbablyPrime(num, (int)Math.Pow(2, byteLength)));
            return num;
        }
    }
    public static class PrimeExtensions
    {
        // Random generator (thread safe)
        private static ThreadLocal<Random> s_Gen = new ThreadLocal<Random>(
          () =>
          {
              return new Random();
          }
        );

        // Random generator (thread safe)
        private static Random Gen
        {
            get
            {
                return s_Gen.Value;
            }
        }

        public static Boolean IsProbablyPrime(this BigInteger value, int witnesses = 10)
        {
            if (value <= 1)
                return false;

            if (witnesses <= 0)
                witnesses = 10;

            BigInteger d = value - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            Byte[] bytes = new Byte[value.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < witnesses; i++)
            {
                do
                {
                    Gen.NextBytes(bytes);

                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= value - 2);

                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);

                    if (x == 1)
                        return false;
                    if (x == value - 1)
                        break;
                }

                if (x != value - 1)
                    return false;
            }

            return true;
        }
    }
}
