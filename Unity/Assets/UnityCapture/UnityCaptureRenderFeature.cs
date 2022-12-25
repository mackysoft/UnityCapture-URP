using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MackySoft.UnityCapture {

	public enum TextureFlipFlags {
		None = 0,
		FlipHorizontally = 1,
		FlipVertically = 2,
		FlipHorizontallyAndVertically = FlipHorizontally | FlipVertically,
	}

	[Serializable]
	public class UnityCaptureSettings {

		[SerializeField]
		Vector2Int m_OutputResolusion = new Vector2Int(1920,1080);

		[SerializeField]
		TextureFlipFlags m_FlipFlags = TextureFlipFlags.None;

		[SerializeField]
		ECaptureDevice m_CaptureDevice = ECaptureDevice.CaptureDevice1;

		[SerializeField]
		EResizeMode m_ResizeMode = EResizeMode.Disabled;

		[SerializeField]
		int m_Timeout = 1000;

		[SerializeField]
		bool m_DoubleBuffering = false;

		[SerializeField]
		bool m_HideWarnings = false;

		public Vector2Int OutputResolusion { get => m_OutputResolusion; set => m_OutputResolusion = value; }
		public TextureFlipFlags FlipFlags { get => m_FlipFlags; set => m_FlipFlags = value; }
		public ECaptureDevice CaptureDevice { get => m_CaptureDevice; set => m_CaptureDevice = value; }
		public EResizeMode ResizeMode { get => m_ResizeMode; set => m_ResizeMode = value; }
		public int Timeout { get => m_Timeout; set => m_Timeout = value; }
		public bool DoubleBuffering { get => m_DoubleBuffering; set => m_DoubleBuffering = value; }
		public bool HideWarnings { get => m_HideWarnings; set => m_HideWarnings = value; }
	}

	public class UnityCaptureRenderFeature : ScriptableRendererFeature {

		[SerializeField]
		UnityCaptureSettings m_Settings = new UnityCaptureSettings();

		UnityCaptureRenderPass m_RenderPass;

		public UnityCaptureSettings Settings => m_Settings;

		public override void Create () {
			m_RenderPass = new UnityCaptureRenderPass(m_Settings) {
				renderPassEvent = RenderPassEvent.AfterRendering
			};
		}

		public override void AddRenderPasses (ScriptableRenderer renderer,ref RenderingData renderingData) {
#if UNITY_EDITOR
			if (!Application.isPlaying || renderingData.cameraData.cameraType != CameraType.Game) {
				return;
			}
#endif
			renderer.EnqueuePass(m_RenderPass);
		}

		protected override void Dispose (bool disposing) {
			base.Dispose(disposing);
			m_RenderPass.Dispose();
		}

		class UnityCaptureRenderPass : ScriptableRenderPass, IDisposable {

			UnityCaptureSettings m_Settings;
			UnityCaptureInterface CaptureInterface;
			RenderTexture m_RenderTexture;

			public UnityCaptureRenderPass (UnityCaptureSettings settings) {
				CaptureInterface = new UnityCaptureInterface(ECaptureDevice.CaptureDevice1);
				m_RenderTexture = new RenderTexture(settings.OutputResolusion.x,settings.OutputResolusion.y,0,RenderTextureFormat.ARGB32);
				m_Settings = settings;
			}

			public override void Execute (ScriptableRenderContext context,ref RenderingData renderingData) {
				CommandBuffer cmd = CommandBufferPool.Get();

				RenderTargetIdentifier src = BuiltinRenderTextureType.CurrentActive;
				RenderTargetIdentifier dst = m_RenderTexture;

				var (scale, offset) = GetTextureOrientation(m_Settings.FlipFlags);
				cmd.Blit(src,dst,scale,offset);

				context.ExecuteCommandBuffer(cmd);

				var result = CaptureInterface.SendTexture(m_RenderTexture,m_Settings.Timeout,m_Settings.DoubleBuffering,m_Settings.ResizeMode,EMirrorMode.Disabled);
				Log(result,m_Settings.HideWarnings);

				CommandBufferPool.Release(cmd);
			}

			public void Dispose () {
				CaptureInterface.Close();

				UnityEngine.Object.Destroy(m_RenderTexture);
				m_RenderTexture = null;
			}

			static (Vector2 scale, Vector2 offset) GetTextureOrientation (TextureFlipFlags flags) {
				bool flipHorizontally = (flags & TextureFlipFlags.FlipHorizontally) == TextureFlipFlags.FlipHorizontally;
				bool flipVertically = (flags & TextureFlipFlags.FlipVertically) == TextureFlipFlags.FlipVertically;
				return (
					new Vector2(flipHorizontally ? -1f : 1f,flipVertically ? -1f : 1f),
					new Vector2(flipHorizontally ? 1f : 0f,flipVertically ? 1f : 0f)
				);
			}

			static void Log (ECaptureSendResult result,bool hideWarnings) {
				switch (result) {
					case ECaptureSendResult.SUCCESS:
						break;
					case ECaptureSendResult.WARNING_FRAMESKIP:
						if (!hideWarnings) {
							Debug.LogWarning("[UnityCapture] Capture device did skip a frame read, capture frame rate will not match render frame rate.");
						}
						break;
					case ECaptureSendResult.WARNING_CAPTUREINACTIVE:
						if (!hideWarnings) {
							Debug.LogWarning("[UnityCapture] Capture device is inactive");
						}
						break;
					case ECaptureSendResult.ERROR_UNSUPPORTEDGRAPHICSDEVICE:
						Debug.LogError("[UnityCapture] Unsupported graphics device (only D3D11 supported)");
						break;
					case ECaptureSendResult.ERROR_PARAMETER:
						Debug.LogError("[UnityCapture] Input parameter error");
						break;
					case ECaptureSendResult.ERROR_TOOLARGERESOLUTION:
						Debug.LogError("[UnityCapture] Render resolution is too large to send to capture device");
						break;
					case ECaptureSendResult.ERROR_TEXTUREFORMAT:
						Debug.LogError("[UnityCapture] Render texture format is unsupported (only basic non-HDR (ARGB32) and HDR (FP16/ARGB Half) formats are supported)");
						break;
					case ECaptureSendResult.ERROR_READTEXTURE:
						Debug.LogError("[UnityCapture] Error while reading texture image data");
						break;
					case ECaptureSendResult.ERROR_INVALIDCAPTUREINSTANCEPTR:
						Debug.LogError("[UnityCapture] Invalid Capture Instance Pointer");
						break;
				}
			}
		}
	}
}