using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models.Extensions {
    internal static class BinaryReaderExtensions {

        internal static Vector2 ReadVector2([NotNull] this BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();

            return new Vector2(x, y);
        }

        internal static Vector3 ReadVector3([NotNull] this BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();

            return new Vector3(x, y, z);
        }

        internal static Vector4 ReadVector4([NotNull] this BinaryReader reader) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();

            return new Vector4(x, y, z, w);
        }

        [NotNull]
        internal static byte[] ReadNullTermStringBytes([NotNull] this BinaryReader reader) {
            var streamLength = reader.BaseStream.Length;
            var buffer = new List<byte>(128);

            while (reader.BaseStream.Position < streamLength) {
                var b = reader.ReadByte();

                if (b == 0) {
                    break;
                }

                buffer.Add(b);
            }

            return buffer.ToArray();
        }

        [NotNull]
        internal static string ReadPmxStringUtf16([NotNull] this BinaryReader reader) {
            var bytes = ReadNullTermStringBytes(reader);

            return Encoding.Unicode.GetString(bytes);
        }

        [NotNull]
        internal static string ReadPmxStringUtf8([NotNull] this BinaryReader reader) {
            var bytes = ReadNullTermStringBytes(reader);

            return Encoding.UTF8.GetString(bytes);
        }

        internal static void Skip([NotNull] this BinaryReader reader, int count) {
            reader.ReadBytes(count);
        }

        internal static T Read<T>([NotNull] this BinaryReader reader)
            where T : struct {
            var structSize = Marshal.SizeOf(typeof(T));
            var buffer = reader.ReadBytes(structSize);

            var ptr = Marshal.AllocHGlobal(structSize);

            Marshal.Copy(buffer, 0, ptr, structSize);

            var result = (T)Marshal.PtrToStructure(ptr, typeof(T));

            Marshal.FreeHGlobal(ptr);

            return result;
        }

        // TODO: Not fully compatible: https://gist.github.com/felixjones/f8a06bd48f9da9a4539f#index-types
        internal static int ReadVarLenIntAsInt32([NotNull] this BinaryReader reader, int size, bool unsignedUnderUInt16 = false) {
            switch (size) {
                case 1: {
                        var value = reader.ReadByte();

                        if (unsignedUnderUInt16) {
                            return value;
                        } else {
                            if (value == unchecked((byte)-1)) {
                                return -1;
                            } else {
                                return value;
                            }
                        }
                    }
                case 2: {
                        var value = reader.ReadUInt16();

                        if (unsignedUnderUInt16) {
                            return value;
                        } else {
                            if (value == unchecked((ushort)-1)) {
                                return -1;
                            } else {
                                return value;
                            }
                        }
                    }
                case 4:
                    return reader.ReadInt32();
                default:
                    return 0;
            }
        }

    }
}
