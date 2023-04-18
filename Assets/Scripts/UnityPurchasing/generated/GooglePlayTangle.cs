// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("hJ5E0QWkiBLBCRGuHIecn5GZ6Gf4tqlPAVYhVAHNfrgaRO/tLpcf09iXxXT6DcnvmiSM69Lp0sNj82FH02HiwdPu5erJZatlFO7i4uLm4+C8t69jGsAyr3CeL+Q7+Gb1JVe4aiVmBA39FvCcklcr/eN2OPiIfyR5YeLs49Nh4unhYeLi40DPzDLAz5p4jfjuG/RMxFThyu4lOwGzaCHgRqwHkJr0DmKocdc/vMedAozpXhI9+CBqXFib0KSSZ3AGlrKYbwV37MxlkBCrNq9S92S5o5gUDKMBcXmHaXlP0x0ajpV/bUz+gkCWhBs+poZGIP7y4JsGKIR/elUR+LRNPEkXUuuI3kqC39c3csCX75ry2Ht+J8dynAZJ/tpWRpDGUuHg4uPi");
        private static int[] order = new int[] { 9,4,11,9,9,12,9,13,9,12,12,12,13,13,14 };
        private static int key = 227;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
