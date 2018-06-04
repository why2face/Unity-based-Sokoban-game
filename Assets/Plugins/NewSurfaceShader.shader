Shader "Custom/NewSurfaceShader" {
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" { }
	_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline width", Range(0.0, 0.1)) = .005
	}

		SubShader
	{
		Pass
	{
		ZWrite Off  //注一  

		CGPROGRAM
#include "UnityCG.cginc"  
#pragma vertex vert  
#pragma fragment frag  

		struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 pos : POSITION;
	};

	uniform float _OutlineWidth;
	uniform float4 _OutlineColor;

	v2f vert(appdata v) {
		v2f o;
		v.vertex.xyz += v.normal * _OutlineWidth;    //注二     
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		return o;
	}

	half4 frag(v2f i) : COLOR{
		return _OutlineColor;
	}

		ENDCG
	}

		Pass    //注三   
	{
		CGPROGRAM
#include "UnityCG.cginc"  
#pragma vertex vert_img   
#pragma fragment frag             

		sampler2D _MainTex;

	float4 frag(v2f_img i) : COLOR{
		float4 col = tex2D(_MainTex, i.uv);
		return col;
	}
		ENDCG
	}
	}
		Fallback "Diffuse"
}
