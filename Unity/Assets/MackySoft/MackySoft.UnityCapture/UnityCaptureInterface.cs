/*
  Unity Capture
  Copyright (c) 2018 Bernhard Schelling

  Feature contributors:
	Brandon J Matthews (low-level interface for custom texture capture)

  Based on UnityCam
  https://github.com/mrayy/UnityCam
  Copyright (c) 2016 MHD Yamen Saraiji

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
	 claim that you wrote the original software. If you use this software
	 in a product, an acknowledgment in the product documentation would be
	 appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
	 misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using UnityEngine;

namespace MackySoft.UnityCapture {
	internal class UnityCaptureInterface {

		[System.Runtime.InteropServices.DllImport("UnityCapturePlugin")]
		extern static System.IntPtr CaptureCreateInstance (int CapNum);

		[System.Runtime.InteropServices.DllImport("UnityCapturePlugin")]
		extern static void CaptureDeleteInstance (System.IntPtr instance);

		[System.Runtime.InteropServices.DllImport("UnityCapturePlugin")]
		extern static ECaptureSendResult CaptureSendTexture (System.IntPtr instance,System.IntPtr nativetexture,int Timeout,bool UseDoubleBuffering,EResizeMode ResizeMode,EMirrorMode MirrorMode,bool IsLinearColorSpace);

		System.IntPtr CaptureInstance;

		public UnityCaptureInterface (ECaptureDevice CaptureDevice) {
			CaptureInstance = CaptureCreateInstance((int)CaptureDevice);
		}

		~UnityCaptureInterface () {
			Close();
		}

		public void Close () {
			if (CaptureInstance != System.IntPtr.Zero)
				CaptureDeleteInstance(CaptureInstance);
			CaptureInstance = System.IntPtr.Zero;
		}

		public ECaptureSendResult SendTexture (Texture Source,int Timeout = 1000,bool DoubleBuffering = false,EResizeMode ResizeMode = EResizeMode.Disabled,EMirrorMode MirrorMode = EMirrorMode.Disabled) {
			if (CaptureInstance == System.IntPtr.Zero)
				return ECaptureSendResult.ERROR_INVALIDCAPTUREINSTANCEPTR;
			return CaptureSendTexture(CaptureInstance,Source.GetNativeTexturePtr(),Timeout,DoubleBuffering,ResizeMode,MirrorMode,QualitySettings.activeColorSpace == ColorSpace.Linear);
		}
	}
}