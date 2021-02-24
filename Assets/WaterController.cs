using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Controlls the water
public class WaterController : MonoBehaviour
{
    public static WaterController current;

    private LuxWaterUtils.GersterWavesDescription Description;
    public Material WaterMaterial;
    public bool isMoving;
    //Wave height and speed
    public float scale = 50f;
    public float speed = 50.0f;
    //The width between the waves
    public float waveDistance = 2f;
    //Noise parameters
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;
    public GameObject display;
    public List<Wave> Waves = new List<Wave>();
    //public Material WavesMaterial;
    void Start()
    {
        LuxWaterUtils.GetGersterWavesDescription(ref Description, WaterMaterial);
        Waves.Add(new Wave()
        {
            x = 1f,
            y = 0f,
            wavelength = 20,
            steepness = 0.15f

        });
        Vector4[] WaveArray = new Vector4[Waves.Count];
        for (int i = 0; i < Waves.Count; i++)
            WaveArray[i] = new Vector4(Waves[i].x, Waves[i].y, Waves[i].steepness, Waves[i].wavelength);
        //WavesMaterial.SetVectorArray("_Waves", WaveArray);
       // WavesMaterial.SetInt("_WavesCount", Waves.Count);
        current = this;
    }
    private void Update()
    {
      /*  Vector4[] WaveArray = new Vector4[Waves.Count];
        for (int i = 0; i < Waves.Count; i++)
            WaveArray[i] = new Vector4(Waves[i].x, Waves[i].y, Waves[i].steepness, Waves[i].wavelength);
        WavesMaterial.SetVectorArray("_Waves", WaveArray);
        WavesMaterial.SetInt("_WavesCount", Waves.Count);
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for(int z = 0; z < 100; z++)
        {
           /* float y = GetWaveYPos(new Vector3(0, 0, z), Time.time);
            GameObject go = Instantiate(display, this.transform);
            go.transform.position = new Vector3(0, y, z);
        }*/
    }
    //Get the y coordinate from whatever wavetype we are using
    public float GetWaveYPos(Vector3 position, float timeSinceStart)
    {
        /*if (isMoving)
        {
            float y = WaveTypes.SinXWave(position, speed, scale, waveDistance, noiseStrength, noiseWalk, timeSinceStart);
            return y;
        }
        else
        {
            return 0f;
        }*/
        //	Check for material – you could add a check here if Gerstner Waves are enabled
       
      /*  if (UpdateWaterMaterialPerFrame)
        {
            //	Update the Gestner Wave settings from the material if needed
            LuxWaterUtils.GetGersterWavesDescription(ref Description, WaterMaterial);
        }*/

        //	Get the offset of the Gerstner displacement. We have to pass:
        //	- a sample location in world space,
        //	- the Gestner Wave settings from the material sttored in our Description struct,
        //	- a time offset (in seconds) which lets us create an effect of the inertia of masses.
        Vector3 Offset = LuxWaterUtils.GetGestnerDisplacement(position, Description, 0);

        //	We assume that the object itself does not move.
        
        Vector3 newPos = position;
        newPos.x += Offset.x;
        newPos.y += Offset.y;
        newPos.z += Offset.z;
    
        return Offset.y;
    }

    //Find the distance from a vertice to water
    //Make sure the position is in global coordinates
    //Positive if above water
    //Negative if below water
    public float DistanceToWater(Vector3 position, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(position, timeSinceStart);

        float distanceToWater = position.y - waterHeight;

        return distanceToWater;
    }
}
public class Wave
{
    public float x { get; set; }
    public float y { get; set; }
    public float wavelength { get; set; }
    public float steepness { get; set; }
}