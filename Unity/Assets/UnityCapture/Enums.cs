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

namespace MackySoft.UnityCapture {

    public enum ECaptureDevice {
        CaptureDevice1 = 0,
        CaptureDevice2 = 1,
        CaptureDevice3 = 2,
        CaptureDevice4 = 3,
        CaptureDevice5 = 4,
        CaptureDevice6 = 5,
        CaptureDevice7 = 6,
        CaptureDevice8 = 7,
        CaptureDevice9 = 8,
        CaptureDevice10 = 9
    }

    public enum EResizeMode {
        Disabled = 0,
        LinearResize = 1
    }

    public enum EMirrorMode {
        Disabled = 0,
        MirrorHorizontally = 1
    }

    public enum ECaptureSendResult {
        SUCCESS = 0,
        WARNING_FRAMESKIP = 1,
        WARNING_CAPTUREINACTIVE = 2,
        ERROR_UNSUPPORTEDGRAPHICSDEVICE = 100,
        ERROR_PARAMETER = 101,
        ERROR_TOOLARGERESOLUTION = 102,
        ERROR_TEXTUREFORMAT = 103,
        ERROR_READTEXTURE = 104,
        ERROR_INVALIDCAPTUREINSTANCEPTR = 200
    }
}