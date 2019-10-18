#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("VD0cQAUayljSpZ9rrPT/+b5JAPkKVhTm7XA7zfW5kWSNfGQh8KGO5U7quaWp6dtPIQ0iJCMUtDvlwrvvu9gYL3lGG5trrFHdw6S+rh8dVeT7Scrp+8bNwuFNg008xsrKys7LyIkfgWdVVJPA0ZPh72TkTkpoiZTD/icxVmoxLSCQ+0GTJwRI+RJgmhDOdrkiCsSO5hmME0REB/o2LmbKcknKxMv7ScrByUnKyst58oBhSb0klWzh6pO1pb8gCpyXDSWuYrNMWCdeBrtRLmEpk/06BTaprCvK4KCTYyuZiuaGtf96fnfBZSz6MlkxANs367X/vXiSH/LuvDYaq9H5gLQ2tT++R4fIobBysTLt/rFWTNtUHTz+XJqNUQ9io+DcksnIysvK");
        private static int[] order = new int[] { 13,8,9,5,13,10,10,10,8,11,13,13,12,13,14 };
        private static int key = 203;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
