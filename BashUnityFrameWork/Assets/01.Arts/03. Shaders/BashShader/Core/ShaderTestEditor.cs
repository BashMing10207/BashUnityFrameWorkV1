//using DG.Tweening;
using UnityEngine;

public class ShaderTestEditor : MonoBehaviour
{
    [SerializeField]
    private string _paramName = "_Bokeh";
    [SerializeField]
    private float _paramValue=0.5f;

    [ContextMenu("Set Static SHADER PARAM")]
    private void SetStaticShaderParam()
    {
        Shader.SetGlobalFloat(_paramName, _paramValue); // shader gloabal parameter set => all materials using this shader will be affected

    }
    //[ContextMenu("Set Static SHADER PARAM DFOTWEEN")]
    //private void SetStaticShaderParamDOTWEEN()
    //{
    //   DOTween.To(() => Shader.GetGlobalFloat(_paramName), x => Shader.SetGlobalFloat(_paramName, x), _paramValue, 1f);
    //}


}
