using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Mediapipe.HandTracking.ARCore {
    public class ARCoreHandInput : InputManager {

        [SerializeField]
        private ARCameraManager camera_manager = null;

        public unsafe override FrameInput GetFrameInput() {
            XRCpuImage image;
            if (!camera_manager.TryGetLatestImage(out image)) {

              return null;
            }

            var conversion_params = new XRCpuImage.ConversionParams
            {
                // Get the entire image
                inputRect = new RectInt(0, 0, image.width, image.height),

                // Downsample by 2
                outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

                // Choose RGBA format
                outputFormat = TextureFormat.RGBA32,

                // Flip across the vertical axis (mirror image)
                transformation = XRCpuImage.Transformation.MirrorY
            };

            int size = image.GetConvertedDataSize(conversion_params);

            var buffer = new NativeArray<byte>(size, Allocator.Temp);
            image.Convert(conversion_params, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

            Texture2D m_texture = new Texture2D(conversion_params.outputDimensions.x, conversion_params.outputDimensions.y, conversion_params.outputFormat, false);

            m_texture.LoadRawTextureData(buffer);
            m_texture.Apply();
            buffer.Dispose();

            byte[] frame_image = ImageConversion.EncodeToJPG(m_texture);

            sbyte[] frame_image_signed = Array.ConvertAll(frame_image, b => unchecked((sbyte)b));

            FrameInput frame_input = new FrameInput(image.width, image.height, frame_image_signed);

            image.Dispose();
            Destroy(m_texture);

            return frame_input;
        }
    }
}