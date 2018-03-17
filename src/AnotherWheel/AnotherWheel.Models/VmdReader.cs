using System;
using System.IO;
using System.Text;
using AnotherWheel.Models.Extensions;
using AnotherWheel.Models.Vmd;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace AnotherWheel.Models {
    // https://github.com/oguna/MMDFormats/blob/master/MikuMikuFormats/Vmd.h
    public sealed class VmdReader {

        private VmdReader([NotNull] BinaryReader reader) {
            _reader = reader;
        }

        public static VmdMotion ReadMotion([NotNull] Stream stream) {
            VmdMotion motion;

            using (var binaryReader = new BinaryReader(stream)) {
                var vmdReader = new VmdReader(binaryReader);
                motion = vmdReader.ReadMotion();
            }

            return motion;
        }

        private VmdMotion ReadMotion() {
            var signature = ReadString(20);

            if (signature != "Vocaloid Motion Data") {
                throw new FormatException("VMD signature is not found.");
            }

            var motion = new VmdMotion();

            var formatVersionString = ReadString(10);

            motion.Version = Convert.ToInt32(formatVersionString);
            motion.ModelName = ReadString(20);

            ReadBoneFrames();
            ReadFacialFrames();
            ReadCameraFrames();
            ReadLightFrames();

            // Unknown 2
            _reader.ReadBytes(4);

            if (_reader.BaseStream.Position != _reader.BaseStream.Length) {
                ReadIKFrames();
            }

            if (_reader.BaseStream.Position != _reader.BaseStream.Length) {
                throw new FormatException("The VMD file may contain other data that this reader does not recognize.");
            }

            return motion;

            void ReadBoneFrames() {
                var frameCount = _reader.ReadInt32();
                var frames = new VmdBoneFrame[frameCount];

                for (var i = 0; i < frameCount; ++i) {
                    frames[i] = ReadBoneFrame();
                }

                motion.BoneFrames = frames;
            }

            void ReadFacialFrames() {
                var frameCount = _reader.ReadInt32();
                var frames = new VmdFacialFrame[frameCount];

                for (var i = 0; i < frameCount; ++i) {
                    frames[i] = ReadFacialFrame();
                }

                motion.FacialFrames = frames;
            }

            void ReadCameraFrames() {
                var frameCount = _reader.ReadInt32();
                var frames = new VmdCameraFrame[frameCount];

                for (var i = 0; i < frameCount; ++i) {
                    frames[i] = ReadCameraFrame();
                }

                motion.CameraFrames = frames;
            }

            void ReadLightFrames() {
                var frameCount = _reader.ReadInt32();
                var frames = new VmdLightFrame[frameCount];

                for (var i = 0; i < frameCount; ++i) {
                    frames[i] = ReadLightFrame();
                }

                motion.LightFrames = frames;
            }

            void ReadIKFrames() {
                var frameCount = _reader.ReadInt32();
                var frames = new VmdIKFrame[frameCount];

                for (var i = 0; i < frameCount; ++i) {
                    frames[i] = ReadIKFrame();
                }

                motion.IKFrames = frames;
            }
        }

        private VmdBoneFrame ReadBoneFrame() {
            var frame = new VmdBoneFrame();

            frame.Name = ReadString(15);
            frame.FrameIndex = _reader.ReadInt32();
            frame.Position = _reader.ReadVector3();
            frame.Rotation = new Quaternion(_reader.ReadVector4());

            ReadMultiDimArray(frame.Interpolation);

            return frame;
        }

        private VmdFacialFrame ReadFacialFrame() {
            var frame = new VmdFacialFrame();

            frame.FacialExpressionName = ReadString(15);
            frame.FrameIndex = _reader.ReadInt32();
            frame.Weight = _reader.ReadSingle();

            return frame;
        }

        private VmdCameraFrame ReadCameraFrame() {
            var frame = new VmdCameraFrame();

            frame.FrameIndex = _reader.ReadInt32();
            frame.Distance = _reader.ReadSingle();
            frame.Position = _reader.ReadVector3();
            frame.Orientation = _reader.ReadVector3();

            ReadMultiDimArray(frame.Interpolation);

            frame.FieldOfView = _reader.ReadSingle();

            ReadMultiDimArray(frame.Unknown);

            return frame;
        }

        private VmdLightFrame ReadLightFrame() {
            var frame = new VmdLightFrame();

            frame.FrameIndex = _reader.ReadInt32();
            frame.Color = _reader.ReadVector3();
            frame.Position = _reader.ReadVector3();

            return frame;
        }

        private IKControl ReadIKControl() {
            var control = new IKControl();

            control.Name = ReadString(20);
            control.Enabled = _reader.ReadBoolean();

            return control;
        }

        private VmdIKFrame ReadIKFrame() {
            var frame = new VmdIKFrame();

            frame.FrameIndex = _reader.ReadInt32();
            frame.Visible = _reader.ReadBoolean();

            var ikCount = _reader.ReadInt32();
            var iks = new IKControl[ikCount];

            for (var i = 0; i < ikCount; ++i) {
                iks[i] = ReadIKControl();
            }

            frame.IKControls = iks;

            return frame;
        }

        [NotNull]
        private string ReadString(int length) {
            var bytes = _reader.ReadBytes(length);
            var str = ShiftJis.GetString(bytes);

            str = str.TrimEnd(NullTermChars);

            return str;
        }

        private void ReadMultiDimArray([NotNull] Array array) {
            var tmp = _reader.ReadBytes(array.Length);
            Buffer.BlockCopy(tmp, 0, array, 0, array.Length);
        }

        private static readonly char[] NullTermChars = { '\0' };

        private static readonly Encoding ShiftJis = Encoding.GetEncoding("Shift-JIS");

        private readonly BinaryReader _reader;

    }
}



