using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class CutOfMaskUI : Image
{
    private Material _cachedMaterial;
    protected override void OnEnable()
    {
        base.OnEnable();  // 기본 동작 유지

        if (_cachedMaterial == null) // 한 번만 생성
        {
            _cachedMaterial = new Material(base.materialForRendering);
            _cachedMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
        }

        _cachedMaterial.SetInt("_StencilComp", (int)CompareFunction.Equal);
        this.material = _cachedMaterial;  // 다시 적용
    }

    public override Material materialForRendering
    {
        get
        {
            if (_cachedMaterial == null)
            {
                _cachedMaterial = new Material(base.materialForRendering);
                _cachedMaterial.SetInt("_StencilComp", (int)CompareFunction.Equal);
            }
            this.material = _cachedMaterial;
            return _cachedMaterial;
        }
    }
}