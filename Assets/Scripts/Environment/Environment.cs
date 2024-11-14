using UnityEngine;

[ExecuteInEditMode]
public class Environment : MonoBehaviour
{
    public SODataEnvironment environment;
    private GPUDictionary gpu;

    private void Update()
    {
        if (environment == null)
            return;

        for (int i = 0; i < environment.dataEnvironement.Count; i++)
        {
            gpu = environment.dataEnvironement[i];
            Graphics.RenderMeshInstanced(new RenderParams(gpu.renderer.material), gpu.renderer.mesh, 0, gpu.matrixTransform);
        }
    }
}