/*
  This sample code is for demonstrating and testing the functionality
  of Unity Capture, and is placed in the public domain.
*/

using UnityEngine;

public class CubesSwayBeeps : MonoBehaviour
{
    public float RotationAmount = 6f, RotationSpeedX = 2.5f, RotationSpeedY = 1.75f;
    public int CubeCount = 10;
    public Material CubeMaterial = null;
    public bool EnableSyncBeeps = true;
    public float BeepDuration = 1/60f*3;
    public Color[] PerFrameBackgroundColors = new Color[] { Color.gray };

    Camera CameraComp;
    Transform[] CubeTransforms;
    Vector3[] CubeSpeeds;
    ulong AudioPosition;
    float AudioStartTime;

    void Start()
    {
        CameraComp = GetComponent<Camera>();

        int TotalCubeCount = CubeCount*CubeCount*CubeCount;
        CubeTransforms = new Transform[TotalCubeCount];
        CubeSpeeds = new Vector3[TotalCubeCount];
        GameObject CubeHolder = new GameObject("CubeHolder");
        for (int i = 0; i < TotalCubeCount; i++)
        {
            int x = (i / (CubeCount * CubeCount)), y = ((i / CubeCount) % CubeCount), z = (i % CubeCount);
            GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
            o.transform.parent = CubeHolder.transform;
            o.transform.position = new Vector3(5 * (x - CubeCount / 2), 5 * (y - CubeCount / 2), 5 * (z - CubeCount / 2));
            if (CubeMaterial != null) o.GetComponent<MeshRenderer>().material = CubeMaterial;
            CubeTransforms[i] = o.transform;
            CubeSpeeds[i] = Random.insideUnitSphere * 50;
        }

        if (EnableSyncBeeps)
        {
            AudioClip myClip = AudioClip.Create("AudioBeeps", 44100/60, 1, 44100, true, OnAudioRead);
            AudioSource aud = gameObject.AddComponent<AudioSource>();
            aud.clip = myClip;
            aud.loop = true;
            AudioPosition = 0;
            AudioStartTime = Time.realtimeSinceStartup;
            aud.Play();
        }
    }

    void OnAudioRead(float[] data)
    {
        const float SineSpeed = 440f * Mathf.PI * 2f / 44100f; //440hz beeps
        for (int count = 0; count < data.Length; AudioPosition++, count++)
        {
            bool SoundDoBeep = ((AudioPosition % 44100) < (44100*BeepDuration)); //beep for 1 second
            data[count] = Mathf.Sign(Mathf.Sin(SineSpeed * AudioPosition)) * (SoundDoBeep ? 1f : 0f);
        }
    }

    void Update()
    {
        transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Cos(Time.time * RotationSpeedX), Mathf.Sin(Time.time * RotationSpeedY)) * RotationAmount);

        for (int i = 0; i < CubeTransforms.Length; i++)
        {
            CubeTransforms[i].rotation = Quaternion.Euler(CubeSpeeds[i] * Time.time);
        }

        CameraComp.backgroundColor = PerFrameBackgroundColors[Time.frameCount % PerFrameBackgroundColors.Length];

        if (EnableSyncBeeps)
        {
            bool SoundDoBeep = (((Time.realtimeSinceStartup - AudioStartTime) % 1.0f) < BeepDuration);
            if (SoundDoBeep) CameraComp.backgroundColor = Color.red;
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(20, 20, 250, 25), "Drawing in OnGUI is not captured");
    }
}
