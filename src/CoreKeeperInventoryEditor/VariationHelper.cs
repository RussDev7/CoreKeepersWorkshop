namespace CoreKeepersWorkshop
{
    internal class VariationHelper
    {
        public static int GetFoodVariation(int ingredient1, int ingredient2)
        {
            return GetPrimaryIngredient(ingredient1, ingredient2) << 16 | GetSecondaryIngredient(ingredient1, ingredient2);
        }

        public static int GetIngredient1FromFoodVariation(int variation)
        {
            return (variation >> 16) & 0xFFFF;
        }

        public static int GetIngredient2FromFoodVariation(int variation)
        {
            return variation & 0xFFFF;
        }

        public static int GetPrimaryIngredient(int ingredient1, int ingredient2)
        {
            if (IngredientShouldBePrimary(ingredient1) && !IngredientShouldBePrimary(ingredient2))
            {
                return ingredient1;
            }

            if (!IngredientShouldBePrimary(ingredient1) && IngredientShouldBePrimary(ingredient2))
            {
                return ingredient2;
            }

            if (FirstIngredientIsPrimary(ingredient1, ingredient2))
            {
                return ingredient1;
            }

            return ingredient2;
        }

        public static int GetSecondaryIngredient(int ingredient1, int ingredient2)
        {
            if (IngredientShouldBePrimary(ingredient1) && !IngredientShouldBePrimary(ingredient2))
            {
                return ingredient1;
            }

            if (!IngredientShouldBePrimary(ingredient1) && IngredientShouldBePrimary(ingredient2))
            {
                return ingredient2;
            }

            if (FirstIngredientIsPrimary(ingredient1, ingredient2))
            {
                return ingredient2;
            }

            return ingredient1;
        }

        private static bool FirstIngredientIsPrimary(int ingredient1, int ingredient2)
        {
            float random = RandomCreateFromIndex((uint)(ingredient1 * 2 + ingredient2 + 87931));
            float random2 = RandomCreateFromIndex((uint)(ingredient2 * 2 + ingredient1 + 87931));
            return random > random2;
        }

        public static bool IngredientShouldBePrimary(int ingredient)
        {
            if (!IngredientIsGoldenPlant(ingredient))
            {
                return ingredient == 9733; // StarlightNautilus.
            }

            return true;
        }

        public static bool IngredientIsGoldenPlant(int ingredient)
        {
            if (ingredient >= 8100 && ingredient <= 8149) // GoldenHeartBerry.
            {
                return true;
            }

            return false;
        }

        // Unity.Mathematics; Seeded pseudo-random number generator based on xorshift.
        // Rewritten by pharuxtan for consolidated use.
        public static float RandomCreateFromIndex(uint n)
        {
            n += 62u;
            n = (n ^ 61u) ^ (n >> 16);
            n *= 9u;
            n ^= (n >> 4);
            n *= 0x27d4eb2du;
            n ^= (n >> 15);
            uint x = (uint)n;
            uint y = (uint)(1812433253 * x + 1);
            uint z = (uint)(1812433253 * y + 1);
            uint w = (uint)(1812433253 * z + 1);
            uint t = x ^ (x << 11);
            uint xs = w ^ (w >> 19) ^ t ^ (t >> 8);
            return ((float)(xs << 9) / 0xFFFFFFFF);
        }
    }
}
