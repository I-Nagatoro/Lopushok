using System;

namespace ClassLibraryMaterial
{
    public static class Calculation
    {
        public static int CalculateMaterial(int productType, int materialType, int count, float width, float length)
        {
            if (count <= 0 || width <= 0 || length <= 0)
                return -1;

            if (!TryGetProductCoefficient(productType, out double productCoefficient))
                return -1;

            if (!TryGetMaterialDefectRate(materialType, out double defectRate))
                return -1;

            double area = width * length;

            double totalMaterial = count * area * productCoefficient;

            double totalMaterialWithDefect = totalMaterial / (1 - defectRate);

            return (int)Math.Ceiling(totalMaterialWithDefect);
        }

        private static bool TryGetProductCoefficient(int productType, out double coefficient)
        {
            switch (productType)
            {
                case 1:
                    coefficient = 1.1;
                    return true;
                case 2:
                    coefficient = 2.5;
                    return true;
                case 3:
                    coefficient = 8.43;
                    return true;
                default:
                    coefficient = 0;
                    return false;
            }
        }

        private static bool TryGetMaterialDefectRate(int materialType, out double defectRate)
        {
            switch (materialType)
            {
                case 1:
                    defectRate = 0.003; 
                    return true;
                case 2:
                    defectRate = 0.0012; 
                    return true;
                default:
                    defectRate = 0;
                    return false;
            }
        }
    }
}