// Upgrade NOTE: replaced 'SeperateSpecular' with 'SeparateSpecular'


Shader "Custom/MenuItemShader" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,0.5)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_AlphaTex("Trans. (Alpha)", 2D) = "white" {}
	}

		Category{
		ZWrite Off
		Alphatest Greater 0
		Tags{ Queue = Transparent }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		SubShader{
		Material{
		Diffuse[_Color]
		Ambient[_Color]
		Shininess[_Shininess]
		Specular[_SpecColor]
		Emission[_Emission]
	}
		Pass{
		Lighting On
		SeparateSpecular On
		SetTexture[_MainTex]{
		Combine texture * primary DOUBLE, texture
	}
		SetTexture[_AlphaTex]{
		constantColor[_Color]
		Combine previous, texture * constant
	}
	}
	}
	}
}



/*
Shader "Custom/MenuItemShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
*/