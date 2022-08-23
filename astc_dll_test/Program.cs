using System;
using System.Runtime.InteropServices;

namespace astc_dll_test
{
    class Program
    {
        /// <summary>
        /// example 테스트용
        /// </summary>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        [DllImport("..\\..\\..\\..\\out\\build\\x64-Debug\\astc_lib\\astcenc_library.dll")]
        public static extern void main_lib1(int argc, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AutoArrayMarshaler))] byte[][] argv);


        /// <summary>
        /// 실제 CLI 소스 테스트용
        /// </summary>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        [DllImport("..\\..\\..\\..\\out\\build\\x64-Debug\\astc_lib\\astcenc_library2.dll")]
        public static extern void main_lib2(int argc, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AutoArrayMarshaler))] byte[][] argv);


        static void Main(string[] args)
        {
            // \astc-encoder\astc_dll_test\bin\Debug\netcoreapp3.1
            string envdir = Environment.CurrentDirectory;


            string compsrc = $"{envdir}\\images\\test.png";              // 원본 이미지 경로 
            string compdest = $"{envdir}\\images\\test.astc";            // 결과 astc 경로

            // compress 실행
            byte[][] test = new byte[6][]
            {
                System.Text.Encoding.UTF8.GetBytes(""),
                System.Text.Encoding.UTF8.GetBytes("-cl"),
                System.Text.Encoding.UTF8.GetBytes(compsrc),
                System.Text.Encoding.UTF8.GetBytes(compdest),
                System.Text.Encoding.UTF8.GetBytes("12x12"),
                System.Text.Encoding.UTF8.GetBytes("-medium")

            };
            main_lib2(test.Length, test);

            string decompsrc = $"{envdir}\\images\\test.astc";              // 원본 이미지 경로 
            string decompdest = $"{envdir}\\images\\test_re.png";            // 결과 astc 경로

            // decompress 실행
            byte[][] test2 = new byte[4][]
            {
                System.Text.Encoding.UTF8.GetBytes(""),
                System.Text.Encoding.UTF8.GetBytes("-dl"),
                System.Text.Encoding.UTF8.GetBytes(decompsrc),
                System.Text.Encoding.UTF8.GetBytes(decompdest),

            };

            main_lib2(test2.Length, test2);
        }
    }

    /// <summary>
    /// 관리되지 않는 메모리 할당/복사/변환 (unmanaged memory block)
    /// IntPtr 포인터 
    /// </summary>
    class AutoArrayMarshaler : ICustomMarshaler
    {
        static ICustomMarshaler GetInstance(string cookie)
        {
            return new AutoArrayMarshaler();
        }
        GCHandle[] handles; // 관리되지 않는 메모리에서 관리되는 개체에 액세스
        GCHandle buffer;    // 버퍼 
        Array[] array;
        public void CleanUpManagedData(object ManagedObj)
        {
        }
        public void CleanUpNativeData(IntPtr pNativeData)
        {
            buffer.Free();
            foreach (GCHandle handle in handles)
            {
                handle.Free();
            }
        }
        public int GetNativeDataSize()
        {
            return IntPtr.Size;
        }
        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            array = (Array[])ManagedObj;
            handles = new GCHandle[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                handles[i] = GCHandle.Alloc(array[i], GCHandleType.Pinned);
            }
            IntPtr[] pointers = new IntPtr[handles.Length];
            for (int i = 0; i < handles.Length; i++)
            {
                pointers[i] = handles[i].AddrOfPinnedObject();
            }
            buffer = GCHandle.Alloc(pointers, GCHandleType.Pinned);
            return buffer.AddrOfPinnedObject();
        }
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return array;
        }
    }
}

