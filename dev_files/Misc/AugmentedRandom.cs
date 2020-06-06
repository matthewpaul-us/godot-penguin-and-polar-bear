using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

    public class AugmentedRandom : Random
    {
        public string Seed { get; private set; }

        public AugmentedRandom()
            :base() { }

        public AugmentedRandom(string seed)
            :base(seed.GetHashCode())
        {
            Seed = seed;
        }

        public T Random<T>(IList<T> items)
        {
            if (items.Count == 0)
                throw new Exception("List contains no items!");

            var index = Next(items.Count);

            return items[index];
        }

        // https://stackoverflow.com/questions/273313/randomize-a-listt
        public void Shuffle<T>(IList<T> list)
        {
            var items = list.Count();
            while (items > 1)
            {
                items--;
                var randomIndex = Next(items + 1);
                T value = list[randomIndex];
                list[randomIndex] = list[items];
                list[items] = value;
            }
        }

    public Vector2 Vector(float componentFromZero)
    {
        return new Vector2((float)NextDouble() * -componentFromZero, (float)NextDouble() * componentFromZero);
    }

    public int Sign()
    {
        return NextDouble() > 0.5 ? 1 : -1;
    }

    public float Binomial()
    {
        return (float)(NextDouble() - NextDouble());
    }
}
