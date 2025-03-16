using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutOfMaskUI : Image
{
    private Material _customMaterial;

    protected override void OnEnable()
    {
        base.OnEnable();

        this.material = CreateMaterialForRendering();  // material을 설정

        // 렌더링 업데이트를 위한 설정
        SetMaterialDirty();  // 렌더링 변경 요청
    }

    public override Material materialForRendering
    {
        get
        {
            // 이미 설정된 material을 반환하도록 해서 중복 생성 방지
            if (_customMaterial == null)
            {
                _customMaterial = CreateMaterialForRendering();
            }

            return _customMaterial;
        }
    }

    private Material CreateMaterialForRendering()
    {
        // 기본 material을 가져오고, 새로 Stencil을 적용한 material 생성
        Material newMaterial = new Material(base.materialForRendering);

        // 원하는 Stencil 설정을 추가
        newMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);  // Stencil 비교 방식 변경 (NotEqual)

        return newMaterial;
    }
}
